using System.Security.Cryptography;
using System.Text;

namespace WhlgPublicWebsite.BusinessLogic.Services.Password;

public class PasswordService
{
    public bool HashMatches(string hash, string password)
    {
        return HashPassword(password) == hash;
    }

    public string HashPassword(string password)
    {
        using var sha256 = SHA256.Create();
        var bytes = Encoding.UTF8.GetBytes(password);
        var hashValue = sha256.ComputeHash(bytes);
        return Convert.ToBase64String(hashValue);
    }
}