using System;

namespace FlightAggregator.Providers.Interfaces;

public interface IFlightProvider
    {
        /// <summary>
        /// Возвращает список перелётов по заданным параметрам.
        /// </summary>
        Task<List<Flight>> GetFlightsAsync(string departure, string destination, DateTime date, CancellationToken cancellationToken);

        /// <summary>
        /// Симуляция бронирования рейса.
        /// </summary>
        Task<bool> BookFlightAsync(BookingRequest request, CancellationToken cancellationToken);
    }
