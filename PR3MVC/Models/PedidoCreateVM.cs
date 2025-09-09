using System.ComponentModel.DataAnnotations;

namespace Practica3Modelo.Models
{
    public class PedidoCreateVM
    {
        public PedidoModel Pedido { get; set; } = new PedidoModel();

        [Required(ErrorMessage = "Selecciona un producto")]
        public int ProductoId { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "La cantidad debe ser mayor a 0")]
        public int Cantidad { get; set; } = 1;
    }
}
