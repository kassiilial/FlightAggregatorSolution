using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using FlightAggregator.Models;
using FlightAggregator.Models.ProviderModels;
using FlightAggregator.Providers.Interfaces;
using Microsoft.Extensions.Logging;

namespace FlightAggregator.Services
{
    public class FlightAggregatorService(
        IEnumerable<IFlightProvider> providers,
        ILogger<FlightAggregatorService> logger)
        : IFlightAggregatorService
    {
        public async Task<bool> BookFlightAsync(BookingRequest request, CancellationToken cancellationToken)
        {
            var provider = providers
                .FirstOrDefault(p => p.ProviderName.Equals(request.Provider, StringComparison.OrdinalIgnoreCase));

            if (provider == null)
            {
                logger.LogWarning("Не найден провайдер для рейса {FlightId}", request.FlightId);
                return false;
            }

            return await provider.BookFlightAsync(request, cancellationToken);
        }

        public async IAsyncEnumerable<Flight> SearchFlightsAsync(
            string departure,
            string destination,
            DateTime date,
            int? maxStops,
            decimal? maxPrice,
            string? airline,
            string? sortBy,
            [EnumeratorCancellation] CancellationToken cancellationToken)
        {
            var flightStreams = providers
                .Select(provider => provider.GetFlightsAsync(departure, destination, date, cancellationToken))
                .ToList();

            if (string.IsNullOrWhiteSpace(sortBy))
            {
                foreach (var flightStream in flightStreams)
                {
                    await foreach (var flight in flightStream.WithCancellation(cancellationToken))
                    {
                        if (FilterFlight(flight, maxStops, maxPrice, airline))
                            yield return flight;
                    }
                }
            }
            else
            {
                var allFlights = new List<Flight>();
                foreach (var flightStream in flightStreams)
                {
                    await foreach (var flight in flightStream.WithCancellation(cancellationToken))
                    {
                        if (FilterFlight(flight, maxStops, maxPrice, airline))
                            allFlights.Add(flight);
                    }
                }

                allFlights = sortBy.ToLower() switch
                {
                    "price" => allFlights.OrderBy(f => f.Price).ToList(),
                    "stops" => allFlights.OrderBy(f => f.Stops).ToList(),
                    "date" => allFlights.OrderBy(f => f.Date).ToList(),
                    _ => allFlights.OrderBy(f => f.Price).ThenBy(f => f.Date).ToList()
                };

                foreach (var flight in allFlights)
                {
                    yield return flight;
                }
            }
        }

        private bool FilterFlight(Flight flight, int? maxStops, decimal? maxPrice, string? airline)
        {
            if (maxStops.HasValue && flight.Stops > maxStops.Value)
                return false;
            if (maxPrice.HasValue && flight.Price > maxPrice.Value)
                return false;
            if (!string.IsNullOrWhiteSpace(airline) &&
                !flight.Airline.Equals(airline, StringComparison.OrdinalIgnoreCase))
                return false;
            return true;
        }
    }
}