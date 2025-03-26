using System;

namespace FlightAggregator.Business;

 public interface IFlightAggregatorService
    {
        Task<List<Flight>> SearchFlightsAsync(string departure, string destination, DateTime date, CancellationToken cancellationToken);
        Task<bool> BookFlightAsync(BookingRequest request, CancellationToken cancellationToken);
    }
