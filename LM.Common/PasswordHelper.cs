namespace LM.Common;
using System.Security.Cryptography;
using System.Text;

public static class PasswordHelper
{
    public static byte[] HashPassword(string password)
    {
        using var sha256 = SHA256.Create();
        return sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
    }

    public static bool VerifyPassword(string password, byte[] hash)
    {
        var hashedInput = HashPassword(password);
        return hashedInput.SequenceEqual(hash);
    }
}
