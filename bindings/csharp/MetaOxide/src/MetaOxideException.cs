using System;

namespace MetaOxide
{
    /// <summary>
    /// Exception thrown when MetaOxide operations fail.
    /// </summary>
    /// <remarks>
    /// This exception wraps errors from the native MetaOxide library,
    /// providing detailed error information including error codes and messages.
    /// </remarks>
    public class MetaOxideException : Exception
    {
        /// <summary>
        /// Gets the native error code from the MetaOxide library.
        /// </summary>
        public int ErrorCode { get; }

        /// <summary>
        /// Gets a description of what caused the error.
        /// </summary>
        public string ErrorDescription { get; }

        /// <summary>
        /// Initializes a new instance of the MetaOxideException class.
        /// </summary>
        /// <param name="errorCode">The native error code</param>
        /// <param name="message">The error message</param>
        public MetaOxideException(int errorCode, string message)
            : base($"MetaOxide error {errorCode}: {message}")
        {
            ErrorCode = errorCode;
            ErrorDescription = message;
        }

        /// <summary>
        /// Initializes a new instance of the MetaOxideException class with a custom message.
        /// </summary>
        /// <param name="message">The error message</param>
        public MetaOxideException(string message)
            : base(message)
        {
            ErrorCode = -1;
            ErrorDescription = message;
        }

        /// <summary>
        /// Initializes a new instance of the MetaOxideException class with a custom message and inner exception.
        /// </summary>
        /// <param name="message">The error message</param>
        /// <param name="innerException">The inner exception</param>
        public MetaOxideException(string message, Exception innerException)
            : base(message, innerException)
        {
            ErrorCode = -1;
            ErrorDescription = message;
        }

        /// <summary>
        /// Creates a MetaOxideException from the current thread-local error state.
        /// </summary>
        /// <returns>A new MetaOxideException with error details from the native library</returns>
        internal static MetaOxideException FromLastError()
        {
            int errorCode = MetaOxideInterop.meta_oxide_last_error();
            IntPtr messagePtr = MetaOxideInterop.meta_oxide_error_message();

            try
            {
                string message = MetaOxideInterop.PtrToStringUtf8(messagePtr)
                    ?? "Unknown error occurred";
                return new MetaOxideException(errorCode, message);
            }
            finally
            {
                if (messagePtr != IntPtr.Zero)
                {
                    MetaOxideInterop.meta_oxide_string_free(messagePtr);
                }
            }
        }

        /// <summary>
        /// Gets a user-friendly error message based on the error code.
        /// </summary>
        /// <returns>A descriptive error message</returns>
        public string GetFriendlyMessage()
        {
            return ErrorCode switch
            {
                0 => "No error",
                1 => "Failed to parse HTML content. The HTML may be malformed or invalid.",
                2 => "Invalid URL format. Please provide a valid absolute or relative URL.",
                3 => "Invalid UTF-8 encoding. The input contains invalid character sequences.",
                4 => "Memory allocation failed. The system may be out of memory.",
                5 => "Failed to serialize data to JSON. The metadata structure may be too complex.",
                6 => "A required parameter was null. Please ensure all required parameters are provided.",
                _ => $"Unknown error code {ErrorCode}: {ErrorDescription}"
            };
        }

        /// <summary>
        /// Returns a string representation of the exception.
        /// </summary>
        public override string ToString()
        {
            return $"MetaOxideException (Code: {ErrorCode}): {ErrorDescription}\n{StackTrace}";
        }
    }
}
