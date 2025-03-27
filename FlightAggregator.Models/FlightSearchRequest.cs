using System;
using System.ComponentModel.DataAnnotations;

namespace FlightAggregator.Models;

public class FlightSearchRequest
{
    [Required] public string Departure { get; set; } = string.Empty;
    [Required] public string Destination { get; set; } = string.Empty;
    [Required] public DateTime? Date { get; set; }
    public int? MaxStops { get; set; }
    public decimal? MaxPrice { get; set; }
    public string? Airline { get; set; }
    public string? SortBy { get; set; }
}