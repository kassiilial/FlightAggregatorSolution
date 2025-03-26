
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FlightAggregator.Business;
using FlightAggregator.Models.Entities;

namespace FlightAggregator.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FlightsController : ControllerBase
    {
        private readonly IFlightAggregatorService _aggregatorService;
        private readonly IDistributedCache _cache;
        private readonly ILogger<FlightsController> _logger;

        public FlightsController(IFlightAggregatorService aggregatorService, IDistributedCache cache, ILogger<FlightsController> logger)
        {
            _aggregatorService = aggregatorService;
            _cache = cache;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetFlights([FromQuery] string departure,
                                                      [FromQuery] string destination,
                                                      [FromQuery] DateTime date,
                                                      [FromQuery] int maxStops,
                                                      [FromQuery] decimal maxPrice,
                                                      CancellationToken cancellationToken)
        {
            // Формирование ключа кэша на основе параметров запроса
            string cacheKey = $"flights-{departure}-{destination}-{date:yyyyMMdd}-{maxStops}-{maxPrice}";
            string cachedData = await _cache.GetStringAsync(cacheKey, cancellationToken);
            List<Flight> flights;

            if (string.IsNullOrEmpty(cachedData))
            {
                _logger.LogInformation("Cache miss for key: {CacheKey}", cacheKey);
                flights = await _aggregatorService.SearchFlightsAsync(departure, destination, date, cancellationToken);

                // Фильтрация и сортировка
                flights = flights.Where(f => f.Stops <= maxStops && f.Price <= maxPrice)
                                 .OrderBy(f => f.Price)
                                 .ThenBy(f => f.Date)
                                 .ToList();

                // Сохраняем результат в кэш (слайдинг-экспирация 5 минут)
                var options = new DistributedCacheEntryOptions
                {
                    SlidingExpiration = TimeSpan.FromMinutes(5)
                };

                string serializedFlights = JsonSerializer.Serialize(flights);
                await _cache.SetStringAsync(cacheKey, serializedFlights, options, cancellationToken);
            }
            else
            {
                _logger.LogInformation("Cache hit for key: {CacheKey}", cacheKey);
                flights = JsonSerializer.Deserialize<List<Flight>>(cachedData);
            }

            return Ok(flights);
        }
    }
}
