namespace SessionLogger;

public abstract class Entity
{
    public Guid Id { get; protected init; } = Guid.NewGuid();
    public DateTime CreatedDate { get; init; } = DateTime.UtcNow;
    public DateTime? ModifiedDate { get; set; }
    public DateTime? DeletedDate { get; set; }
    
    public void Delete()
        => DeletedDate = DateTime.UtcNow;
}