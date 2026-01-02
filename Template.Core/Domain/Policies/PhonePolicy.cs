using System.Text.RegularExpressions;

namespace Template.Core.Domain.Policies;

public static class PhonePolicy
{
    private static readonly Regex Regex = new(@"^\+?[0-9]{8,15}$", RegexOptions.Compiled);

    public static bool IsValid(string phone) => !string.IsNullOrWhiteSpace(phone) && Regex.IsMatch(phone);
}
