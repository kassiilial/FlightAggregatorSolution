using System;
using System.Collections.Generic;
using FlightAggregator.Models.ProviderModels;

namespace FlightAggregator.Models.Configurations;

 public static class TestDataConfig
    {
        public static List<Flight> Provider1Flights => new List<Flight>
        {
            new Flight("FP1-001", "Moscow", "Paris", DateTime.Today, 150, 0, "AirProvider1", "Provider1"),
            new Flight("FP1-002", "Moscow", "Paris", DateTime.Today, 200, 1, "AirProvider1", "Provider1")
        };

        public static List<Flight> Provider2Flights => new List<Flight>
        {
            new Flight("FP2-101", "Moscow", "Paris", DateTime.Today, 180, 0, "AirProvider2", "Provider2"),
            new Flight("FP2-102", "Moscow", "Paris", DateTime.Today, 220, 2, "AirProvider2", "Provider2")
        };
    }
