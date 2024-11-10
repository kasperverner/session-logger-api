using SessionLogger.Users;

namespace SessionLogger.Tasks;

public class Comment : Entity
{
    private Comment() { }
    
    public Comment(Guid taskId, User user, string value, Comment? parent = null)
    {
        TaskId = taskId;
        
        UserId = user.Id;
        User = user;
        
        ParentId = parent?.Id;
        Parent = parent;
        
        Value = value;
    }
    
    public Guid TaskId { get; init; }
    public Guid UserId { get; init; }
    public User User { get; init; }
    public Guid? ParentId { get; init; }
    public Comment? Parent { get; init; }
    public string Value { get; private set; }

    public void UpdateValue(string value)
    {
        if (Value == value)
            return;
        
        Value = value;
    }
}