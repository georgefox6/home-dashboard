using System.Net;
using System.Reflection;

namespace HomeDashboardApi.Exceptions.Infrastructure
{
    /// <summary>
    /// Defines a middleware for handling JSON exceptions.
    /// </summary>
    public class HttpContextExceptionsMiddleware
    {
        private static readonly Type ExceptionHandlerInterfaceType;

        private readonly IHttpContextExceptionHandler<Exception> defaultExceptionHandler;
        private readonly IHostEnvironment hostEnvironment;
        private readonly RequestDelegate httpRequestDelegate;
        private readonly IServiceProvider serviceProvider;

        static HttpContextExceptionsMiddleware()
        {
            ExceptionHandlerInterfaceType = typeof( IHttpContextExceptionHandler<> );
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HttpContextExceptionsMiddleware" /> class.
        /// </summary>
        /// <param name="httpRequestDelegate">The request delegate for processing a HTTP request.</param>
        /// <param name="hostEnvironment">The host environment.</param>
        /// <param name="serviceProvider">The service provider.</param>
        /// <param name="defaultExceptionHandler">The default exception handler.</param>
        public HttpContextExceptionsMiddleware(
            RequestDelegate httpRequestDelegate,
            IHostEnvironment hostEnvironment,
            IServiceProvider serviceProvider,
            IHttpContextExceptionHandler<Exception> defaultExceptionHandler )
        {
            this.httpRequestDelegate = httpRequestDelegate;
            this.hostEnvironment = hostEnvironment;
            this.serviceProvider = serviceProvider;
            this.defaultExceptionHandler = defaultExceptionHandler;
        }

        /// <summary>
        /// Invokes the middleware for performing requests on the <paramref name="context"/> with exception handling.
        /// </summary>
        /// <param name="context">The current <see cref="HttpContext"/> for processing requests.</param>
        public async Task Invoke( HttpContext context )
        {
            try
            {
                await httpRequestDelegate( context );
            }
            catch ( AggregateException exception )
            {
                var innerExceptions = exception.InnerExceptions.GroupBy( e => e.GetType() )
                    .Select( g => g.Last() )
                    .ToList();

                await HandleExceptionAsync( context, innerExceptions.Last() );
            }
            catch ( Exception exception )
            {
                await HandleExceptionAsync( context, exception );
            }
        }

        private async Task HandleExceptionAsync( HttpContext context, Exception exception )
        {
            if ( context.Response.HasStarted )
            {
                return;
            }

            context.Response.Clear();

            Type exceptionHandlerType = ExceptionHandlerInterfaceType.MakeGenericType( exception.GetType() );
            dynamic exceptionHandler;

            try
            {
                exceptionHandler = serviceProvider.GetService( exceptionHandlerType );
            }
            catch ( Exception )
            {
                await HandleWithDefaultHandlerAsync( context, exception );
                return;
            }

            if ( exceptionHandler == null )
            {
                await HandleWithDefaultHandlerAsync( context, exception );
                return;
            }

            MethodInfo handleMethod = exceptionHandlerType.GetTypeInfo().GetMethod( "HandleAsync" );

            try
            {
                if ( handleMethod is not null )
                {
                    await handleMethod.Invoke( exceptionHandler, new object[] { context, exception } );
                }
            }
            catch ( Exception handleException )
            {
                string exceptionName = handleException.GetType().FullName;
                string originalExceptionName = exception.GetType().FullName;

                if ( !hostEnvironment.IsProduction() )
                {
                    var response = new ExceptionResponse<Exception>(
                        "ExceptionHandlerThrewException",
                        $"Exception {exceptionName} thrown with message {handleException.Message} when handling exception {originalExceptionName} with message {exception.Message}",
                        handleException );

                    await context.Response.WriteJsonAsync( HttpStatusCode.InternalServerError, response );
                }
                else
                {
                    context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                }
            }
        }

        private async Task HandleWithDefaultHandlerAsync( HttpContext context, Exception exception )
        {
            string originalExceptionName = exception.GetType().FullName;

            try
            {
                await defaultExceptionHandler.HandleAsync( context, exception );
            }
            catch ( Exception handlerException )
            {
                string exceptionName = handlerException.GetType().FullName;

                if ( !hostEnvironment.IsProduction() )
                {
                    var response = new ExceptionResponse<Exception>(
                        "DefaultExceptionHandlerThrewException",
                        $"Exception {exceptionName} thrown with message {handlerException.Message} when handling exception {originalExceptionName} with message {exception.Message}",
                        handlerException );

                    await context.Response.WriteJsonAsync( HttpStatusCode.InternalServerError, response );
                }
                else
                {
                    context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                }
            }
        }
    }
}