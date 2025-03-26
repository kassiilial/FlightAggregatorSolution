using System;

namespace FlightAggregator.Business.Services;

public class FlightAggregatorService : IFlightAggregatorService
    {
        private readonly IEnumerable<IFlightProvider> _providers;
        private readonly ILogger<FlightAggregatorService> _logger;

        public FlightAggregatorService(IEnumerable<IFlightProvider> providers, ILogger<FlightAggregatorService> logger)
        {
            _providers = providers;
            _logger = logger;
        }

        public async Task<List<Flight>> SearchFlightsAsync(string departure, string destination, DateTime date, CancellationToken cancellationToken)
        {
            var tasks = _providers.Select(provider => provider.GetFlightsAsync(departure, destination, date, cancellationToken));
            var results = await Task.WhenAll(tasks);
            return results.SelectMany(flights => flights).ToList();
        }

        public async Task<bool> BookFlightAsync(BookingRequest request, CancellationToken cancellationToken)
        {
            // Определяем провайдера по префиксу идентификатора рейса
            IFlightProvider provider = _providers.FirstOrDefault(p => request.FlightId.StartsWith("FP1"))
                ?? _providers.FirstOrDefault(p => request.FlightId.StartsWith("FP2"));

            if (provider == null)
            {
                _logger.LogWarning("Не найден провайдер для рейса {FlightId}", request.FlightId);
                return false;
            }

            return await provider.BookFlightAsync(request, cancellationToken);
        }
    }
