using Microsoft.AspNetCore.Http;
using StockAlert.Domain.Security;
using System.Security.Claims;

namespace StockAlert.Infrastructure.Security
{
    public class LoggedUserAccessor : ILoggedUserAccessor
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        public LoggedUserAccessor(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }
        public Guid GetUserId()
        {
            // Pega o ID que guardamos no Claim (ClaimTypes.NameIdentifier) dentro do JwtTokenGenerator
            var identifier = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(identifier))
                throw new System.UnauthorizedAccessException("User not identified.");

            return Guid.Parse(identifier);
        }
    }
}
