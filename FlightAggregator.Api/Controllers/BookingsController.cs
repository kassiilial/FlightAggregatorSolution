using FlightAggregator.Models.ProviderModels;
using Microsoft.AspNetCore.Mvc;
using FlightAggregator.Services;

namespace FlightAggregator.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BookingsController(IFlightAggregatorService aggregatorService, ILogger<BookingsController> logger)
        : ControllerBase
    {
        [HttpPost]
        public async Task<IActionResult> BookFlight([FromBody] BookingRequest bookingRequest,
            CancellationToken cancellationToken)
        {
            var success = await aggregatorService.BookFlightAsync(bookingRequest, cancellationToken);
            if (success)
            {
                logger.LogInformation("Бронирование успешно для рейса {FlightId}", bookingRequest.FlightId);
                return Ok(new { Message = "Бронирование подтверждено" });
            }
            else
            {
                logger.LogWarning("Ошибка бронирования для рейса {FlightId}", bookingRequest.FlightId);
                return BadRequest(new { Message = "Ошибка бронирования" });
            }
        }
    }
}