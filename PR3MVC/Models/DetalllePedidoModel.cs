using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Practica3Modelo.Models;

public class DetallePedidoModel
{
    public int Id { get; set; }

    public int PedidoId { get; set; }
    public PedidoModel? Pedido { get; set; }

    public int ProductoId { get; set; }
    public ProductoModel? Producto { get; set; }

    public int Cantidad { get; set; }

    [Precision(10, 2)]
    public decimal PrecioUnitario { get; set; }

    [NotMapped]
    public decimal Subtotal => Math.Round(PrecioUnitario * Cantidad, 2);
}
