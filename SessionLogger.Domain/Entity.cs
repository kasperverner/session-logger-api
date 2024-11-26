namespace SessionLogger;

public abstract class Entity
{
    public Guid Id { get; protected init; } = Guid.NewGuid();
    public DateTime CreatedDate { get; init; } = DateTime.UtcNow;
    public DateTime? ModifiedDate { get; private set; }
    public DateTime? DeletedDate { get; private set; }
    
    public void Delete()
        => DeletedDate = DateTime.UtcNow;
    
    public void Modify()
        => ModifiedDate = DateTime.UtcNow;
}