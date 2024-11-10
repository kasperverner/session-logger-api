namespace SessionLogger.Users;

[Flags]
public enum Role
{
    None = 0,
    Employee = 1,
    Manager = 1 << 1,
}