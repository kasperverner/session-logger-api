using SessionLogger.Extensions;
using SessionLogger.Filters.Parameters;

namespace SessionLogger.Departments;

public static class DepartmentsEndpoints
{
    public static IEndpointRouteBuilder MapDepartmentsEndpoints(this IEndpointRouteBuilder application)
    {
        var endpoints = application
            .MapGroup("/departments")
            .WithTags("Departments")
            .MapEndpoint<GetDepartments>()
            .MapEndpoint<CreateDepartment>()
            .MapGroup("/{departmentId:guid}")
            .AddEndpointFilter<DepartmentIdFromRouteFilter>()
            .MapEndpoint<GetDepartment>()
            .MapEndpoint<UpdateDepartment>()
            .MapEndpoint<DeleteDepartment>();
        
        return application;
    }
}