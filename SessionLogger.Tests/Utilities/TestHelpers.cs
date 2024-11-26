namespace SessionLogger.Tests.Utilities;

public static class TestHelpers
{
    public static class Constants
    {
        public static class Customers
        {
            public const string Name = "Megadodo Publications";
        }

        public static class Users
        {
            public static class User
            {
                public static readonly Guid PrincipalId = Guid.Parse("11111111-1111-1111-1111-111111111111");
                public const string Name = "Test User";
                public const string Email = "user@domain.tld";
            }
        }
    }
}