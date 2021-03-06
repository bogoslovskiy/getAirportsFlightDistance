namespace getAirportsFlightDistance.Models
{
    using FluentValidation;

    // ReSharper disable once UnusedType.Global
    public class GetAirportsFlightDistanceQueryParametersValidator : AbstractValidator<GetAirportsFlightDistanceQueryParameters>
    {
        public GetAirportsFlightDistanceQueryParametersValidator()
        {
            const string shouldBeThreeLetterIataCodeMessage = "Should be 3-letter IATA code";
            
            RuleFor(x => x.DepartureAirportIataCode)
                .NotNull()
                .Length(3)
                .WithMessage(shouldBeThreeLetterIataCodeMessage);
            
            RuleFor(x => x.ArrivalAirportIataCode)
                .NotNull()
                .Length(3)
                .WithMessage(shouldBeThreeLetterIataCodeMessage);
        }
    }
}