using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FlightAggregator.Models;
using FlightAggregator.Models.ProviderModels;

namespace FlightAggregator.Providers.Interfaces;

public interface IFlightProvider
{
    string ProviderName { get; }
    
    IAsyncEnumerable<Flight> GetFlightsAsync(string departure, string destination, DateTime date,
        CancellationToken cancellationToken);

    Task<bool> BookFlightAsync(BookingRequest request, CancellationToken cancellationToken);
}