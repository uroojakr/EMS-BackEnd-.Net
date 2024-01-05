

using EMS.Business.Interfaces;
using EMS.Business.Models;
using EMSWebApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace EMSWebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : Microsoft.AspNetCore.Mvc.ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IConfiguration _configuration;
        private readonly ILogger<AuthController> _logger;

        public AuthController(IUserService userService, IConfiguration configuration, ILogger<AuthController> logger)
        {
            _configuration = configuration;
            _userService = userService;
            _logger = logger;
        }

        [Microsoft.AspNetCore.Mvc.HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            var user = await _userService.AuthenticateUser(model.UserName, model.Password);

            if (user == null)
            {
                _logger.LogWarning("Authentication failed for user: {UserName}", model.UserName);
                return Unauthorized();
            }

            var token = GenerateJwtToken(user);
            return Ok(new { token });
        }

        [NonAction]

        public string GenerateJwtToken(UserModel user)
        {
            var section = _configuration.GetSection("JwtSettings");
            var secretKey = section["JwtSecretKey"];
            var keyBytes = System.Text.Encoding.ASCII.GetBytes(secretKey!);
            var issuer = section["JwtIssuer"];
            var audience1 = section["JwtAudience"];
            var expires = DateTime.UtcNow.AddHours(1);

            var claims = new[]
            {
                 new Claim(ClaimTypes.Name, user.UserName),
                  new Claim(ClaimTypes.Role, user.UserType.ToString())
            };
            Console.WriteLine($"Claims in the JWT: {string.Join(", ", claims.Select(c => c.ToString()))}");

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience1,
                claims: claims,
                expires: expires,
                signingCredentials: new SigningCredentials(new SymmetricSecurityKey(keyBytes), SecurityAlgorithms.HmacSha256)
            );
            var _token = new JwtSecurityTokenHandler().WriteToken(token);
            return _token;
        }

    }
}
