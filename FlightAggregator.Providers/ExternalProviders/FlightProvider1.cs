using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace FlightAggregator.Api.Providers
{
    public class FlightProvider1 : IFlightProvider
    {
        public async Task<List<Flight>> GetFlightsAsync(string departure, string destination, DateTime date, CancellationToken cancellationToken)
        {
            // Симулируем задержку ответа
            await Task.Delay(1000, cancellationToken);
            // В реальном случае можно фильтровать данные по параметрам запроса
            return TestDataConfig.Provider1Flights;
        }

        public async Task<bool> BookFlightAsync(BookingRequest request, CancellationToken cancellationToken)
        {
            await Task.Delay(500, cancellationToken);
            return true;
        }
    }
}
