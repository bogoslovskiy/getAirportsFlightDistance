namespace getAirportsFlightDistance.Services
{
    using System.Threading.Tasks;
    using getAirportsFlightDistance.Services.Models;

    public interface IAirportDescriptionService
    {
        Task<AirportDescription> GetDescription(string iata);
    }
}