namespace HomeDashboardApi.Exceptions.TemperatureNotFound
{
    public class TemperatureNotFoundException : Exception
    {
        public TemperatureNotFoundException() : base( "A temeprature for that time period could not be found." ) { }
    }
}