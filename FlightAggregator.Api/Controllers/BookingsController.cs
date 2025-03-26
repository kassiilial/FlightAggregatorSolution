using Microsoft.AspNetCore.Mvc;
using FlightAggregator.Business;
using FlightAggregator.Models.Entities;

namespace FlightAggregator.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BookingsController : ControllerBase
    {
        private readonly IFlightAggregatorService _aggregatorService;
        private readonly ILogger<BookingsController> _logger;

        public BookingsController(IFlightAggregatorService aggregatorService, ILogger<BookingsController> logger)
        {
            _aggregatorService = aggregatorService;
            _logger = logger;
        }

        [HttpPost]
        public async Task<IActionResult> BookFlight([FromBody] BookingRequest bookingRequest, CancellationToken cancellationToken)
        {
            var success = await _aggregatorService.BookFlightAsync(bookingRequest, cancellationToken);
            if (success)
            {
                _logger.LogInformation("Бронирование успешно для рейса {FlightId}", bookingRequest.FlightId);
                return Ok(new { Message = "Бронирование подтверждено" });
            }
            else
            {
                _logger.LogWarning("Ошибка бронирования для рейса {FlightId}", bookingRequest.FlightId);
                return BadRequest(new { Message = "Ошибка бронирования" });
            }
        }
    }
}
