namespace getAirportsFlightDistance.Services.Models
{
    public class GetAirportFlightDistanceInMilesResult
    {
        public bool Success { get; set; }

        public bool DepartureAirportNotFound { get; set; }

        public bool ArrivalAirportNotFound { get; set; }

        public double DistanceInMiles { get; set; }
    }
}