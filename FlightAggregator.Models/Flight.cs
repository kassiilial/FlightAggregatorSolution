using System;

namespace FlightAggregator.Models;

public class Flight
{
    public string FlightNumber { get; set; } = string.Empty;
    public string Departure { get; set; } = string.Empty;
    public string Destination { get; set; } = string.Empty;
    public DateTime Date { get; set; }
    public decimal Price { get; set; }
    public int Stops { get; set; }
    public string Airline { get; set; } = string.Empty;
    public string Provider { get; set; } = string.Empty;
}