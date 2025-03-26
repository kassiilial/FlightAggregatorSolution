using System;
using System.Collections.Generic;
using System.Text;

namespace FlightAggregator.Models.Entities
{
    public record Flight(
         string Id,
         string Departure,
         string Destination,
         DateTime Date,
         decimal Price,
         int Stops,
         string Airline,
         string Provider  // источник данных
     );
}
