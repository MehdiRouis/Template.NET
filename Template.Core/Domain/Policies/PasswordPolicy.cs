namespace Template.Core.Domain.Policies;

public static class PasswordPolicy
{
    public static bool IsValid(string password)
    {
        if (string.IsNullOrWhiteSpace(password))
            return false;

        if (password.Length < 1)
            return false;

        if (password.Length > 256)
            return false;

        return true;
    }

    public static bool IsStrong(string password)
    {
        if (!IsValid(password))
            return false;

        if (password.Length < 12)
            return false;

        return password.Any(char.IsUpper)
            && password.Any(char.IsLower)
            && password.Any(char.IsDigit)
            && password.Any("!@#$%^&*".Contains);
    }
}
