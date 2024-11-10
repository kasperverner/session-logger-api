namespace SessionLogger.Exceptions;

[Serializable]
public class ForbiddenAccessException(string message) 
    : Exception(message);