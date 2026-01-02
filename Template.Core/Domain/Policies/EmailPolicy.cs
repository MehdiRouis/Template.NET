using System.Text.RegularExpressions;

namespace Template.Core.Domain.Policies;

public static class EmailPolicy
{
    private static readonly Regex Regex = new(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.Compiled);

    public static bool IsValid(string email) => !string.IsNullOrWhiteSpace(email) && Regex.IsMatch(email);
}
