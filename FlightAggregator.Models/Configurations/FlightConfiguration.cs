using System.Collections.Generic;
using FlightAggregator.Models.ProviderModels;

namespace FlightAggregator.Models.Configurations;

public class FlightConfiguration
{
    // ReSharper disable once CollectionNeverUpdated.Global
    public List<FlightFromProvider> Provider1Flights { get; set; } = [];

    // ReSharper disable once CollectionNeverUpdated.Global
    public List<FlightFromProvider> Provider2Flights { get; set; } = [];
}