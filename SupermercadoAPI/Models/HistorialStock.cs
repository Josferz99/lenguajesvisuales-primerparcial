using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SupermercadoAPI.Models
{
    public class HistorialStock
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El producto es obligatorio")]
        public int ProductoId { get; set; }

        [Required(ErrorMessage = "El usuario es obligatorio")]
        public int UsuarioId { get; set; }

        [Required(ErrorMessage = "El tipo de movimiento es obligatorio")]
        [StringLength(20, ErrorMessage = "El tipo de movimiento no puede exceder 20 caracteres")]
        public string TipoMovimiento { get; set; } = string.Empty; // "Entrada" o "Salida"

        [Required(ErrorMessage = "La cantidad es obligatoria")]
        [Range(1, int.MaxValue, ErrorMessage = "La cantidad debe ser mayor a 0")]
        public int Cantidad { get; set; }

        public DateTime Fecha { get; set; } = DateTime.Now;

        [StringLength(300, ErrorMessage = "El motivo no puede exceder 300 caracteres")]
        public string? Motivo { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal? PrecioUnitario { get; set; } // Para registrar el precio al momento del movimiento

        // Propiedades de navegación
        public virtual Producto? Producto { get; set; }
        public virtual Usuario? Usuario { get; set; }
    }
}