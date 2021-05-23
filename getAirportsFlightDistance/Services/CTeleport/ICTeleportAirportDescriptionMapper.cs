namespace getAirportsFlightDistance.Services.CTeleport
{
    using getAirportsFlightDistance.Services.CTeleport.Models;
    using getAirportsFlightDistance.Services.Models;

    public interface ICTeleportAirportDescriptionMapper
    {
        AirportDescription Map(CTeleportAirportDescriptionDto airportDescriptionDto);
    }
}