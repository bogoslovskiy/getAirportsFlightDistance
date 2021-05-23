namespace getAirportsFlightDistance.Services.CTeleport
{
    using getAirportsFlightDistance.Services.CTeleport.Models;
    using getAirportsFlightDistance.Services.Models;

    public class CTeleportAirportDescriptionMapper : ICTeleportAirportDescriptionMapper
    {
        public AirportDescription Map(CTeleportAirportDescriptionDto airportDescriptionDto)
        {
            return new()
            {
                IataCode = airportDescriptionDto.IataCode,
                Location = new Location
                {
                    Latitude = airportDescriptionDto.Location.Latitude,
                    Longitude = airportDescriptionDto.Location.Longitude
                }
            };
        }
    }
}