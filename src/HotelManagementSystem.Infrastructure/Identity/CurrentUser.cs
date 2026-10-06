using HotelManagementSystem.Application.Common.Interfaces;
using Microsoft.AspNetCore.Http;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace HotelManagementSystem.Infrastructure.Identity
{
    public sealed class CurrentUser(IHttpContextAccessor httpContextAccessor) : ICurrentUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;

        private ClaimsPrincipal? User => _httpContextAccessor.HttpContext?.User;

        public Guid? UserId => Guid.TryParse(User?.FindFirst(JwtRegisteredClaimNames.Sub)?.Value, out var id) ? id : null;

        public string? UserName => User?.FindFirst(JwtRegisteredClaimNames.UniqueName)?.Value;

        public Guid? HotelId => Guid.TryParse(User?.FindFirst("hotelId")?.Value, out var hotelId) ? hotelId : null;

        public IReadOnlyCollection<string> Roles => User?.FindAll(ClaimTypes.Role).Select(c => c.Value).ToList() ?? new List<string>();

        public bool IsAuthenticated => User?.Identity?.IsAuthenticated ?? false;
    }
}