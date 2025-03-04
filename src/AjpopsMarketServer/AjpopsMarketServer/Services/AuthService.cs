// Ignore Spelling: jwt

using AjpopsMarketServer.Models;
using AjpopsMarketServer.Repositories;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace AjpopsMarketServer.Services;

public interface IAuthService
{
    Task<AuthResult> AuthenticateAsync(string email, string password);
    string GenerateJwtToken(User user);
}

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly JwtSettings _jwtSettings;

    public AuthService(IUserRepository userRepository, IOptions<JwtSettings> jwtSettings)
    {
        _userRepository = userRepository;
        _jwtSettings = jwtSettings.Value;
    }

    public async Task<AuthResult> AuthenticateAsync(string email, string password)
    {
        // Buscar usuario por email
        var user = await _userRepository.GetByEmailAsync(email);

        if (user == null)
        {
            return new AuthResult
            {
                Success = false,
                ErrorMessage = "Usuario no encontrado"
            };
        }

        // Verificar contraseña (debería estar hasheada en producción)
        if (user.PasswordHash != password) // En producción: !VerifyPasswordHash(password, user.PasswordHash)
        {
            return new AuthResult
            {
                Success = false,
                ErrorMessage = "Contraseña incorrecta"
            };
        }

        // Actualizar última fecha de login
        user = await _userRepository.UpdateLastLoginAsync(user.Id);

        // Generar token JWT
        var token = GenerateJwtToken(user);

        return new AuthResult
        {
            Success = true,
            Token = token,
            User = user
        };
    }

    public string GenerateJwtToken(User user)
    {
        // Generar token que es válido por 7 días
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_jwtSettings.Secret);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.Email, user.Email)
            }),
            Expires = DateTime.UtcNow.AddDays(7),
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(key),
                SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }
}
