using FlightAggregator.Api.Specification;
using FlightAggregator.Models;
using Microsoft.AspNetCore.Mvc;
using FlightAggregator.Services;
using Swashbuckle.AspNetCore.Filters;

namespace FlightAggregator.Api.Controllers;

    [ApiController]
    [Route("api/[controller]")]
    public class FlightsController(
        IFlightAggregatorService aggregatorService)
        : ControllerBase
    {
        [HttpPost]
        [SwaggerRequestExample(typeof(FlightSearchRequest), typeof(FlightRequestExample))]
        public async Task<IActionResult> SearchFlights([FromBody] FlightSearchRequest request, CancellationToken cancellationToken)
        {
            var flights = await aggregatorService.SearchFlightsAsync(
                request.Departure,
                request.Destination,
                request.Date!.Value,
                request.MaxStops,
                request.MaxPrice,
                request.Airline,
                request.SortBy,
                cancellationToken);

            return Ok(flights);
        }
    }

