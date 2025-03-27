using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using FlightAggregator.Services;

namespace FlightAggregator.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FlightsController(
        IFlightAggregatorService aggregatorService)
        : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> GetFlights(
            [FromQuery, Required] string departure,
            [FromQuery, Required] string destination,
            [FromQuery, Required] DateTime date,
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
