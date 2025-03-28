using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FlightAggregator.Models;
using FlightAggregator.Models.Configurations;
using FlightAggregator.Models.ProviderModels;
using FlightAggregator.Providers.Interfaces;
using Microsoft.Extensions.Options;

namespace FlightAggregator.Providers.ExternalProviders
{
    public class FlightProvider2(IOptions<FlightConfiguration> options) : IFlightProvider
    {
        private readonly FlightConfiguration _configuration = options.Value;

        public string ProviderName => "Provider2";

        public async Task<List<Flight>> GetFlightsAsync(string departure, string destination, DateTime date,
            CancellationToken cancellationToken)
        {
            if (!EnvironmentHelper.IsDevelopment()) return [];

            await Task.Delay(500, cancellationToken);

            var flightsFromProvider = _configuration.Provider2Flights.Where(f => f.Departure == departure
                && f.Destination == destination
                && f.Date.Date == date.Date).ToList();

            return flightsFromProvider.Select(flight => new Flight
                {
                    FlightNumber = flight.FlightNumber,
                    Departure = flight.Departure,
                    Destination = flight.Destination,
                    Date = flight.Date,
                    Price = flight.Price,
                    Stops = flight.Stops,
                    Airline = flight.Airline,
                    Provider = ProviderName
                })
                .ToList();
        }

        public async Task<bool> BookFlightAsync(BookingRequest request, CancellationToken cancellationToken)
        {
            if (!EnvironmentHelper.IsDevelopment()) return false;

            await Task.Delay(500, cancellationToken);
            return true;
        }
    }
}