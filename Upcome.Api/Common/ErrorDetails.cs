using Newtonsoft.Json;

namespace Upcome.Api.Common;

/// <summary>
/// Error details
/// </summary>
public class ErrorDetails
{
    /// <summary>
    /// Status code
    /// </summary>
    public int StatusCode { get; set; }

    /// <summary>
    /// Message
    /// </summary>
    public string? Message { get; set; }

    /// <inheritdoc />
    public override string ToString()
    {
        return JsonConvert.SerializeObject(this);
    }
}