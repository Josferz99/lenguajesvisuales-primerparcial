using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SupermercadoAPI.Data;
using SupermercadoAPI.DTOs;
using SupermercadoAPI.Models;
using System.ComponentModel.DataAnnotations;

namespace SupermercadoAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] // Todas las operaciones requieren autenticación
    public class UsuariosController : ControllerBase
    {
        private readonly SupermercadoContext _context;

        public UsuariosController(SupermercadoContext context)
        {
            _context = context;
        }

        // GET: api/usuarios
        [HttpGet]
        [Authorize(Roles = "Admin")] // Solo administradores pueden ver la lista
        public async Task<ActionResult<IEnumerable<UsuarioDto>>> GetUsuarios()
        {
            try
            {
                var usuarios = await _context.Usuarios
                    .Where(u => u.Activo)
                    .Select(u => new UsuarioDto
                    {
                        Id = u.Id,
                        Nombre = u.Nombre,
                        Email = u.Email,
                        Rol = u.Rol,
                        FechaCreacion = u.FechaCreacion,
                        Activo = u.Activo
                    })
                    .OrderBy(u => u.Nombre)
                    .ToListAsync();

                return Ok(usuarios);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error interno del servidor", detail = ex.Message });
            }
        }

        // GET: api/usuarios/5
        [HttpGet("{id}")]
        public async Task<ActionResult<UsuarioDto>> GetUsuario(int id)
        {
            try
            {
                // Los usuarios solo pueden ver su propio perfil, los admins pueden ver cualquiera
                var currentUserId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? "0");
                var currentUserRole = User.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value;

                if (currentUserRole != "Admin" && currentUserId != id)
                {
                    return Forbid(); // 403 Forbidden
                }

                var usuario = await _context.Usuarios
                    .Where(u => u.Id == id && u.Activo)
                    .Select(u => new UsuarioDto
                    {
                        Id = u.Id,
                        Nombre = u.Nombre,
                        Email = u.Email,
                        Rol = u.Rol,
                        FechaCreacion = u.FechaCreacion,
                        Activo = u.Activo
                    })
                    .FirstOrDefaultAsync();

                if (usuario == null)
                {
                    return NotFound(new { message = "Usuario no encontrado" });
                }

                return Ok(usuario);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error interno del servidor", detail = ex.Message });
            }
        }

        // POST: api/usuarios
        [HttpPost]
        [Authorize(Roles = "Admin")] // Solo admins pueden crear usuarios
        public async Task<ActionResult<UsuarioDto>> PostUsuario(UsuarioCreateDto usuarioDto)
        {
            try
            {
                // Verificar si el email ya existe
                if (await _context.Usuarios.AnyAsync(u => u.Email == usuarioDto.Email))
                {
                    return BadRequest(new { message = "El email ya está registrado" });
                }

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

                return CreatedAtAction(nameof(GetUsuario), new { id = usuario.Id }, usuarioResponse);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error interno del servidor", detail = ex.Message });
            }
        }

        // PUT: api/usuarios/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUsuario(int id, UsuarioUpdateDto usuarioDto)
        {
            try
            {
                // Los usuarios solo pueden actualizar su propio perfil, los admins pueden actualizar cualquiera
                var currentUserId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? "0");
                var currentUserRole = User.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value;

                if (currentUserRole != "Admin" && currentUserId != id)
                {
                    return Forbid(); // 403 Forbidden
                }

                var usuario = await _context.Usuarios.FindAsync(id);
                if (usuario == null)
                {
                    return NotFound(new { message = "Usuario no encontrado" });
                }

                // Verificar si ya existe otro usuario con el mismo email
                if (await _context.Usuarios.AnyAsync(u => u.Email == usuarioDto.Email && u.Id != id))
                {
                    return BadRequest(new { message = "Ya existe otro usuario con ese email" });
                }

                // Solo los admins pueden cambiar roles
                if (currentUserRole != "Admin" && usuario.Rol != usuarioDto.Rol)
                {
                    return BadRequest(new { message = "No tienes permisos para cambiar el rol" });
                }

                usuario.Nombre = usuarioDto.Nombre;
                usuario.Email = usuarioDto.Email;
                usuario.Rol = usuarioDto.Rol;
                usuario.Activo = usuarioDto.Activo;

                await _context.SaveChangesAsync();
                return Ok(new { message = "Usuario actualizado correctamente" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error interno del servidor", detail = ex.Message });
            }
        }

        // DELETE: api/usuarios/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")] // Solo administradores
        public async Task<IActionResult> DeleteUsuario(int id)
        {
            try
            {
                var usuario = await _context.Usuarios.FindAsync(id);
                if (usuario == null)
                {
                    return NotFound(new { message = "Usuario no encontrado" });
                }

                // No permitir eliminar el último administrador
                if (usuario.Rol == "Admin")
                {
                    var cantidadAdmins = await _context.Usuarios.CountAsync(u => u.Rol == "Admin" && u.Activo);
                    if (cantidadAdmins <= 1)
                    {
                        return BadRequest(new { message = "No se puede eliminar el último administrador del sistema" });
                    }
                }

                // Eliminación lógica
                usuario.Activo = false;
                await _context.SaveChangesAsync();

                return Ok(new { message = "Usuario eliminado correctamente" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error interno del servidor", detail = ex.Message });
            }
        }

        // PUT: api/usuarios/5/cambiar-password
        [HttpPut("{id}/cambiar-password")]
        public async Task<IActionResult> CambiarPassword(int id, [FromBody] CambiarPasswordDto passwordDto)
        {
            try
            {
                // Los usuarios solo pueden cambiar su propia contraseña, los admins pueden cambiar cualquiera
                var currentUserId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? "0");
                var currentUserRole = User.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value;

                if (currentUserRole != "Admin" && currentUserId != id)
                {
                    return Forbid();
                }

                var usuario = await _context.Usuarios.FindAsync(id);
                if (usuario == null)
                {
                    return NotFound(new { message = "Usuario no encontrado" });
                }

                // Si no es admin, verificar la contraseña actual
                if (currentUserRole != "Admin")
                {
                    if (!BCrypt.Net.BCrypt.Verify(passwordDto.PasswordActual, usuario.Password))
                    {
                        return BadRequest(new { message = "La contraseña actual es incorrecta" });
                    }
                }

                usuario.Password = BCrypt.Net.BCrypt.HashPassword(passwordDto.PasswordNueva);
                await _context.SaveChangesAsync();

                return Ok(new { message = "Contraseña actualizada correctamente" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error interno del servidor", detail = ex.Message });
            }
        }
    }

    // DTO para cambiar contraseña
    public class CambiarPasswordDto
    {
        public string PasswordActual { get; set; } = string.Empty;

        [Required]
        [MinLength(6)]
        public string PasswordNueva { get; set; } = string.Empty;
    }
}