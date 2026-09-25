using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using PhonebookApplication.Models;
using PhonebookApplication.Repositories;

namespace PhonebookApplication.Services
{
    public class AuthService : IAuthService
    {
        private const int TokenLifetimeHours = 1;

        private readonly IUserRepository _userRepository;
        private readonly IPasswordService _passwordService;
        private readonly IConfiguration _configuration;

        public AuthService(
            IUserRepository userRepository,
            IPasswordService passwordService,
            IConfiguration configuration)
        {
            _userRepository = userRepository;
            _passwordService = passwordService;
            _configuration = configuration;
        }

        public async Task<LoginResponse?> LoginAsync(LoginRequest request)
        {
            User? user = await _userRepository.GetUserForLoginAsync(request.Username);

            if (user == null ||
                !_passwordService.VerifyPassword(
                    user,
                    user.PasswordHash,
                    request.Password))
            {
                return null;
            }

            string issuer = GetRequiredConfiguration("Jwt:Issuer");
            string audience = GetRequiredConfiguration("Jwt:Audience");
            string signingKey = GetRequiredConfiguration("Jwt:Key");
            byte[] signingKeyBytes = Encoding.UTF8.GetBytes(signingKey);

            if (signingKeyBytes.Length < 32)
            {
                throw new InvalidOperationException(
                    "Jwt:Key must contain at least 32 bytes of key material.");
            }

            DateTime issuedAt = DateTime.UtcNow;
            DateTime expiresAt = issuedAt.AddHours(TokenLifetimeHours);

            var claims = new[]
            {
                new Claim(
                    ClaimTypes.NameIdentifier,
                    user.Id.ToString(CultureInfo.InvariantCulture)),
                new Claim(ClaimTypes.Name, user.Username)
            };

            var signingCredentials = new SigningCredentials(
                new SymmetricSecurityKey(signingKeyBytes),
                SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                notBefore: issuedAt,
                expires: expiresAt,
                signingCredentials: signingCredentials);

            string encodedToken = new JwtSecurityTokenHandler().WriteToken(token);

            return new LoginResponse
            {
                Token = encodedToken,
                Username = user.Username,
                ExpiresAt = expiresAt
            };
        }

        private string GetRequiredConfiguration(string key)
        {
            string? value = _configuration[key];

            if (string.IsNullOrWhiteSpace(value))
            {
                throw new InvalidOperationException(
                    $"JWT configuration value '{key}' is missing.");
            }

            return value;
        }
    }
}
