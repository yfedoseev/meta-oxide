package io.github.yfedoseev.metaoxide;

/**
 * Exception thrown when MetaOxide encounters an error during metadata extraction.
 * <p>
 * This exception wraps native library errors with error codes and messages from the underlying
 * Rust implementation.
 * </p>
 *
 * @since 0.1.0
 */
public class MetaOxideException extends Exception {

    private static final long serialVersionUID = 1L;

    private final int errorCode;

    /**
     * Constructs a new MetaOxideException with the specified error message.
     *
     * @param message the error message
     */
    public MetaOxideException(String message) {
        super(message);
        this.errorCode = -1;
    }

    /**
     * Constructs a new MetaOxideException with the specified error code and message.
     *
     * @param errorCode the error code from the native library
     * @param message   the error message
     */
    public MetaOxideException(int errorCode, String message) {
        super(message);
        this.errorCode = errorCode;
    }

    /**
     * Constructs a new MetaOxideException with the specified message and cause.
     *
     * @param message the error message
     * @param cause   the cause of this exception
     */
    public MetaOxideException(String message, Throwable cause) {
        super(message, cause);
        this.errorCode = -1;
    }

    /**
     * Gets the error code from the native library.
     * <p>
     * Returns -1 if no specific error code is available.
     * </p>
     *
     * @return the error code, or -1 if unavailable
     */
    public int getErrorCode() {
        return errorCode;
    }

    /**
     * Returns a string representation of this exception, including the error code if available.
     *
     * @return a string representation
     */
    @Override
    public String toString() {
        if (errorCode >= 0) {
            return "MetaOxideException[code=" + errorCode + "]: " + getMessage();
        }
        return "MetaOxideException: " + getMessage();
    }
}
