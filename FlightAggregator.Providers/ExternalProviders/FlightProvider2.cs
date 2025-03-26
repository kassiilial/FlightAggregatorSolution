using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace FlightAggregator.Api.Providers
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