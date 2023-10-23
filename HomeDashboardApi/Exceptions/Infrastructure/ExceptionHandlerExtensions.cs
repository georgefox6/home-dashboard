using HomeDashboardApi.Exceptions.TemperatureNotFound;
using HomeDashboardApi.Exceptions.NoBinSchedulesFound;

namespace HomeDashboardApi.Exceptions.Infrastructure
{
    /// <summary>
    ///     Defines a collection of extensions for exception handling.
    /// </summary>
    public static class ExceptionHandlerExtensions
    {
        /// <summary>
        ///     Adds application exception handlers to the service collection.
        /// </summary>
        /// <param name="services">The service collection.</param>
        /// <returns>The configured service collection.</returns>
        public static IServiceCollection AddExceptionHandlers( this IServiceCollection services )
        {
            services
                .AddDefaultHttpContextExceptionHandlers()
                // Books
                .AddHttpContextExceptionHandler<TemperatureNotFoundException, TemperatureNotFoundExceptionHandler>()
                .AddHttpContextExceptionHandler<NoBinSchedulesFoundException, NoBinSchedulesFoundExceptionHandler>();                

            return services;
        }
    }
}