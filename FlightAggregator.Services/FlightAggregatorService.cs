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

        public async IAsyncEnumerable<List<Flight>> SearchFlightsAsync(
            string departure,
            string destination,
            DateTime date,
            int? maxStops,
            decimal? maxPrice,
            string? airline,
            string? sortBy,
            [EnumeratorCancellation] CancellationToken cancellationToken)
        {
            var flightTasks = providers
                .Select(provider => provider.GetFlightsAsync(departure, destination, date, cancellationToken))
                .ToList();

            if (string.IsNullOrWhiteSpace(sortBy))
            {
                while (flightTasks.Any())
                {
                    var flightTask = await Task.WhenAny(flightTasks);
                    flightTasks.Remove(flightTask);
                    yield return flightTask.Result;
                }
            }
            else
            {
                while (flightTasks.Any())
                {
                    var flightTask = await Task.WhenAny(flightTasks);
                    flightTasks.Remove(flightTask);
                    yield return flightTask.Result;
                }
            }
        }
    }
}