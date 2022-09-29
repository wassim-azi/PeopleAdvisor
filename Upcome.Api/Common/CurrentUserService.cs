using System.Security.Claims;

namespace Upcome.Api.Common;

public interface ICurrentUserService
{
    public string? IpAddress { get; }
    public string? UserName { get; }
    public string? UserId { get; }
    bool IsAuthenticated { get; }
}

public class CurrentUserService : ICurrentUserService
{
    public string? IpAddress { get; }
    public string? UserName { get; }
    public string? UserId { get; }
    public bool IsAuthenticated { get; }

    public CurrentUserService(IHttpContextAccessor httpContextAccessor)
    {
        IpAddress = httpContextAccessor.HttpContext?.Connection.RemoteIpAddress?.MapToIPv4().ToString();
        UserName = httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.Name);
        UserId = httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);
        IsAuthenticated = string.IsNullOrWhiteSpace(UserId) == false;
    }
}