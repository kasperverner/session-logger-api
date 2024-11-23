using Microsoft.AspNetCore.Http;
using SessionLogger.Extensions;
using SessionLogger.Interfaces;

namespace SessionLogger.Infrastructure.Services;

public class AuthorizationService(IHttpContextAccessor contextAccessor) : IAuthorizationService
{
    public Guid GetAuthorizedUserPrincipalId()
     => contextAccessor.HttpContext?.User.GetPrincipalId() 
        ?? throw new UnauthorizedAccessException();
}