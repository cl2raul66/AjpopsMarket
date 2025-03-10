using System.Security.Cryptography;
using System.Text;

namespace AjpopsMarketServer.Helpers;

public class PasswordHelper
{
    /// <summary>
    /// Genera un hash SHA256 de la contraseña proporcionada.
    /// </summary>
    /// <param name="password">La contraseña en texto plano a hashear.</param>
    /// <returns>Una cadena hexadecimal que representa el hash SHA256 de la contraseña.</returns>
    public static string HashPassword(string password)
    {
        var hashedBytes = SHA256.HashData(Encoding.UTF8.GetBytes(password));
        return BitConverter.ToString(hashedBytes).Replace("-", "").ToLower();
    }

    /// <summary>
    /// Verifica si una contraseña coincide con su hash almacenado.
    /// </summary>
    /// <param name="storedHash">El hash almacenado de la contraseña original.</param>
    /// <param name="password">La contraseña en texto plano a verificar.</param>
    /// <returns>True si la contraseña coincide con el hash almacenado, false en caso contrario.</returns>
    public static bool VerifyPassword(string storedHash, string password)
    {
        var computedHash = HashPassword(password);
        return computedHash.Equals(storedHash);
    }
}
