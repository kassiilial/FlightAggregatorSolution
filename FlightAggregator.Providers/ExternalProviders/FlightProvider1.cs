using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FlightAggregator.Models.Configurations;
using FlightAggregator.Models.ProviderModels;
using FlightAggregator.Providers.Interfaces;
using Microsoft.Extensions.Options;

namespace FlightAggregator.Providers.ExternalProviders
{
    public class FlightProvider1(IOptions<FlightConfiguration> options) : IFlightProvider
    {
        private readonly FlightConfiguration _configuration = options.Value;

        public async Task<List<Flight>> GetFlightsAsync(string departure, string destination, DateTime date,
            CancellationToken cancellationToken)
        {
            await Task.Delay(500, cancellationToken);
            return _configuration.Provider1Flights
                .Where(f => f.Departure == departure
                            && f.Destination == destination
                            && f.Date.Date == date.Date).ToList();
        }

        public async Task<bool> BookFlightAsync(BookingRequest request, CancellationToken cancellationToken)
        {
            await Task.Delay(500, cancellationToken);
            return true;
        }
    }
}