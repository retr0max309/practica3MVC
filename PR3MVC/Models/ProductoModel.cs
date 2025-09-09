using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace Practica3Modelo.Models
{
    public class ProductoModel
    {
        public int Id { get; set; }

        [Required, StringLength(120)]
        public string Nombre { get; set; } = null!;

        [StringLength(400)]
        public string? Descripcion { get; set; }

        [Precision(10, 2)]
        [Range(0, 9999999, ErrorMessage = "El precio debe ser mayor o igual a 0.")]
        public decimal Precio { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "El stock debe ser mayor o igual a 0.")]
        public int Stock { get; set; }

        public bool Activo { get; set; } = true;

        public ICollection<DetallePedidoModel> Detalles { get; set; } = new List<DetallePedidoModel>();
    }
}
