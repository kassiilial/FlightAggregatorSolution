using System;

namespace FlightAggregator.Models.ProviderModels
{
    public record Flight(
        string Id,
        string Departure,
        string Destination,
        DateTime Date,
        decimal Price,
        int Stops,
        string Airline,
        string Provider
    );
}