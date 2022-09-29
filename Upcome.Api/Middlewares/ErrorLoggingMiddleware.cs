using System.Net;
using System.Security.Authentication;
using Upcome.Api.Common;

namespace Upcome.Api.Middlewares;

/// <summary>
/// Error logging middleware
/// </summary>
public class ErrorLoggingMiddleware
{
    private readonly RequestDelegate _next;

    /// <summary>
    /// Constructor
    /// </summary>
    public ErrorLoggingMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    /// <summary>
    /// Invoke
    /// </summary>
    /// <param name="context">HTTP context</param>
    public async Task Invoke(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (FileNotFoundException exception)
        {
            await HandleExceptionAsync(context ?? new DefaultHttpContext(), exception);
        }
        catch (WebException exception)
        {
            await HandleExceptionAsync(context ?? new DefaultHttpContext(), exception);
        }
        catch (AuthenticationException exception)
        {
            await HandleExceptionAsync(context ?? new DefaultHttpContext(), exception);
        }
        catch (Exception exception)
        {
            await HandleExceptionAsync(context ?? new DefaultHttpContext(), exception);
        }
    }

    /// <summary>
    /// handle exception
    /// </summary>
    /// <param name="context">HTTP context</param>
    /// <param name="exception">Exception</param>
    private Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
        return context.Response.WriteAsync(new ErrorDetails
        {
            StatusCode = context.Response.StatusCode,
            Message = "Internal Server Error"
        }.ToString());
    }
}