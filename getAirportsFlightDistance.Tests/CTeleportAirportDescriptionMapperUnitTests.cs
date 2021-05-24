namespace getAirportsFlightDistance.Tests
{
    using getAirportsFlightDistance.Services.CTeleport;
    using getAirportsFlightDistance.Services.CTeleport.Models;
    using getAirportsFlightDistance.Services.Models;
    using Xunit;
    
    public class CTeleportAirportDescriptionMapperUnitTests
    {
        private readonly CTeleportAirportDescriptionMapper unit;

        public CTeleportAirportDescriptionMapperUnitTests()
        {
            unit = new CTeleportAirportDescriptionMapper();
        }
        
        [Fact]
        public void Map_ShouldReturnAirportDescriptionWithProperFields()
        {
            // Arrange.
            CTeleportAirportDescriptionDto cTeleportAirportDescriptionDto = new()
            {
                IataCode = "KRR",
                Location = new CTeleportLocationDto
                {
                    Latitude = 45.12345,
                    Longitude = 44.54321
                }
            };

            // Act.
            AirportDescription airportDescription = unit.Map(cTeleportAirportDescriptionDto);
            
            // Assert.
            Assert.Equal(expected: cTeleportAirportDescriptionDto.IataCode, airportDescription.IataCode);
            Assert.NotNull(@object: airportDescription.Location);
            Assert.Equal(
                expected: cTeleportAirportDescriptionDto.Location.Latitude,
                actual: airportDescription.Location.Latitude,
                precision: 5
            );
            Assert.Equal(
                expected: cTeleportAirportDescriptionDto.Location.Longitude,
                actual: airportDescription.Location.Longitude,
                precision: 5
            );
        }
    }
}