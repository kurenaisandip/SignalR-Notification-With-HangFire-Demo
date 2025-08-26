using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SignalR_Notification_With_HangFire_Demo.EF;
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace SignalR_Notification_With_HangFire_Demo.Controller;

[ApiController]
[Route("api/Login")]
public class LoginController: ControllerBase
{
    private readonly AppDbContext _context;
    private readonly IConfiguration _configuration;

    public LoginController(AppDbContext context, IConfiguration configuration)
    {
        _context = context;
        _configuration = configuration;
    }

    public class LoginRequestDto
    {
        [Required]
        public string Username { get; set; } = default!;
        [Required]
        public string Password { get; set; } = default!;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequestDto request)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var user = await _context.Logins.FirstOrDefaultAsync(x => x.Username == request.Username && x.IsActive);
        if (user == null || user.PasswordHash != request.Password) // TODO: Replace with proper password hash check
        {
            return Unauthorized("Invalid credentials or inactive user.");
        }

        // Build claims (role added as both ClaimTypes.Role and 'role' for policies)
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, user.Username),
            new Claim(ClaimTypes.Role, user.Role),
            new Claim("role", user.Role),
            new Claim("loginId", user.LoginID.ToString())
        };

        var jwtSection = _configuration.GetSection("Jwt");
        var keyStr = jwtSection.GetValue<string>("Key");
        var issuer = jwtSection.GetValue<string>("Issuer");
        var audience = jwtSection.GetValue<string>("Audience");
        var expireMinutes = jwtSection.GetValue<int?>("ExpireMinutes") ?? 60;

        if (string.IsNullOrWhiteSpace(keyStr) || string.IsNullOrWhiteSpace(issuer) || string.IsNullOrWhiteSpace(audience))
        {
            return StatusCode(500, "JWT configuration is missing. Please check appsettings.json.");
        }

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(keyStr));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var expires = DateTime.UtcNow.AddMinutes(expireMinutes);

        // Pass the claims directly to the token
        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: expires,
            signingCredentials: creds
        );

        var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

        // Optionally update last login
        user.LastLogin = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        return Ok(new {
            token = tokenString,
            role = user.Role,
            expires = expires
        });
    }
}