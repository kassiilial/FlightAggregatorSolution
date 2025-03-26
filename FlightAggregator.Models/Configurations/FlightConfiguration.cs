using System.Collections.Generic;
using FlightAggregator.Models.ProviderModels;

namespace FlightAggregator.Models.Configurations;

public class FlightConfiguration
{
    // ReSharper disable once CollectionNeverUpdated.Global
    public List<Flight> Provider1Flights { get; set; } = [];

    // ReSharper disable once CollectionNeverUpdated.Global
    public List<Flight> Provider2Flights { get; set; } = [];
}