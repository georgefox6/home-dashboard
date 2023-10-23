namespace HomeDashboardApi.Exceptions.Infrastructure
{
    /// <summary>
    /// Defines a collection of extensions for <see cref="IHttpContextExceptionHandler{TException}"/> objects.
    /// </summary>
    public static class HttpContextExceptionHandlerExtensions
    {
        /// <summary>
        /// Adds a <see cref="HttpContext"/> exception handler to the service collection.
        /// </summary>
        /// <typeparam name="TException">The type of exception handled.</typeparam>
        /// <typeparam name="THandler">The type of exception handler.</typeparam>
        /// <param name="serviceCollection">The service collection.</param>
        /// <returns>The configured service collection.</returns>
        public static IServiceCollection AddHttpContextExceptionHandler<TException, THandler>(
            this IServiceCollection serviceCollection )
            where TException : Exception
            where THandler : class, IHttpContextExceptionHandler<TException>
        {
            serviceCollection.AddTransient<IHttpContextExceptionHandler<TException>, THandler>();
            return serviceCollection;
        }

        /// <summary>
        /// Adds the default common <see cref="HttpContext"/> exception handlers to the service collection.
        /// </summary>
        /// <param name="serviceCollection">The service collection.</param>
        /// <returns>The configured service collection.</returns>
        public static IServiceCollection AddDefaultHttpContextExceptionHandlers(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddHttpContextExceptionHandler<Exception, DefaultExceptionHandler>();
            return serviceCollection;
        }

        /// <summary>
        /// Adds the <see cref="IHttpContextExceptionHandler{TException}"/> middleware to the application builder.
        /// </summary>
        /// <param name="builder">The application builder.</param>
        /// <returns>The configured application builder.</returns>
        public static IApplicationBuilder UseHttpContextExceptionHandling(this IApplicationBuilder builder)
        {
            builder.UseMiddleware<HttpContextExceptionsMiddleware>();
            return builder;
        }
    }
}