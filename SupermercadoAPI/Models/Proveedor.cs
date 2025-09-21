using System.ComponentModel.DataAnnotations;

namespace SupermercadoAPI.Models
{
    public class Proveedor
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre del proveedor es obligatorio")]
        [StringLength(150, ErrorMessage = "El nombre no puede exceder 150 caracteres")]
        public string Nombre { get; set; } = string.Empty;

        [Phone(ErrorMessage = "Formato de teléfono inválido")]
        [StringLength(20, ErrorMessage = "El teléfono no puede exceder 20 caracteres")]
        public string? Telefono { get; set; }

        [EmailAddress(ErrorMessage = "Formato de email inválido")]
        [StringLength(150, ErrorMessage = "El email no puede exceder 150 caracteres")]
        public string? Email { get; set; }

        [StringLength(250, ErrorMessage = "La dirección no puede exceder 250 caracteres")]
        public string? Direccion { get; set; }

        public DateTime FechaCreacion { get; set; } = DateTime.Now;

        public bool Activo { get; set; } = true;

        // Propiedad de navegación - Un proveedor puede suministrar muchos productos
        public virtual ICollection<Producto> Productos { get; set; } = new List<Producto>();
    }
}