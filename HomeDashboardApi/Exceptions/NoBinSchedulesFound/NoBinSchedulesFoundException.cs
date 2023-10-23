namespace HomeDashboardApi.Exceptions.NoBinSchedulesFound
{
    public class NoBinSchedulesFoundException : Exception
    {
        public NoBinSchedulesFoundException() : base("No bin schedules exist after the current date.") { }
    }
}