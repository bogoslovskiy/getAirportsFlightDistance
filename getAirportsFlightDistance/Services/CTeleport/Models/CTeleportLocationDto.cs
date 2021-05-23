namespace getAirportsFlightDistance.Services.CTeleport.Models
{
    using System.Text.Json.Serialization;

    public class CTeleportLocationDto
    {
        [JsonPropertyName("lon")]
        public double Longitude { get; set; }
        
        [JsonPropertyName("lat")]
        public double Latitude { get; set; }
    }
}