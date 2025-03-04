using System.Security.Cryptography;
using System.Text;

namespace AjpopsMarketServer.Helpers;

public class PasswordHelper
{
    // Método privado de utilidad para hashear contraseñas
    public static string HashPassword(string password)
    {
        using var sha256 = SHA256.Create();
        var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
        return BitConverter.ToString(hashedBytes).Replace("-", "").ToLower();
    }

    // Método de verificación de contraseña
    public static bool VerifyPassword(string storedHash, string password)
    {
        var computedHash = HashPassword(password);
        return computedHash.Equals(storedHash);
    }
}
