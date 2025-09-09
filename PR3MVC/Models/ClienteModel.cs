using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace Practica3Modelo.Models;

[Index(nameof(CI), IsUnique = true)]
[Index(nameof(Correo), IsUnique = true)]
public class ClienteModel
{
    public int Id { get; set; }

    [Required, StringLength(100)]
    public string Nombres { get; set; } = null!;
    [Required, StringLength(100)]
    public string Apellidos { get; set; } = null!;
    [Required, StringLength(20)]
    public string CI { get; set; } = null!;
    [StringLength(20)]
    public string? Celular { get; set; }
    [EmailAddress, StringLength(120)]
    public string? Correo { get; set; }
    [StringLength(200)]
    public string? Direccion { get; set; }

    public ICollection<PedidoModel> Pedidos { get; set; } = new List<PedidoModel>();
}
