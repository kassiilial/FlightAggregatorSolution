using System;

namespace FlightAggregator.Models.ProviderModels
{
    public record FlightFromProvider(
        string FlightNumber,
        string Departure,
        string Destination,
        DateTime Date,
        decimal Price,
        int Stops,
        string Airline
    );
}