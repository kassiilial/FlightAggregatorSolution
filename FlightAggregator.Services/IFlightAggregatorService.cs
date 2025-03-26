using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FlightAggregator.Models.ProviderModels;

namespace FlightAggregator.Services;

public interface IFlightAggregatorService
{
    Task<List<Flight>> SearchFlightsAsync(string departure, string destination, DateTime date,
        CancellationToken cancellationToken);

    Task<bool> BookFlightAsync(BookingRequest request, CancellationToken cancellationToken);
}