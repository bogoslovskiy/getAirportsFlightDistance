namespace getAirportsFlightDistance.Services
{
    using System.Threading.Tasks;
    using getAirportsFlightDistance.Services.Models;

    public interface IAirportFlightDistanceService
    {
        Task<GetAirportFlightDistanceInMilesResult> GetDistanceInMiles(GetAirportFlightDistanceInMilesParameters parameters);
    }
}