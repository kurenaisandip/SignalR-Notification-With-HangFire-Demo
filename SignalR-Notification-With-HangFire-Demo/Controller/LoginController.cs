using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using SignalR_Notification_With_HangFire_Demo.EF;
using SignalR_Notification_With_HangFire_Demo.EF.Entity;
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
        public string Username { get; set; }
        public string Password { get; set; }
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequestDto request)
    {
        var user = await _context.Logins.FirstOrDefaultAsync(x => x.Username == request.Username && x.IsActive == true);
        if (user == null || user.PasswordHash != request.Password) // NOTE: Replace with proper password hash check in production
        {
            return Unauthorized("Invalid credentials or inactive user.");
        }

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, user.Username),
            new Claim(ClaimTypes.Role, user.Role),
            new Claim("role", user.Role), // For policy-based auth
            new Claim("loginId", user.LoginID.ToString())
        };

        var jwtSettings = _configuration.GetSection("Jwt");
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Key"]));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var expires = DateTime.UtcNow.AddMinutes(Convert.ToDouble(jwtSettings["ExpireMinutes"]));

        // Wrap claims in ClaimsIdentity and ClaimsPrincipal (for demonstration)
        var identity = new ClaimsIdentity(claims, "Token");
        var principal = new ClaimsPrincipal(identity);
        // You can use 'principal' for manual authentication context if needed

        var token = new JwtSecurityToken(
            issuer: jwtSettings["Issuer"],
            audience: jwtSettings["Audience"],
            claims: identity.Claims, // Use claims from ClaimsIdentity
            expires: expires,
            signingCredentials: creds
        );

        var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

        return Ok(new {
            token = tokenString,
            role = user.Role,
            expires = expires
        });
    }
}