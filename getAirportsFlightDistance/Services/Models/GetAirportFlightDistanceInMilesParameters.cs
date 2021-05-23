namespace getAirportsFlightDistance.Services.Models
{
    public class GetAirportFlightDistanceInMilesParameters
    {
        public string DepartureAirportIataCode { get; set; }
        
        public string ArrivalAirportIataCode { get; set; }
    }
}