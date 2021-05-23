namespace getAirportsFlightDistance.Controllers
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.Threading.Tasks;
    using getAirportsFlightDistance.Models;
    using getAirportsFlightDistance.Services;
    using getAirportsFlightDistance.Services.Models;
    using Microsoft.AspNetCore.Mvc;

    [ApiController]
    [Route("airportsFlightDistance")]
    public class GetAirportsFlightDistanceController : ControllerBase
    {
        private readonly IAirportFlightDistanceService airportFlightDistanceService;

        public GetAirportsFlightDistanceController(IAirportFlightDistanceService airportFlightDistanceService)
        {
            this.airportFlightDistanceService = airportFlightDistanceService;
        }

        [HttpGet]
        public async Task<IActionResult> Get([FromQuery, Required] GetAirportsFlightDistanceQueryParameters queryParameters)
        {
            GetAirportFlightDistanceInMilesResult getDistanceImMilesResult = await airportFlightDistanceService.GetDistanceInMiles(
                new GetAirportFlightDistanceInMilesParameters
                {
                    DepartureAirportIataCode = queryParameters.DepartureAirportIataCode,
                    ArrivalAirportIataCode = queryParameters.ArrivalAirportIataCode
                }
            );

            if (getDistanceImMilesResult.Success)
            {
                return Ok(getDistanceImMilesResult.DistanceInMiles);
            }

            if (getDistanceImMilesResult.DepartureAirportNotFound || getDistanceImMilesResult.ArrivalAirportNotFound)
            {
                return NotFound();
            }

            throw new ApplicationException($"Unexpected {nameof(airportFlightDistanceService.GetDistanceInMiles)} result");
        }
    }
}