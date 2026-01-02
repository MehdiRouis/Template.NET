namespace Template.Core.Domain.Policies;

public static class PasswordPolicy
{
    public static bool IsStrong(string password)
    {
        if (string.IsNullOrWhiteSpace(password)) return false;
        if (password.Length < 12) return false;

        return password.Any(char.IsUpper)
            && password.Any(char.IsLower)
            && password.Any(char.IsDigit)
            && password.Any("!@#$%^&*".Contains);
    }
}
