using System.Globalization;

namespace PeopleAdvisor.Api.Common;

/// <summary>
/// custom exception class for throwing application specific exceptions (e.g. for validation)
/// that can be caught and handled within the application
/// </summary>
public abstract class AppException : Exception
{
    /// <inheritdoc />
    protected AppException() : base() { }


    /// <inheritdoc />
    protected AppException(string message) : base(message) { }

    /// <inheritdoc />
    protected AppException(string message, params object[] args) : base(string.Format(CultureInfo.CurrentCulture, message, args))
    {
    }
}