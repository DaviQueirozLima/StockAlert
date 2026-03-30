using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using StockAlert.Domain.Entities;
using StockAlert.Domain.Security;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace StockAlert.Infrastructure.Security;

public class JwtTokenGenerator : ITokenService
{
    private readonly IConfiguration _configuration;

    public JwtTokenGenerator(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public string GenerateToken(User user)
    {
        var signingKey = Environment.GetEnvironmentVariable("JWT_SIGNING_KEY")
         ?? _configuration["Settings:Jwt:SigningKey"];
        var expirationMinutes = _configuration.GetValue<int>("Settings:Jwt:ExpirationInMinutes");

        var key = Encoding.ASCII.GetBytes(signingKey!);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Name, user.Name)
            }),
            Expires = DateTime.UtcNow.AddMinutes(expirationMinutes),
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(key),
                SecurityAlgorithms.HmacSha256Signature)
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);

        return tokenHandler.WriteToken(token);
    }
}
