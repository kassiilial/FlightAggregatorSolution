namespace FlightAggregator.Models.ProviderModels;

public record BookingRequest(
    string FlightId,
    string Provider,
    string PassengerName,
    string PassengerEmail
);