using System.Security.Claims;
using SessionLogger.Exceptions;

namespace SessionLogger.Extensions;

public static class ClaimsPrincipalExtensions
{
    public static Guid GetPrincipalId(this ClaimsPrincipal claimsPrincipal)
    {
        if(!Guid.TryParse(claimsPrincipal.FindFirstValue(ClaimTypes.NameIdentifier), out var id))
        {
            throw new ProblemException("Invalid PrincipalId", "The PrincipalId is not a valid Guid");
        }

        return id;
    }

    public static string GetPrincipalName(this ClaimsPrincipal claimsPrincipal)
    {
        var name = claimsPrincipal.Identity?.Name;
        
        return name ?? throw new ProblemException("Invalid PrincipalName", "The Bearer token \"name\" is not set");
    }


    public static string GetPrincipalEmail(this ClaimsPrincipal claimsPrincipal)
    {
        var email = claimsPrincipal.FindFirstValue(ClaimTypes.Email);
        
        return email ?? throw new ProblemException("Invalid PrincipalEmail", "The Bearer token \"email\" is not set");
    }
}