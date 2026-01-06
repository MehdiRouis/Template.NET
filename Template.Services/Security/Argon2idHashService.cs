using Konscious.Security.Cryptography;
using Microsoft.Extensions.Configuration;
using System.Security.Cryptography;
using System.Text;
using Template.Services.Security;

public class Argon2idHashService : IHashService
{
    #region Fields
    private readonly IConfiguration _configuration;
    private readonly Dictionary<string, byte[]> _pepperCache = new();
    #endregion Fields

    public Argon2idHashService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    private byte[] GetPepper(string key)
    {
        if (_pepperCache.TryGetValue(key, out var cached))
            return cached;

        var pepper = Encoding.UTF8.GetBytes(_configuration["Auth:" + key] ?? throw new ArgumentNullException("Auth:" + key + " configuration is missing"));

        if (pepper.Length < 32)
            throw new InvalidOperationException("Auth:" + key + " must be at least 256 bits.");

        _pepperCache[key] = pepper;
        return pepper;
    }

    public string Hash(string text, string pepperKey)
    {
        var salt = RandomNumberGenerator.GetBytes(16);

        var argon = new Argon2id(Encoding.UTF8.GetBytes(text))
        {
            Salt = salt,
            KnownSecret = GetPepper(pepperKey),
            Iterations = 4,
            MemorySize = 65536,
            DegreeOfParallelism = 2
        };

        var hash = argon.GetBytes(32);

        return $"ARGON2ID$4$65536$2${Convert.ToBase64String(salt)}${Convert.ToBase64String(hash)}";
    }

    public bool Verify(string text, string textHash, string pepperKey)
    {
        var parts = textHash.Split('$', StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length != 6 || parts[0] != "ARGON2ID") return false;

        var salt = Convert.FromBase64String(parts[4]);
        var expected = Convert.FromBase64String(parts[5]);

        var argon = new Argon2id(Encoding.UTF8.GetBytes(text))
        {
            Salt = salt,
            KnownSecret = GetPepper(pepperKey),
            Iterations = int.Parse(parts[1]),
            MemorySize = int.Parse(parts[2]),
            DegreeOfParallelism = int.Parse(parts[3])
        };

        var actual = argon.GetBytes(expected.Length);

        return CryptographicOperations.FixedTimeEquals(actual, expected);
    }

    public static class HashPeppers     {
        public const string PasswordPepper = "PasswordPepper";
        public const string RefreshPepper = "RefreshPepper";
    }
}
