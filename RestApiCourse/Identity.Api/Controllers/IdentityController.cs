using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.Json;

namespace Identity.Api.Controllers
{
    [ApiController]
    public class IdentityController : ControllerBase
    {
        private readonly JwtSettings _jwtSettings;

        private static readonly string Issuer = "https://id.palamarrostyslav.com";
        private static readonly string Audience = "https://movies.palamarrostyslav.com";

        public IdentityController(IOptions<JwtSettings> jwtOptions)
        {
            _jwtSettings = jwtOptions.Value;
        }

        [HttpPost("token")]
        public IActionResult GenerateToken([FromBody] TokenGenerationRequest request)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_jwtSettings.TokenSecret);

            var claims = new List<Claim>
            {
                new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new(JwtRegisteredClaimNames.Sub, request.Email),
                new(JwtRegisteredClaimNames.Email, request.Email),
                new("userid", request.UserId.ToString())
            };

            foreach (var customClaim in request.CustomClaims)
            {
                var jsonElement = (JsonElement)customClaim.Value;
                var valueType = jsonElement.ValueKind switch
                {
                    JsonValueKind.True => ClaimValueTypes.Boolean,
                    JsonValueKind.False => ClaimValueTypes.Boolean,
                    JsonValueKind.Number => ClaimValueTypes.Double,
                    _ => ClaimValueTypes.String
                };

                var claim = new Claim(customClaim.Key, customClaim.Value.ToString()!, valueType);
                claims.Add(claim);
            }

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.Add(TimeSpan.FromHours(_jwtSettings.TokenLifetimeHours)),
                Issuer = Issuer,
                Audience = Audience,
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);

            var jwt = tokenHandler.WriteToken(token);
            return Ok(jwt);
        }
    }
}
