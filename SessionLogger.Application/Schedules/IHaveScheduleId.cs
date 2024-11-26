namespace SessionLogger.Schedules;

public interface IHaveScheduleId
{
    public Guid ScheduleId { get; init; }
}