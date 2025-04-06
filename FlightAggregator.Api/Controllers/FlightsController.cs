using System.Runtime.CompilerServices;
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
        public async IAsyncEnumerable<List<Flight>> SearchFlights([FromBody] FlightSearchRequest request, [EnumeratorCancellation] CancellationToken cancellationToken)
        {
             await foreach (var flights in aggregatorService.SearchFlightsAsync(
                                      request.Departure,
                                      request.Destination,
                                      request.Date!.Value,
                                      request.MaxStops,
                                      request.MaxPrice,
                                      request.Airline,
                                      request.SortBy,
                                      cancellationToken))
            {
                yield return flights;
            }
        }
    }

