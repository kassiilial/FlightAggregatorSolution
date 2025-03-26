using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FlightAggregator.Models.Configurations;
using FlightAggregator.Models.ProviderModels;
using FlightAggregator.Providers.Interfaces;

namespace FlightAggregator.Providers.ExternalProviders
{
    public class FlightProvider2 : IFlightProvider
    {
        public async Task<List<Flight>> GetFlightsAsync(string departure, string destination, DateTime date, CancellationToken cancellationToken)
        {
            // Симулируем более длительную задержку
            await Task.Delay(1500, cancellationToken);
            return TestDataConfig.Provider2Flights;
        }

        public async Task<bool> BookFlightAsync(BookingRequest request, CancellationToken cancellationToken)
        {
            await Task.Delay(500, cancellationToken);
            return true;
        }
    }
}