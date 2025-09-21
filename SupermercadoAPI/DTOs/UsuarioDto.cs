using System.ComponentModel.DataAnnotations;

namespace SupermercadoAPI.DTOs
{
    // Para crear usuarios
    public class UsuarioCreateDto
    {
        [Required(ErrorMessage = "El nombre es obligatorio")]
        [StringLength(100)]
        public string Nombre { get; set; } = string.Empty;

        [Required(ErrorMessage = "El email es obligatorio")]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "La contraseña es obligatoria")]
        [MinLength(6, ErrorMessage = "La contraseña debe tener al menos 6 caracteres")]
        public string Password { get; set; } = string.Empty;

        [Required]
        public string Rol { get; set; } = "Empleado";
    }

    // Para actualizar usuarios
    public class UsuarioUpdateDto
    {
        [Required]
        [StringLength(100)]
        public string Nombre { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string Rol { get; set; } = string.Empty;

        public bool Activo { get; set; } = true;
    }

    // Para respuestas (sin contraseña)
    public class UsuarioDto
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Rol { get; set; } = string.Empty;
        public DateTime FechaCreacion { get; set; }
        public bool Activo { get; set; }
    }

    // Para login
    public class LoginDto
    {
        [Required(ErrorMessage = "El email es obligatorio")]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "La contraseña es obligatoria")]
        public string Password { get; set; } = string.Empty;
    }

    // Respuesta del login
    public class LoginResponseDto
    {
        public string Token { get; set; } = string.Empty;
        public UsuarioDto Usuario { get; set; } = new UsuarioDto();
        public DateTime Expiration { get; set; }
    }
}