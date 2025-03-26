using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FlightAggregator.Models.ProviderModels;
using FlightAggregator.Providers.Interfaces;
using Microsoft.Extensions.Logging;

namespace FlightAggregator.Services;

public class FlightAggregatorService(IEnumerable<IFlightProvider> providers, ILogger<FlightAggregatorService> logger)
    : IFlightAggregatorService
{
    public async Task<List<Flight>> SearchFlightsAsync(string departure, string destination, DateTime date,
        CancellationToken cancellationToken)
    {
        var tasks = providers.Select(provider =>
            provider.GetFlightsAsync(departure, destination, date, cancellationToken));
        var results = await Task.WhenAll(tasks);
        return results.SelectMany(flights => flights).ToList();
    }

    public async Task<bool> BookFlightAsync(BookingRequest request, CancellationToken cancellationToken)
    {
        var provider = providers.FirstOrDefault(p => request.FlightId.StartsWith("FP1"))
                       ?? providers.FirstOrDefault(p => request.FlightId.StartsWith("FP2"));

        if (provider == null)
        {
            logger.LogWarning("Не найден провайдер для рейса {FlightId}", request.FlightId);
            return false;
        }

        return await provider.BookFlightAsync(request, cancellationToken);
    }
}