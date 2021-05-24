namespace getAirportsFlightDistance.Services
{
    using System;
    using System.Threading.Tasks;
    using Geolocation;
    using getAirportsFlightDistance.Services.Models;

    public class AirportFlightDistanceService : IAirportFlightDistanceService
    {
        // Для примера кэширования, здесь должен быть IAirportDescriptionService.
        private readonly ICachedAirportDescriptionService airportDescriptionService;

        public AirportFlightDistanceService(ICachedAirportDescriptionService airportDescriptionService)
        {
            this.airportDescriptionService =
                airportDescriptionService ?? throw new ArgumentNullException(nameof(airportDescriptionService));
        }

        public async Task<GetAirportFlightDistanceInMilesResult> GetDistanceInMiles(GetAirportFlightDistanceInMilesParameters parameters)
        {
            AirportDescription departureAirportDescription = await airportDescriptionService.GetDescription(
                iata: parameters.DepartureAirportIataCode
            );

            if (departureAirportDescription == null)
            {
                return new() {DepartureAirportNotFound = true};
            }
            
            AirportDescription arrivalAirportDescription = await airportDescriptionService.GetDescription(
                iata: parameters.ArrivalAirportIataCode
            );

            if (arrivalAirportDescription == null)
            {
                return new() {ArrivalAirportNotFound = true};
            }

            double distanceInMiles = GeoCalculator.GetDistance(
                originLatitude: departureAirportDescription.Location.Latitude,
                originLongitude: departureAirportDescription.Location.Longitude,
                destinationLatitude: arrivalAirportDescription.Location.Latitude,
                destinationLongitude: arrivalAirportDescription.Location.Longitude,
                decimalPlaces: 1,
                distanceUnit: DistanceUnit.Miles
            );

            return new()
            {
                Success = true,
                DistanceInMiles = distanceInMiles
            };
        }
    }
}