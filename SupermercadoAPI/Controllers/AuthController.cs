using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using SupermercadoAPI.Data;
using SupermercadoAPI.DTOs;
using SupermercadoAPI.Models;

namespace SupermercadoAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly SupermercadoContext _context;
        private readonly IConfiguration _configuration;

        public AuthController(SupermercadoContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            try
            {
                // Buscar usuario por email
                var usuario = await _context.Usuarios
                    .FirstOrDefaultAsync(u => u.Email == loginDto.Email && u.Activo);

                if (usuario == null)
                {
                    return Unauthorized(new { message = "Email o contraseña incorrectos" });
                }

                // Verificar contraseña
                if (!BCrypt.Net.BCrypt.Verify(loginDto.Password, usuario.Password))
                {
                    return Unauthorized(new { message = "Email o contraseña incorrectos" });
                }

                // Generar token JWT
                var token = GenerarTokenJWT(usuario);

                var response = new LoginResponseDto
                {
                    Token = token,
                    Usuario = new UsuarioDto
                    {
                        Id = usuario.Id,
                        Nombre = usuario.Nombre,
                        Email = usuario.Email,
                        Rol = usuario.Rol,
                        FechaCreacion = usuario.FechaCreacion,
                        Activo = usuario.Activo
                    },
                    Expiration = DateTime.UtcNow.AddMinutes(60)
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error interno del servidor", detail = ex.Message });
            }
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] UsuarioCreateDto usuarioDto)
        {
            try
            {
                // Verificar si el email ya existe
                if (await _context.Usuarios.AnyAsync(u => u.Email == usuarioDto.Email))
                {
                    return BadRequest(new { message = "El email ya está registrado" });
                }

                // Crear nuevo usuario
                var usuario = new Usuario
                {
                    Nombre = usuarioDto.Nombre,
                    Email = usuarioDto.Email,
                    Password = BCrypt.Net.BCrypt.HashPassword(usuarioDto.Password),
                    Rol = usuarioDto.Rol,
                    FechaCreacion = DateTime.Now,
                    Activo = true
                };

                _context.Usuarios.Add(usuario);
                await _context.SaveChangesAsync();

                var usuarioResponse = new UsuarioDto
                {
                    Id = usuario.Id,
                    Nombre = usuario.Nombre,
                    Email = usuario.Email,
                    Rol = usuario.Rol,
                    FechaCreacion = usuario.FechaCreacion,
                    Activo = usuario.Activo
                };

                return CreatedAtAction(nameof(Register), new { id = usuario.Id }, usuarioResponse);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error interno del servidor", detail = ex.Message });
            }
        }

        private string GenerarTokenJWT(Usuario usuario)
        {
            var jwtSettings = _configuration.GetSection("Jwt");
            var key = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(jwtSettings["Key"]!));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
                new Claim(ClaimTypes.Name, usuario.Nombre),
                new Claim(ClaimTypes.Email, usuario.Email),
                new Claim(ClaimTypes.Role, usuario.Rol),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var token = new JwtSecurityToken(
                issuer: jwtSettings["Issuer"],
                audience: jwtSettings["Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(Convert.ToDouble(jwtSettings["ExpireMinutes"])),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}