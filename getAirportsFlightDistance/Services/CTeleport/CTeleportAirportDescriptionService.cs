namespace getAirportsFlightDistance.Services.CTeleport
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.IO;
    using System.Net;
    using System.Net.Http;
    using System.Text.Json;
    using System.Threading.Tasks;
    using getAirportsFlightDistance.Services.CTeleport.Models;
    using getAirportsFlightDistance.Services.Models;

    public class CTeleportAirportDescriptionService : IAirportDescriptionService
    {
        private readonly CTeleportAirportDescriptionServiceConfiguration configuration;
        private readonly ICTeleportAirportDescriptionMapper mapper;
        private readonly IHttpClientFactory httpClientFactory;

        public CTeleportAirportDescriptionService(
            CTeleportAirportDescriptionServiceConfiguration configuration,
            ICTeleportAirportDescriptionMapper mapper,
            IHttpClientFactory httpClientFactory)
        {
            this.configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            this.mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            this.httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
        }

        public async Task<AirportDescription> GetDescription(string iata)
        {
            using HttpClient httpClient = httpClientFactory.CreateClient();

            HttpResponseMessage response = await httpClient.GetAsync(
                requestUri: new Uri(baseUri: configuration.Uri, relativeUri: iata.ToUpper())
            );

            if (response.StatusCode == HttpStatusCode.OK)
            {
                Stream airportDescriptionJsonStream = await response.Content.ReadAsStreamAsync();
                
                CTeleportAirportDescriptionDto airportDescriptionDto = 
                    await JsonSerializer.DeserializeAsync<CTeleportAirportDescriptionDto>(airportDescriptionJsonStream);

                ValidateAirportDescriptionDto(airportDescriptionDto);
                
                return mapper.Map(airportDescriptionDto);
            }

            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                return null;
            }

            throw new ApplicationException(
                $"Unexpected GET airport description response from CTeleport API with status code: {response.StatusCode}"
            );
        }

        private void ValidateAirportDescriptionDto(CTeleportAirportDescriptionDto airportDescriptionDto)
        {
            if (string.IsNullOrWhiteSpace(airportDescriptionDto.IataCode))
            {
                throw new ValidationException("CTeleport airport IATA code is not provided");
            }

            if (airportDescriptionDto.Location == null)
            {
                throw new ValidationException("CTeleport airport location is not provided");
            }
        }
    }
}