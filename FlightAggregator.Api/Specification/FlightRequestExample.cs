using FlightAggregator.Models;
using Swashbuckle.AspNetCore.Filters;

namespace FlightAggregator.Api.Specification;

public class FlightRequestExample : IExamplesProvider<Flight>
{
    public Flight GetExamples()
    {
        return new Flight
        {
            Departure = "Moscow",
            Destination = "Paris",
            Date = DateTime.Parse("2025-04-10")
        };
    }
}