using System.Net;
using HomeDashboardApi.Exceptions.Infrastructure;

namespace HomeDashboardApi.Exceptions.NoBinSchedulesFound
{
    public class NoBinSchedulesFoundExceptionHandler : IHttpContextExceptionHandler<NoBinSchedulesFoundException>
    {
        public async Task HandleAsync( HttpContext context, NoBinSchedulesFoundException exception )
        {
            var response = new NoBinSchedulesFoundResponse( exception );

            await context.Response.WriteJsonAsync(HttpStatusCode.NotFound, response);
        }
    }
}