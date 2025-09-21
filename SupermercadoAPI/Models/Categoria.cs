using System.ComponentModel.DataAnnotations;

namespace SupermercadoAPI.Models
{
    public class Categoria
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre de la categoría es obligatorio")]
        [StringLength(100, ErrorMessage = "El nombre no puede exceder 100 caracteres")]
        public string Nombre { get; set; } = string.Empty;

        [StringLength(300, ErrorMessage = "La descripción no puede exceder 300 caracteres")]
        public string? Descripcion { get; set; }

        public DateTime FechaCreacion { get; set; } = DateTime.Now;

        public bool Activa { get; set; } = true;

        // Propiedad de navegación - Una categoría puede tener muchos productos
        public virtual ICollection<Producto> Productos { get; set; } = new List<Producto>();
    }
}