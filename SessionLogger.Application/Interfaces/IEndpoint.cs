using Microsoft.AspNetCore.Routing;

namespace SessionLogger.Interfaces;

public interface IEndpoint
{
    static abstract void Map(IEndpointRouteBuilder application);
}