using SessionLogger.Projects;
using SessionLogger.Users;

namespace SessionLogger.Tasks;

public abstract class Task : Entity
{
    protected Task() { }

    public Task(Project project, string name, string? description = null)
    {
        Id = Guid.NewGuid();
        ProjectId = project.Id;
        Project = project;
        Name = name;
        Description = description;
    }
    
    public Guid ProjectId { get; init; }
    public Project Project { get; init; }
    
    public TaskType Type { get; init; }
    public string Name { get; private set; }
    public string? Description { get; private set; }
    public IList<User> AssignedUsers { get; init; } = new List<User>();
    public IList<Comment> Comments { get; init; } = new List<Comment>();

    public TaskState? GetState()
    {
        if (Type == TaskType.Completable)
            return ((CompletableTask)this).State;
        
        return null;
    }
    
    public void AssignUser(User user)
    {
        AssignedUsers.Add(user);
        user.AssignTask(this);
    }

    public Comment AddComment(User user, string value, Comment? parent = null)
    {
        var comment = new Comment(Id, user, value, parent);
        Comments.Add(comment);
        
        return comment;
    }

    public void UpdateName(string name)
        => Name = name;
    
    public void UpdateDescription(string? description)
        => Description = description;
}