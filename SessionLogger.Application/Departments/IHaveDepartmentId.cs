namespace SessionLogger.Departments;

public interface IHaveDepartmentId
{
    public Guid DepartmentId { get; init; }
}