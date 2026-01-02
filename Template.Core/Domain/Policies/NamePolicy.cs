namespace Template.Core.Domain.Policies;

public static class NamePolicy
{
    public static bool IsValid(string name)
    {
        if (string.IsNullOrWhiteSpace(name)) return false;
        if (name.Length < 2 || name.Length > 100) return false;
        return name.All(c => char.IsLetter(c) || c == ' ' || c == '-');
    }
}
