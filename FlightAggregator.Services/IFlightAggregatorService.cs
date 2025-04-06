using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FlightAggregator.Models;
using FlightAggregator.Models.ProviderModels;

namespace FlightAggregator.Services;

public interface IFlightAggregatorService
{
    IAsyncEnumerable<List<Flight>> SearchFlightsAsync(string departure,
        string destination,
        DateTime date,
        int? maxStops,
        decimal? maxPrice,
        string? airline,
        string? sortBy,
        CancellationToken cancellationToken);
    
    Task<bool> BookFlightAsync(BookingRequest request, CancellationToken cancellationToken);
}