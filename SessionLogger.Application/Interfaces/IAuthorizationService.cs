namespace SessionLogger.Interfaces;

public interface IAuthorizationService
{
    Guid GetAuthorizedUserPrincipalId();
}