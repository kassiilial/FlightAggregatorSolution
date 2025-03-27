using FlightAggregator.Models.ProviderModels;
using FlightAggregator.Providers.Interfaces;
using FlightAggregator.Services;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Moq;

namespace FlightAggregator.Tests
{
    public class FlightAggregatorTests
    {
        [Fact]
        public async Task SearchFlightsAsync_ReturnsAggregatedFlights()
        {
            // Arrange
            var mockProvider1 = new Mock<IFlightProvider>();
            var mockProvider2 = new Mock<IFlightProvider>();

            mockProvider1.SetupGet(p => p.ProviderName).Returns("Provider1");
            mockProvider2.SetupGet(p => p.ProviderName).Returns("Provider2");

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

            var mockCache = new Mock<IDistributedCache>();

            mockCache
                .Setup(c => c.GetAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((byte[])null);

            var service = new FlightAggregatorService(providers, logger, mockCache.Object);

            // Act
            var result = await service.SearchFlightsAsync(
                "Moscow", "Paris", DateTime.Today,
                maxStops: null, maxPrice: null, airline: null, sortBy: null,
                CancellationToken.None);

            // Assert
            Assert.Equal(2, result.Count);
            Assert.Contains(result, f => f.Id == "FP1-001");
            Assert.Contains(result, f => f.Id == "FP2-101");
        }

        [Fact]
        public async Task BookFlightAsync_CallsCorrectProvider()
        {
            // Arrange
            var mockProvider1 = new Mock<IFlightProvider>();
            var mockProvider2 = new Mock<IFlightProvider>();

            mockProvider1.SetupGet(p => p.ProviderName).Returns("Provider1");
            mockProvider2.SetupGet(p => p.ProviderName).Returns("Provider2");

            var bookingRequest = new BookingRequest("FP1-001", "Provider1", "John Doe", "john@example.com");

            mockProvider1
                .Setup(p => p.BookFlightAsync(bookingRequest, It.IsAny<CancellationToken>()))
                .ReturnsAsync(true)
                .Verifiable();

            mockProvider2
                .Setup(p => p.BookFlightAsync(It.IsAny<BookingRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            var providers = new List<IFlightProvider> { mockProvider1.Object, mockProvider2.Object };
            var logger = Mock.Of<ILogger<FlightAggregatorService>>();

            var mockCache = new Mock<IDistributedCache>();

            mockCache
                .Setup(c => c.GetAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((byte[])null);

            var aggregatorService = new FlightAggregatorService(providers, logger, mockCache.Object);

            // Act
            var result = await aggregatorService.BookFlightAsync(bookingRequest, CancellationToken.None);

            // Assert
            Assert.True(result);
            mockProvider1.Verify(p => p.BookFlightAsync(bookingRequest, It.IsAny<CancellationToken>()), Times.Once);
            mockProvider2.Verify(p => p.BookFlightAsync(It.IsAny<BookingRequest>(), It.IsAny<CancellationToken>()), Times.Never);
        }
    }
}
