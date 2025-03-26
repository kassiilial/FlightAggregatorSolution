using FlightAggregator.Models.ProviderModels;
using FlightAggregator.Providers.Interfaces;
using FlightAggregator.Services;
using Microsoft.Extensions.Logging;
using Moq;

namespace FlightAggregator.Tests;

public class FlightAggregatorTests
{
    [Fact]
    public async Task SearchFlightsAsync_ReturnsAggregatedFlights()
    {
        // Arrange
        var mockProvider1 = new Mock<IFlightProvider>();
        var mockProvider2 = new Mock<IFlightProvider>();

        var flights1 = new List<Flight>
        {
            new("FP1-001", "Moscow", "Paris", DateTime.Today, 150, 0, "AirProvider1", "Provider1")
        };
        var flights2 = new List<Flight>
        {
            new("FP2-101", "Moscow", "Paris", DateTime.Today, 180, 0, "AirProvider2", "Provider2")
        };

        mockProvider1.Setup(p => p.GetFlightsAsync("Moscow", "Paris", DateTime.Today, It.IsAny<CancellationToken>()))
            .ReturnsAsync(flights1);
        mockProvider2.Setup(p => p.GetFlightsAsync("Moscow", "Paris", DateTime.Today, It.IsAny<CancellationToken>()))
            .ReturnsAsync(flights2);

        var providers = new List<IFlightProvider> { mockProvider1.Object, mockProvider2.Object };
        var logger = Mock.Of<ILogger<FlightAggregatorService>>();
        var service = new FlightAggregatorService(providers, logger);

        // Act
        var result = await service.SearchFlightsAsync("Moscow", "Paris", DateTime.Today, CancellationToken.None);

        // Assert
        Assert.Equal(2, result.Count);
        Assert.Contains(result, f => f.Id == "FP1-001");
        Assert.Contains(result, f => f.Id == "FP2-101");
    }
}