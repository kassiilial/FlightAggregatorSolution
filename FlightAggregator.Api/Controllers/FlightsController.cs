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
        [HttpPost("stream")]
        [Produces("application/json")]
        [SwaggerRequestExample(typeof(FlightSearchRequest), typeof(FlightRequestExample))]
        public IAsyncEnumerable<Flight> SearchFlights([FromBody] FlightSearchRequest request, CancellationToken cancellationToken)
        {
            return aggregatorService.SearchFlightsAsync(
                request.Departure,
                request.Destination,
                request.Date!.Value,
                request.MaxStops,
                request.MaxPrice,
                request.Airline,
                request.SortBy,
                cancellationToken);
        }
    }

