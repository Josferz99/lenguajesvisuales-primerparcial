using System.ComponentModel.DataAnnotations;

namespace SupermercadoAPI.DTOs
{
    // Para crear productos
    public class ProductoCreateDto
    {
        [Required(ErrorMessage = "El nombre es obligatorio")]
        [StringLength(200)]
        public string Nombre { get; set; } = string.Empty;

        [StringLength(500)]
        public string? Descripcion { get; set; }

        [Required(ErrorMessage = "El precio es obligatorio")]
        [Range(0.01, double.MaxValue, ErrorMessage = "El precio debe ser mayor a 0")]
        public decimal Precio { get; set; }

        [Required(ErrorMessage = "El stock es obligatorio")]
        [Range(0, int.MaxValue, ErrorMessage = "El stock no puede ser negativo")]
        public int Stock { get; set; }

        [Required(ErrorMessage = "La categoría es obligatoria")]
        public int CategoriaId { get; set; }

        [Required(ErrorMessage = "El proveedor es obligatorio")]
        public int ProveedorId { get; set; }

        public DateTime? FechaVencimiento { get; set; }
    }

    // Para actualizar productos
    public class ProductoUpdateDto
    {
        [Required]
        [StringLength(200)]
        public string Nombre { get; set; } = string.Empty;

        [StringLength(500)]
        public string? Descripcion { get; set; }

        [Required]
        [Range(0.01, double.MaxValue)]
        public decimal Precio { get; set; }

        [Required]
        [Range(0, int.MaxValue)]
        public int Stock { get; set; }

        [Required]
        public int CategoriaId { get; set; }

        [Required]
        public int ProveedorId { get; set; }

        public DateTime? FechaVencimiento { get; set; }

        public bool Activo { get; set; } = true;
    }

    // Para respuestas
    public class ProductoDto
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string? Descripcion { get; set; }
        public decimal Precio { get; set; }
        public int Stock { get; set; }
        public int CategoriaId { get; set; }
        public string CategoriaNombre { get; set; } = string.Empty;
        public int ProveedorId { get; set; }
        public string ProveedorNombre { get; set; } = string.Empty;
        public DateTime? FechaVencimiento { get; set; }
        public DateTime FechaCreacion { get; set; }
        public bool Activo { get; set; }
    }
}