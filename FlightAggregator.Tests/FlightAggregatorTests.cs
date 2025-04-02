using FlightAggregator.Models;
using FlightAggregator.Models.ProviderModels;
using FlightAggregator.Providers.Interfaces;
using FlightAggregator.Services;
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
                new()
                {
                    FlightNumber = "FP1-001",
                    Departure = "Moscow",
                    Destination = "Paris",
                    Date = DateTime.Today,
                    Price = 150m,
                    Stops = 0,
                    Airline = "AirProvider1",
                    Provider = "Provider1"
                }
            };

            var flights2 = new List<Flight>
            {
                new()
                {
                    FlightNumber = "FP2-101",
                    Departure = "Moscow",
                    Destination = "Paris",
                    Date = DateTime.Today,
                    Price = 180m,
                    Stops = 0,
                    Airline = "AirProvider2",
                    Provider = "Provider2"
                }
            };

            mockProvider1
                .Setup(p => p.GetFlightsAsync("Moscow", "Paris", DateTime.Today, It.IsAny<CancellationToken>()))
                .Returns((string departure, string destination, DateTime date, CancellationToken token) => flights1.ToAsyncEnumerable());
            mockProvider2
                .Setup(p => p.GetFlightsAsync("Moscow", "Paris", DateTime.Today, It.IsAny<CancellationToken>()))
                .Returns((string departure, string destination, DateTime date, CancellationToken token) => flights2.ToAsyncEnumerable());


            var providers = new List<IFlightProvider> { mockProvider1.Object, mockProvider2.Object };
            var logger = Mock.Of<ILogger<FlightAggregatorService>>();
            
            var service = new FlightAggregatorService(providers, logger);

            // Act
            var result = new List<Flight>();
            await foreach (var flight in service.SearchFlightsAsync(
                               "Moscow", "Paris", DateTime.Today,
                               maxStops: null, maxPrice: null, airline: null, sortBy: null,
                               CancellationToken.None))
            {
                result.Add(flight);
            }


            // Assert
            Assert.Equal(2, result.Count);
            Assert.Contains(result, f => f.FlightNumber == "FP1-001");
            Assert.Contains(result, f => f.FlightNumber == "FP2-101");
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
            
            var aggregatorService = new FlightAggregatorService(providers, logger);

            // Act
            var result = await aggregatorService.BookFlightAsync(bookingRequest, CancellationToken.None);

            // Assert
            Assert.True(result);
            mockProvider1.Verify(p => p.BookFlightAsync(bookingRequest, It.IsAny<CancellationToken>()), Times.Once);
            mockProvider2.Verify(p => p.BookFlightAsync(It.IsAny<BookingRequest>(), It.IsAny<CancellationToken>()),
                Times.Never);
        }
    }
}