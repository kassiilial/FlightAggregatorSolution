using System;

namespace FlightAggregator.Models.Entities;

public record BookingRequest(
        string FlightId,
        string PassengerName,
        string PassengerEmail
    );
