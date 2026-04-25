using System.Security.Cryptography;
using System.Text;

namespace ExaminationSystem.Application.Common.Helper;

public static class OtpHasher
{
    public static string Hash(string otp)
    {
        byte[] bytes = SHA256.HashData(Encoding.UTF8.GetBytes(otp));
        return Convert.ToHexString(bytes); // or Convert.ToBase64String(bytes)
    }

    public static bool Verify(string otp, string storedHash)
    {
        string hash = Hash(otp);
        // Use constant-time comparison to prevent timing attacks
        return CryptographicOperations.FixedTimeEquals(
            Convert.FromHexString(hash),
            Convert.FromHexString(storedHash)
        );
    }
}