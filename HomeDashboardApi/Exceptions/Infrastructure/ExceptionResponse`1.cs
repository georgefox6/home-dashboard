using Newtonsoft.Json;

namespace HomeDashboardApi.Exceptions.Infrastructure
{
    /// <summary>
    /// Defines a response to a an exception being thrown.
    /// </summary>
    /// <typeparam name="TException">The type of exception thrown.</typeparam>
    public class ExceptionResponse<TException>
        where TException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ExceptionResponse{TException}" /> class with an error code and message.
        /// </summary>
        /// <param name="errorCode">The error code.</param>
        /// <param name="errorMessage">The error message.</param>
        /// <param name="exception">The exception thrown.</param>
        public ExceptionResponse( string errorCode, string errorMessage, TException exception )
        {
            ErrorCode = errorCode;
            ErrorMessage = errorMessage;
            Exception = exception;
        }

        /// <summary>
        /// Gets the error code.
        /// </summary>
        public string ErrorCode { get; }

        /// <summary>
        /// Gets the error message.
        /// </summary>
        public string ErrorMessage { get; }
        
        /// <summary>
        /// Gets the exception thrown.
        /// </summary>
        [ JsonIgnore ]
        protected TException Exception { get; }
    }
}