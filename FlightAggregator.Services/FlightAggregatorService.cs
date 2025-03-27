using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using FlightAggregator.Models;
using FlightAggregator.Models.ProviderModels;
using FlightAggregator.Providers.Interfaces;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;

namespace FlightAggregator.Services;

public class FlightAggregatorService(IEnumerable<IFlightProvider> providers, ILogger<FlightAggregatorService> logger, IDistributedCache cache)
    : IFlightAggregatorService
{
    public async Task<List<Flight>> SearchFlightsAsync(
            string departure, 
            string destination, 
            DateTime date,
            int? maxStops,
            decimal? maxPrice,
            string? airline,
            string? sortBy,
            CancellationToken cancellationToken)
        {
            var cacheKey = $"flights-{departure}-{destination}-{date:yyyyMMdd}-{maxStops}-{maxPrice}-{airline}-{sortBy}";
            var cachedData = await cache.GetStringAsync(cacheKey, cancellationToken);

            if (!string.IsNullOrEmpty(cachedData))
            {
                logger.LogInformation("Cache hit for key: {CacheKey}", cacheKey);
                return JsonSerializer.Deserialize<List<Flight>>(cachedData) ?? [];
            }

            logger.LogInformation("Cache miss for key: {CacheKey}", cacheKey);

            var tasks = providers.Select(provider =>
                provider.GetFlightsAsync(departure, destination, date, cancellationToken));
            var results = await Task.WhenAll(tasks);

            var allFlights = results.SelectMany(flights => flights);
            if (maxStops.HasValue)
                allFlights = allFlights.Where(f => f.Stops <= maxStops.Value);
            if (maxPrice.HasValue)
                allFlights = allFlights.Where(f => f.Price <= maxPrice.Value);
            if (!string.IsNullOrWhiteSpace(airline))
                allFlights = allFlights.Where(f => f.Airline.Equals(airline, StringComparison.OrdinalIgnoreCase));

            allFlights = sortBy?.ToLower() switch
            {
                "price" => allFlights.OrderBy(f => f.Price),
                "stops" => allFlights.OrderBy(f => f.Stops),
                "date" => allFlights.OrderBy(f => f.Date),
                _ => allFlights.OrderBy(f => f.Price).ThenBy(f => f.Date)
            };

            var flightsList = allFlights.ToList();

            var options = new DistributedCacheEntryOptions
            {
                SlidingExpiration = TimeSpan.FromMinutes(5)
            };
            await cache.SetStringAsync(cacheKey, JsonSerializer.Serialize(flightsList), options, cancellationToken);

            return flightsList;
        }
    
    public async Task<bool> BookFlightAsync(BookingRequest request, CancellationToken cancellationToken)
    {
        var provider = providers.FirstOrDefault(p => p.ProviderName.Equals(request.Provider, StringComparison.OrdinalIgnoreCase));
        
        if (provider == null)
        {
            logger.LogWarning("Не найден провайдер для рейса {FlightId}", request.FlightId);
            return false;
        }

        return await provider.BookFlightAsync(request, cancellationToken);
    }
}