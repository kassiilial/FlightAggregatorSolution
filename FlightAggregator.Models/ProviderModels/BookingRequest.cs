namespace FlightAggregator.Models.ProviderModels;

public record BookingRequest(
    string FlightId,
    string PassengerName,
    string PassengerEmail
);