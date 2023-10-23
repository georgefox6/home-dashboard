using System.Net;

namespace HomeDashboardApi.Exceptions.Infrastructure
{
    /// <summary>
    /// Defines a default exception handler for exceptions thrown which are not explicitly handled.
    /// </summary>
    public class DefaultExceptionHandler : IHttpContextExceptionHandler<Exception>
    {
        /// <summary>
        /// Handles the specified exception for the given context.
        /// </summary>
        /// <param name="context">The request context.</param>
        /// <param name="exception">The exception thrown.</param>
        /// <returns>An asynchronous operation.</returns>
        public async Task HandleAsync( HttpContext context, Exception exception )
        {
            var response = new ExceptionResponse<Exception>( "UnhandledException", "An unhandled error occurred.", exception );
            await context.Response.WriteJsonAsync( HttpStatusCode.InternalServerError, response );
        }
    }
}