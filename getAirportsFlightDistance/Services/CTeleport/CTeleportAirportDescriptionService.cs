namespace getAirportsFlightDistance.Services.CTeleport
{
    using System;
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

        public CTeleportAirportDescriptionService(
            CTeleportAirportDescriptionServiceConfiguration configuration,
            ICTeleportAirportDescriptionMapper mapper)
        {
            this.configuration = configuration;
            this.mapper = mapper;
        }

        public async Task<AirportDescription> GetDescription(string iata)
        {
            using HttpClient httpClient = new();

            HttpResponseMessage response = await httpClient.GetAsync(
                requestUri: new Uri(baseUri: configuration.Uri, relativeUri: iata.ToUpper())
            );

            if (response.StatusCode == HttpStatusCode.OK)
            {
                string airportDescriptionJson = await response.Content.ReadAsStringAsync();
                
                CTeleportAirportDescriptionDto airportDescriptionDto =
                    JsonSerializer.Deserialize<CTeleportAirportDescriptionDto>(airportDescriptionJson);

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
    }
}