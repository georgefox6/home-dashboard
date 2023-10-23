using HomeDashboardApi.Exceptions.Infrastructure;

namespace HomeDashboardApi.Exceptions.NoBinSchedulesFound
{
    public class NoBinSchedulesFoundResponse : ExceptionResponse<NoBinSchedulesFoundException>
    {
        public NoBinSchedulesFoundResponse(NoBinSchedulesFoundException exception ) : base("NoBinSchedulesFound", "No bin schedules exist after the current date.", exception ) { }
    }
}