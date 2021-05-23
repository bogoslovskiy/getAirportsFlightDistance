namespace getAirportsFlightDistance
{
    using System;
    using System.Text.Json;
    using System.Text.Json.Serialization;
    using FluentValidation.AspNetCore;
    using getAirportsFlightDistance.Services;
    using getAirportsFlightDistance.Services.CTeleport;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    
    public class Startup
    {
        public IConfiguration Configuration { get; }
        
        public Startup(IConfiguration configuration)
        {
            this.Configuration = configuration;
        }
        
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers()
                .AddJsonOptions(
                    options =>
                    {
                        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.CamelCase));
                    }
                )
                .AddFluentValidation(
                    fv =>
                    {
                        fv.RegisterValidatorsFromAssemblyContaining<Startup>();
                    }
                );
            
            FluentValidation.ValidatorOptions.Global.PropertyNameResolver = FluentValidationJsonPropertyNameResolver.ResolvePropertyName;

            services.AddSingleton(GetCTeleportAirportDescriptionServiceConfiguration());
            
            services.AddSingleton<ICTeleportAirportDescriptionMapper, CTeleportAirportDescriptionMapper>();
            
            services.AddSingleton<IAirportDescriptionService, CTeleportAirportDescriptionService>();

            services.AddSingleton<IAirportFlightDistanceService, AirportFlightDistanceService>();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }

        private CTeleportAirportDescriptionServiceConfiguration GetCTeleportAirportDescriptionServiceConfiguration()
        {
            const string uriParamName = "CTeleport:AirportDescriptionServiceUri";
            
            if (!Uri.TryCreate(this.Configuration[uriParamName], UriKind.Absolute, out Uri uri))
            {
                throw new ApplicationException($"Missing or invalid required '{uriParamName}'");
            }

            return new CTeleportAirportDescriptionServiceConfiguration
            {
                Uri = uri
            };
        }
    }
}