using Microsoft.EntityFrameworkCore;

namespace Practica3Modelo.Models;

public enum EstadoPedido { Pendiente = 0, Pagado = 1, Enviado = 2, Entregado = 3, Cancelado = 4 }

public class PedidoModel
{
    public int Id { get; set; }

    public int ClienteId { get; set; }
    public ClienteModel? Cliente { get; set; }

    public DateTime Fecha { get; set; } = DateTime.UtcNow;

    [Precision(10, 2)]
    public decimal Total { get; set; }  // puedes calcularlo al guardar

    public EstadoPedido Estado { get; set; } = EstadoPedido.Pendiente;

    public ICollection<DetallePedidoModel> Detalles { get; set; } = new List<DetallePedidoModel>();
}
