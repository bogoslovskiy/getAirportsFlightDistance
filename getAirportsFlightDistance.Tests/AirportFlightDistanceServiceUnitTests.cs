namespace getAirportsFlightDistance.Tests
{
    using System;
    using System.Threading.Tasks;
    using Geolocation;
    using getAirportsFlightDistance.Services;
    using getAirportsFlightDistance.Services.Models;
    using Moq;
    using Xunit;

    public class AirportFlightDistanceServiceUnitTests
    {
        private readonly AirportFlightDistanceService unit;
        // Для примера кэширования, здесь должен быть IAirportDescriptionService.
        private readonly Mock<ICachedAirportDescriptionService> airportDescriptionServiceMock;

        public AirportFlightDistanceServiceUnitTests()
        {
            airportDescriptionServiceMock = new Mock<ICachedAirportDescriptionService>();

            unit = new AirportFlightDistanceService(airportDescriptionServiceMock.Object);
        }

        [Fact]
        public async Task GetDistanceInMiles_ShouldReturnProperResult()
        {
            // Arrange.
            
            double originLatitude = 45;
            double originLongitude = 46;
            double destinationLatitude = 47;
            double destinationLongitude = 48;
            
            SetupAirportDescriptionService(
                ("MRV", new() {IataCode = "MRV", Location = new() {Latitude = originLatitude, Longitude = originLongitude}}),
                ("SVO", new() {IataCode = "SVO", Location = new() {Latitude = destinationLatitude, Longitude = destinationLongitude}})
            );
            
            GetAirportFlightDistanceInMilesParameters parameters = new()
            {
                DepartureAirportIataCode = "SVO",
                ArrivalAirportIataCode = "MRV"
            };

            // Act.
            GetAirportFlightDistanceInMilesResult result = await unit.GetDistanceInMiles(parameters);
            
            // Assert.
            
            double expectedDistanceInMiles = GeoCalculator.GetDistance(
                originLatitude: originLatitude,
                originLongitude: originLongitude,
                destinationLatitude: destinationLatitude,
                destinationLongitude: destinationLongitude,
                decimalPlaces: 1,
                distanceUnit: DistanceUnit.Miles
            );
            
            Assert.NotNull(result);
            Assert.True(result.Success);
            Assert.Equal(expected: expectedDistanceInMiles, actual: result.DistanceInMiles, precision: 1);
        }
        
        [Fact]
        public async Task GetDistanceInMiles_ShouldReturnDepartureAirportNotFound_WhenDepartureAirportNotFoundByIataCode()
        {
            // Arrange.
            
            double originLatitude = 45;
            double originLongitude = 46;
            double destinationLatitude = 47;
            double destinationLongitude = 48;
            
            SetupAirportDescriptionService(
                ("MRV", new() {IataCode = "MRV", Location = new() {Latitude = originLatitude, Longitude = originLongitude}}),
                ("SVO", new() {IataCode = "SVO", Location = new() {Latitude = destinationLatitude, Longitude = destinationLongitude}})
            );
            
            GetAirportFlightDistanceInMilesParameters parameters = new()
            {
                DepartureAirportIataCode = "KRR",
                ArrivalAirportIataCode = "SVO"
            };

            // Act.
            GetAirportFlightDistanceInMilesResult result = await unit.GetDistanceInMiles(parameters);
            
            // Assert.
            
            Assert.NotNull(result);
            Assert.False(result.Success);
            Assert.True(result.DepartureAirportNotFound);
        }
        
        [Fact]
        public async Task GetDistanceInMiles_ShouldReturnArrivalAirportNotFound_WhenArrivalAirportNotFoundByIataCode()
        {
            // Arrange.
            
            double originLatitude = 45;
            double originLongitude = 46;
            double destinationLatitude = 47;
            double destinationLongitude = 48;
            
            SetupAirportDescriptionService(
                ("MRV", new() {IataCode = "MRV", Location = new() {Latitude = originLatitude, Longitude = originLongitude}}),
                ("SVO", new() {IataCode = "SVO", Location = new() {Latitude = destinationLatitude, Longitude = destinationLongitude}})
            );
            
            GetAirportFlightDistanceInMilesParameters parameters = new()
            {
                DepartureAirportIataCode = "MRV",
                ArrivalAirportIataCode = "KRR"
            };

            // Act.
            GetAirportFlightDistanceInMilesResult result = await unit.GetDistanceInMiles(parameters);
            
            // Assert.
            
            Assert.NotNull(result);
            Assert.False(result.Success);
            Assert.True(result.ArrivalAirportNotFound);
        }

        private void SetupAirportDescriptionService(params (string IataCode, AirportDescription Description)[] airportDescriptions)
        {
            foreach ((string IataCode, AirportDescription Description) airportDescription in airportDescriptions)
            {
                airportDescriptionServiceMock
                    .Setup(
                        x => x.GetDescription(
                            It.Is<string>(y => string.Equals(y, airportDescription.IataCode, StringComparison.OrdinalIgnoreCase))
                        )
                    )
                    .ReturnsAsync(airportDescription.Description);
            }
        }
    }
}