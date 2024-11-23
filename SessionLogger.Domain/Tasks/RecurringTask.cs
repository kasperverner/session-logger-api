using SessionLogger.Projects;

namespace SessionLogger.Tasks;

public class RecurringTask : Task
{
    private RecurringTask()
    {
        Type = TaskType.Recurring;
    }

    public RecurringTask(Project project, string name, string? description = null) : base(project, name, description)
    {
        Type = TaskType.Recurring;
    }
}