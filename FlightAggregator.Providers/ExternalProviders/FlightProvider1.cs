using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using FlightAggregator.Models;
using FlightAggregator.Models.Configurations;
using FlightAggregator.Models.ProviderModels;
using FlightAggregator.Providers.Interfaces;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace FlightAggregator.Providers.ExternalProviders;

public class FlightProvider1(
    IOptions<FlightConfiguration> options,
    IDistributedCache cache,
    ILogger<FlightProvider1> logger)
    : IFlightProvider
{
    private readonly FlightConfiguration _configuration = options.Value;

    public string ProviderName => "Provider1";

    public async IAsyncEnumerable<Flight> GetFlightsAsync(
        string departure,
        string destination,
        DateTime date,
        [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        var cacheKey = GetCacheKey(departure, destination, date);

        var cachedFlights = await GetFlightsFromCacheAsync(cacheKey, cancellationToken);
        if (cachedFlights is { Count: > 0 })
        {
            logger.LogInformation("Cache hit for key {CacheKey}", cacheKey);
            foreach (var flight in cachedFlights)
            {
                yield return flight;
            }
            yield break;
        }

        logger.LogInformation("Cache miss for key {CacheKey}", cacheKey);

        await Task.Delay(500, cancellationToken);

        var flightsFromProvider = _configuration.Provider1Flights
            .Where(f => f.Departure == departure
                        && f.Destination == destination
                        && f.Date.Date == date.Date);

        var flights = new List<Flight>();
        foreach (var flight in flightsFromProvider)
        {
            var flightItem = new Flight
            {
                FlightNumber = flight.FlightNumber,
                Departure = flight.Departure,
                Destination = flight.Destination,
                Date = flight.Date,
                Price = flight.Price,
                Stops = flight.Stops,
                Airline = flight.Airline,
                Provider = ProviderName
            };
            flights.Add(flightItem);
            yield return flightItem;
        }

        if (flights.Any())
        {
            await SetFlightsToCacheAsync(cacheKey, flights, cancellationToken);
        }
    }

    public async Task<bool> BookFlightAsync(BookingRequest request, CancellationToken cancellationToken)
    {
        if (!EnvironmentHelper.IsDevelopment())
            return false;

        await Task.Delay(500, cancellationToken);
        return true;
    }
        
    private string GetCacheKey(string departure, string destination, DateTime date)
    {
        return $"{ProviderName}-flights-{departure}-{destination}-{date:yyyyMMdd}";
    }
        
    private async Task<List<Flight>?> GetFlightsFromCacheAsync(string cacheKey, CancellationToken cancellationToken)
    {
        var cachedData = await cache.GetStringAsync(cacheKey, cancellationToken);
        if (!string.IsNullOrEmpty(cachedData))
        {
            try
            {
                return JsonSerializer.Deserialize<List<Flight>>(cachedData);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Ошибка десериализации данных из кэша по ключу {CacheKey}", cacheKey);
            }
        }
        return null;
    }
        
    private async Task SetFlightsToCacheAsync(string cacheKey, List<Flight> flights, CancellationToken cancellationToken)
    {
        var options = new DistributedCacheEntryOptions
        {
            SlidingExpiration = TimeSpan.FromMinutes(5)
        };

        var data = JsonSerializer.Serialize(flights);
        await cache.SetStringAsync(cacheKey, data, options, cancellationToken);
    }
}