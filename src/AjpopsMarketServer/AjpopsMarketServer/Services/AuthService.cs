// Ignore Spelling: jwt

using AjpopsMarketServer.Enums;
using AjpopsMarketServer.Helpers;
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
    Task<(bool Success, string ErrorMessage, string Token, User? User)> AuthenticateAsync(string email, string password);
    Task<(bool Success, string ErrorMessage, User? User)> RegisterAsync(User user, string password);
    Task<bool> LogOutAsync(string userId);
    bool IsSessionActiveAsync(string userId, string token);
    Task<string> GenerateJwtTokenAsync(User user);
}

public class AuthService : IAuthService
{
    private readonly IUserRepository userRepo;
    private readonly JwtSettings _jwtSettings;

    public AuthService(IUserRepository userRepository, IOptions<JwtSettings> jwtSettings)
    {
        userRepo = userRepository;
        _jwtSettings = jwtSettings.Value;
    }

    public bool IsSessionActiveAsync(string userId, string token)
    {
        // Validate if the token is valid and belongs to the user
        if (string.IsNullOrEmpty(token) || string.IsNullOrEmpty(userId))
            return false;

        // Validate the token
        try
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_jwtSettings.Secret);

            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = false,
                ValidateAudience = false,
                ClockSkew = TimeSpan.Zero
            };

            // This validates the token signature and expiration
            var principal = tokenHandler.ValidateToken(token, validationParameters, out var validatedToken);

            // Extract user ID from the token
            var userIdClaim = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            // Check if the token belongs to the user
            return userIdClaim == userId;
        }
        catch
        {
            return false;
        }
    }

    public async Task<(bool Success, string ErrorMessage, string Token, User? User)> AuthenticateAsync(string email, string password)
    {
        var user = await userRepo.GetByEmailAsync(email);

        if (user is null)
        {
            return (false, "Usuario incorrecto", string.Empty, null);
        }

        if (!PasswordHelper.VerifyPassword(user.PasswordHash, password))
        {
            return (false, "Contraseña incorrecta", string.Empty, null);
        }

        if (!user.IsActive)
        {
            return (false, "Usuario inactivo", string.Empty, null);
        }

        // Generate JWT token
        var token = await GenerateJwtTokenAsync(user);

        // Update last login timestamp
        user.LastLogin = DateTime.UtcNow;
        var updateInput = new UpdateUserInput(
            user.Id,
            user.Email, 
            user.PasswordHash, 
            user.FullName,
            user.Type,
            user.IsActive
        );
        await userRepo.UpdateAsync(updateInput);

        return (true, string.Empty, token, user);
    }

    public async Task<string> GenerateJwtTokenAsync(User user)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_jwtSettings.Secret);

        var expiration = user.Type is UserType.Admin ? DateTime.UtcNow.AddMinutes(30) : DateTime.UtcNow.AddDays(1);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(
            [
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Type.ToString())
            ]),
            Expires = expiration,
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(key),
                SecurityAlgorithms.HmacSha256Signature
            )
        };

        // Add membership level claim if user is a member
        if (user is Member member)
        {
            tokenDescriptor.Subject.AddClaim(new Claim("MembershipLevel", member.Level.ToString()));
        }

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }

    public async Task<bool> LogOutAsync(string userId)
    {
        // In a token-based authentication system like JWT, 
        // traditional logout involves client-side token disposal

        // For server-side logout tracking, you would need to implement
        // a token blacklist or revocation system

        // This is a simple implementation that could be expanded
        // with a token blacklist stored in database or cache

        // For demonstration, we'll just return true
        // In a real implementation, you might:
        // 1. Add the token to a blacklist
        // 2. Update user session state in database

        return true;
    }

    public async Task<(bool Success, string ErrorMessage, User? User)> RegisterAsync(User user, string password)
    {
        var foundByEmail = await userRepo.GetByEmailAsync(user.Email);
        if (foundByEmail is not null)
        {
            return (false, "El correo electrónico ya está registrado", null);
        }

        string hashedPassword = PasswordHelper.HashPassword(password);

        // Set common user properties
        user.PasswordHash = hashedPassword;
        user.CreatedAt = DateTime.UtcNow;
        user.Id = Guid.NewGuid().ToString("N");

        userRepo.BeginTransaction();
        var createdUser = await userRepo.CreateAsync(user);

        if (createdUser is null)
        {
            userRepo.Rollback();
            return (false, "No se pudo crear el usuario", null);
        }

        userRepo.Commit();
        return (true, string.Empty, createdUser);
    }

    public async Task<(bool Success, string ErrorMessage, AuthResponseOutput? response)> LogInAsync(LoginInput input)
    {
        var authResult = await AuthenticateAsync(input.Email, input.Password);

        if (!authResult.Success)
        {
            return (false, authResult.ErrorMessage, null);
        }

        // Calculate token expiration (should match the expiration set in GenerateJwtTokenAsync)
        var expiration = authResult.User?.Type is UserType.Admin
            ? DateTime.UtcNow.AddMinutes(30)
            : DateTime.UtcNow.AddDays(1);

        return (true, string.Empty, new AuthResponseOutput(authResult.Token, expiration));
    }
}