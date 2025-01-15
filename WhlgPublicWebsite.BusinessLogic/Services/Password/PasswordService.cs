using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Options;

namespace WhlgPublicWebsite.BusinessLogic.Services.Password;

public class PasswordService(IOptions<PasswordConfiguration> options)
{
    public bool HashMatchesConfiguredPassword(string hash)
    {
        if (options.Value.Password == null)
        {
            return false;
        }
        
        return HashPassword(options.Value.Password) == hash;
    }

    public string HashPassword(string password)
    {
        using var sha256 = SHA256.Create();
        var bytes = Encoding.UTF8.GetBytes(password);
        var hashValue = sha256.ComputeHash(bytes);
        return Convert.ToBase64String(hashValue);
    }
}