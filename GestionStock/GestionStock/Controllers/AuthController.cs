using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using GestionStock.Context;
using GestionStock.DTOs;
using GestionStock.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace GestionStock.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _config;

        public AuthController(ApplicationDbContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDto dto)
        {
            if (_context.Utilisateurs.Any(u => u.Email == dto.Email))
                return BadRequest("Email déjà utilisé");

            var user = new Utilisateur
            {
                Nom = dto.Nom,
                Email = dto.Email,
                Role = Enum.Parse<Role>(dto.Role),
                Password = BCrypt.Net.BCrypt.HashPassword(dto.Password), // 🔒 hachage 
                Telephone = dto.Telephone,
                Adresse = dto.Adresse,
            };

            _context.Utilisateurs.Add(user);
            await _context.SaveChangesAsync();

            return Ok("Inscription réussie !");
        }

        [HttpPost("login")]
        public IActionResult Login(LoginDto dto)
        {
            var user = _context.Utilisateurs.SingleOrDefault(u => u.Email == dto.Email);
            if (user == null || !BCrypt.Net.BCrypt.Verify(dto.Password, user.Password))
                return Unauthorized("Email ou mot de passe incorrect");

            var claims = new[]
            {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Role, user.Role.ToString())
        };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddMinutes(double.Parse(_config["Jwt:DurationInMinutes"])),
                signingCredentials: creds
            );

            return Ok(new { token = new JwtSecurityTokenHandler().WriteToken(token) });
        }
    }
}
