namespace getAirportsFlightDistance.Services.CTeleport.Models
{
    using System.Text.Json.Serialization;

    public class CTeleportAirportDescriptionDto
    {
        [JsonPropertyName("iata")]
        public string IataCode { get; set; }

        [JsonPropertyName("location")]
        public CTeleportLocationDto Location { get; set; }
    }
}