namespace getAirportsFlightDistance.Tests
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.Net;
    using System.Net.Http;
    using System.Threading;
    using System.Threading.Tasks;
    using getAirportsFlightDistance.Services.CTeleport;
    using getAirportsFlightDistance.Services.Models;
    using Moq;
    using Moq.Protected;
    using Xunit;

    public class CTeleportAirportDescriptionServiceUnitTests
    {
        private readonly CTeleportAirportDescriptionService unit;
        private readonly Mock<IHttpClientFactory> httpClientFactoryMock;

        public CTeleportAirportDescriptionServiceUnitTests()
        {
            httpClientFactoryMock = new Mock<IHttpClientFactory>();

            unit = new CTeleportAirportDescriptionService(
                new CTeleportAirportDescriptionServiceConfiguration
                {
                    Uri = new Uri("http://localhost")
                },
                new CTeleportAirportDescriptionMapper(),
                httpClientFactoryMock.Object
            );
        }

        [Fact]
        public async Task GetDescription_ShouldReturnAirportDescription()
        {
            // Arrange.
            SetupHttpClient(
                HttpStatusCode.OK,
                @"{""country"":""Netherlands"",""city_iata"":""AMS"",""iata"":""AMS"",""city"":""Amsterdam"",""timezone_region_name"":""Europe/Amsterdam"",""country_iata"":""NL"",""rating"":3,""name"":""Amsterdam"",""location"":{""lon"":4.763385,""lat"":52.309069},""type"":""airport"",""hubs"":7}"
            );
            
            // Act.
            AirportDescription airportDescription = await unit.GetDescription("AMS");

            // Assert.
            Assert.NotNull(@object: airportDescription);
            Assert.Equal(expected: "AMS", actual: airportDescription.IataCode);
            Assert.NotNull(@object: airportDescription.Location);
            Assert.Equal(expected: 4.763385, actual: airportDescription.Location.Longitude, precision: 6);
            Assert.Equal(expected: 52.309069, actual: airportDescription.Location.Latitude, precision: 6);
        }
        
        [Fact]
        public async Task GetDescription_ShouldThrowsValidationException_WhenResponseJsonHasNotIataCodeField()
        {
            // Arrange.
            SetupHttpClient(
                HttpStatusCode.OK,
                @"{""country"":""Netherlands"",""city_iata"":""AMS"",""city"":""Amsterdam"",""timezone_region_name"":""Europe/Amsterdam"",""country_iata"":""NL"",""rating"":3,""name"":""Amsterdam"",""location"":{""lon"":4.763385,""lat"":52.309069},""type"":""airport"",""hubs"":7}"
            );
            
            // Act + assert.
            await Assert.ThrowsAsync<ValidationException>(
                async () => { _ = await unit.GetDescription("AMS"); }
            );
        }
        
        [Fact]
        public async Task GetDescription_ShouldThrowsValidationException_WhenResponseJsonHasNotLocationField()
        {
            // Arrange.
            SetupHttpClient(
                HttpStatusCode.OK,
                @"{""country"":""Netherlands"",""city_iata"":""AMS"",""iata"":""AMS"",""city"":""Amsterdam"",""timezone_region_name"":""Europe/Amsterdam"",""country_iata"":""NL"",""rating"":3,""name"":""Amsterdam"",""type"":""airport"",""hubs"":7}"
            );
            
            // Act + assert.
            await Assert.ThrowsAsync<ValidationException>(
                async () => { _ = await unit.GetDescription("AMS"); }
            );
        }
        
        [Fact]
        public async Task GetDescription_ShouldReturnNull_WhenHttpResponseStatusNotFound()
        {
            // Arrange.
            SetupHttpClient(HttpStatusCode.NotFound, string.Empty);
            
            // Act.
            AirportDescription airportDescription = await unit.GetDescription("AMS");

            // Assert.
            Assert.Null(@object: airportDescription);
        }
        
        [Fact]
        public async Task GetDescription_ShouldThrowException_WhenUnexpectedHttpResponseStatus()
        {
            // Arrange.
            SetupHttpClient(HttpStatusCode.BadRequest, string.Empty);
            
            // Act + assert.
            await Assert.ThrowsAsync<ApplicationException>(
                async () => { _ = await unit.GetDescription("AMS"); }
            );
        }

        private void SetupHttpClient(HttpStatusCode responseStatusCode, string responseBody)
        {
            var httpMessageHandlerMock = new Mock<HttpMessageHandler>();

            var httpClient = new HttpClient(handler: httpMessageHandlerMock.Object, disposeHandler: false);

            httpClientFactoryMock
                .Setup(x => x.CreateClient(It.IsAny<string>()))
                .Returns(httpClient);

            httpMessageHandlerMock
                .Protected()
                .As<IHttpMessageHandler>()
                .Setup(x => x.SendAsync(It.IsAny<HttpRequestMessage>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(
                    new HttpResponseMessage
                    {
                        StatusCode = responseStatusCode,
                        Content = new StringContent(responseBody)
                    }
                );
        }

        // Для возможности сделать мок для protected методов абстрактного класса.
        interface IHttpMessageHandler
        {
            Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken);
        }
    }
}