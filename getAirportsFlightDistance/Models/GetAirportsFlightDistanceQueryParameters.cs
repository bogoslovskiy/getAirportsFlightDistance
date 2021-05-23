namespace getAirportsFlightDistance.Models
{
    using System.Text.Json.Serialization;
    using Microsoft.AspNetCore.Mvc;

    public class GetAirportsFlightDistanceQueryParameters
    {
        [JsonPropertyName("departureAirport")]
        [FromQuery(Name = "departureAirport")]
        public string DepartureAirportIataCode { get; set; }

        [JsonPropertyName("arrivalAirport")]
        [FromQuery(Name = "arrivalAirport")]
        public string ArrivalAirportIataCode { get; set; }
    }
}