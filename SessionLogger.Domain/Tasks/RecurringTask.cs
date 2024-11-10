namespace SessionLogger.Tasks;

public class RecurringTask : Task
{
    private RecurringTask()
    {
        Type = TaskType.Recurring;
    }

    public RecurringTask(Guid projectId, string name, string? description = null) : base(projectId, name, description)
    {
        Type = TaskType.Recurring;
    }
}