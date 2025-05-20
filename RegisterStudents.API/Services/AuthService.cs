using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using RegisterStudents.API.Configurations;
using RegisterStudents.API.Data;
using RegisterStudents.API.Dto;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace RegisterStudents.API.Services
{
    public class AuthService : IAuthService
    {
        private readonly AppDbContext _context;
        private readonly JwtConfiguration _jwtSettings;

        public AuthService(AppDbContext context, IOptions<JwtConfiguration> jwtOptions)
        {
            _context = context;
            _jwtSettings = jwtOptions.Value;
        }

        public async Task<LoginResponseDto> AuthenticateAsync(string email, string password)
        {
            // Buscar el usuario e incluir su rol
            var user = await _context.Users
                .Include(u => u.Role) // ✅ Esto es clave
                .FirstOrDefaultAsync(u => u.Email == email);

            if (user == null)
                return null;

            // Verificar contraseña
            bool verified = BCrypt.Net.BCrypt.Verify(password, user.PasswordHash);
            if (!verified)
                return null;

            // Claims JWT
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()), // 👈 Este es importante
                new Claim("FullName", $"{user.FirstName} {user.LastName}")
            };

            if (!string.IsNullOrEmpty(user.Role?.Name))
            {
                claims.Add(new Claim(ClaimTypes.Role, user.Role.Name));
            }


            // Crear el token
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.SecretKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expiration = DateTime.UtcNow.AddMinutes(_jwtSettings.ExpirationMinutes);

            var token = new JwtSecurityToken(
                issuer: _jwtSettings.Issuer,
                audience: _jwtSettings.Audience,
                claims: claims,
                expires: expiration,
                signingCredentials: creds
            );

            return new LoginResponseDto
            {
                Token = new JwtSecurityTokenHandler().WriteToken(token),
                Expiration = expiration
            };
        }
    }
}
