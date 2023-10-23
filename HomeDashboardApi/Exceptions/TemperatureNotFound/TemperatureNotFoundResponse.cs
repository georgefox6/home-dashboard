using HomeDashboardApi.Exceptions.Infrastructure;

namespace HomeDashboardApi.Exceptions.TemperatureNotFound
{
    public class TemperatureNotFoundResponse : ExceptionResponse<TemperatureNotFoundException>
    {
        public TemperatureNotFoundResponse(TemperatureNotFoundException exception ) : base( "TemperatureNotFound", "There are no temperature records for that room in that time period.", exception ) { }
    }
}