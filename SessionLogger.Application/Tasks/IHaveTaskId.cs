namespace SessionLogger.Tasks;

public interface IHaveTaskId
{
    public Guid TaskId { get; init; }
}