namespace SessionLogger.Exceptions;

[Serializable]
public class NotFoundException(string name, object key)
    : Exception($"Entity \"{name}\" with key \"{key}\" was not found.")
{
    public string Name { get; init; } = name;
}