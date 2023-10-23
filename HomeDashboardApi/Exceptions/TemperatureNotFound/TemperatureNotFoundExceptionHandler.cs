using System.Net;
using HomeDashboardApi.Exceptions.Infrastructure;

namespace HomeDashboardApi.Exceptions.TemperatureNotFound
{
    public class TemperatureNotFoundExceptionHandler : IHttpContextExceptionHandler<TemperatureNotFoundException>
    {
        public async Task HandleAsync( HttpContext context, TemperatureNotFoundException exception )
        {
            var response = new TemperatureNotFoundResponse( exception );

            await context.Response.WriteJsonAsync(HttpStatusCode.NotFound, response);
        }
    }
}