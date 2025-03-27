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
        ILogger<FlightsController> logger)
        : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> GetFlights(
            [FromQuery] string departure,
            [FromQuery] string destination,
            [FromQuery] DateTime date,
            [FromQuery] int? maxStops,
            [FromQuery] decimal? maxPrice,
            [FromQuery] string? airline,
            [FromQuery] string? sortBy,
            CancellationToken cancellationToken)
        {
            var flights = await aggregatorService.SearchFlightsAsync(departure, destination, date, maxStops, maxPrice, airline, sortBy, cancellationToken);
            
            return Ok(flights);
        }
    }
}
