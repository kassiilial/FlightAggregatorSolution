using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;
using FlightAggregator.Models.ProviderModels;
using FlightAggregator.Services;

namespace FlightAggregator.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FlightsController(
        IFlightAggregatorService aggregatorService,
        IDistributedCache cache,
        ILogger<FlightsController> logger)
        : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> GetFlights([FromQuery] string departure,
            [FromQuery] string destination,
            [FromQuery] DateTime date,
            [FromQuery] int maxStops,
            [FromQuery] decimal maxPrice,
            CancellationToken cancellationToken)
        {
            var cacheKey = $"flights-{departure}-{destination}-{date:yyyyMMdd}";
            var cachedData = await cache.GetStringAsync(cacheKey, cancellationToken);
            List<Flight> flights;

            if (string.IsNullOrEmpty(cachedData))
            {
                logger.LogInformation("Cache miss for key: {CacheKey}", cacheKey);
                flights = await aggregatorService.SearchFlightsAsync(departure, destination, date, cancellationToken);

                flights = flights.Where(f => f.Stops <= maxStops && f.Price <= maxPrice)
                    .OrderBy(f => f.Price)
                    .ThenBy(f => f.Date)
                    .ToList();

                var options = new DistributedCacheEntryOptions
                {
                    SlidingExpiration = TimeSpan.FromHours(1)
                };

                var serializedFlights = JsonSerializer.Serialize(flights);
                await cache.SetStringAsync(cacheKey, serializedFlights, options, cancellationToken);
            }
            else
            {
                logger.LogInformation("Cache hit for key: {CacheKey}", cacheKey);
                flights = JsonSerializer.Deserialize<List<Flight>>(cachedData)
                    .Where(f => f.Stops <= maxStops && f.Price <= maxPrice)
                    .ToList();
            }

            return Ok(flights);
        }
    }
}