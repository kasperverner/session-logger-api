namespace SessionLogger.Exceptions;

[Serializable]
public class ProblemException(string title, string message) : Exception(message)
{
    public string Title { get; init; } = title;
}