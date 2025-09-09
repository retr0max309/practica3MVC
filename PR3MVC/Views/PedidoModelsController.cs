using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Practica3Modelo.Data;
using Practica3Modelo.Models;

namespace PR3MVC.Controllers
{
    public class PedidoModelsController : Controller
    {
        private readonly ArtesaniasDbContext _context;

        public PedidoModelsController(ArtesaniasDbContext context)
        {
            _context = context;
        }

       
        public async Task<IActionResult> Index()
        {
            var pedidos = _context.Pedidos
                .Include(p => p.Cliente)
                .OrderByDescending(p => p.Fecha);
            return View(await pedidos.ToListAsync());
        }

        
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var pedido = await _context.Pedidos
                .Include(p => p.Cliente)
                .Include(p => p.Detalles)
                    .ThenInclude(d => d.Producto)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (pedido == null) return NotFound();

            await RecalcularTotalAsync(pedido.Id);

            return View(pedido);
        }

        
        public async Task<IActionResult> Create()
        {
            var clientes = await _context.Clientes
                .Select(c => new { c.Id, Nombre = c.Nombres + " " + c.Apellidos })
                .ToListAsync();
            ViewData["ClienteId"] = new SelectList(clientes, "Id", "Nombre");

            var productos = await _context.Productos
                .Where(p => p.Activo && p.Stock > 0)
                .OrderBy(p => p.Nombre)
                .ToListAsync();
            ViewData["ProductoId"] = new SelectList(productos, "Id", "Nombre");

            return View(new PedidoCreateVM());
        }

     
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(PedidoCreateVM vm)
        {
            var prod = await _context.Productos.FindAsync(vm.ProductoId);

            if (prod is null)
                ModelState.AddModelError("ProductoId", "Producto no encontrado.");
            if (vm.Cantidad <= 0)
                ModelState.AddModelError("Cantidad", "La cantidad debe ser mayor a 0.");
            if (prod is not null && prod.Stock < vm.Cantidad)
                ModelState.AddModelError("", "Stock insuficiente para este producto.");

            if (!ModelState.IsValid)
            {
                var clientes = await _context.Clientes
                    .Select(c => new { c.Id, Nombre = c.Nombres + " " + c.Apellidos })
                    .ToListAsync();
                ViewData["ClienteId"] = new SelectList(clientes, "Id", "Nombre", vm.Pedido.ClienteId);

                var productos = await _context.Productos
                    .Where(p => p.Activo && p.Stock > 0)
                    .OrderBy(p => p.Nombre)
                    .ToListAsync();
                ViewData["ProductoId"] = new SelectList(productos, "Id", "Nombre", vm.ProductoId);

                return View(vm);
            }

            using var tx = await _context.Database.BeginTransactionAsync();
            try
            {
                var pedido = vm.Pedido;
                
                pedido.Total = prod!.Precio * vm.Cantidad;

                _context.Pedidos.Add(pedido);
                await _context.SaveChangesAsync();

                var detalle = new DetallePedidoModel
                {
                    PedidoId = pedido.Id,
                    ProductoId = prod.Id,
                    Cantidad = vm.Cantidad,
                    PrecioUnitario = prod.Precio
                };
                _context.DetallesPedido.Add(detalle);

                prod.Stock -= vm.Cantidad;

                await _context.SaveChangesAsync();
                await tx.CommitAsync();

                return RedirectToAction(nameof(Details), new { id = pedido.Id });
            }
            catch
            {
                await tx.RollbackAsync();
                throw;
            }
        }

        
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var pedido = await _context.Pedidos.FindAsync(id);
            if (pedido == null) return NotFound();

            var clientes = await _context.Clientes
                .Select(c => new { c.Id, Nombre = c.Nombres + " " + c.Apellidos })
                .ToListAsync();
            ViewData["ClienteId"] = new SelectList(clientes, "Id", "Nombre", pedido.ClienteId);

            return View(pedido);
        }

      
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,ClienteId,Fecha,Estado")] PedidoModel pedidoModel)
        {
            if (id != pedidoModel.Id) return NotFound();

            if (!ModelState.IsValid)
            {
                var clientes = await _context.Clientes
                    .Select(c => new { c.Id, Nombre = c.Nombres + " " + c.Apellidos })
                    .ToListAsync();
                ViewData["ClienteId"] = new SelectList(clientes, "Id", "Nombre", pedidoModel.ClienteId);
                return View(pedidoModel);
            }

            try
            {
               
                var pedidoDb = await _context.Pedidos.FindAsync(id);
                if (pedidoDb == null) return NotFound();

                pedidoDb.ClienteId = pedidoModel.ClienteId;
                pedidoDb.Fecha = pedidoModel.Fecha;
                pedidoDb.Estado = pedidoModel.Estado;

                await _context.SaveChangesAsync();

               
                await RecalcularTotalAsync(pedidoDb.Id);

                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PedidoModelExists(pedidoModel.Id)) return NotFound();
                throw;
            }
        }

        
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var pedido = await _context.Pedidos
                .Include(p => p.Cliente)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (pedido == null) return NotFound();

            return View(pedido);
        }

        
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            using var tx = await _context.Database.BeginTransactionAsync();
            try
            {
                var detalles = await _context.DetallesPedido
                    .Include(d => d.Producto)
                    .Where(d => d.PedidoId == id)
                    .ToListAsync();

                // Reponer stock
                foreach (var d in detalles)
                {
                    if (d.Producto != null)
                        d.Producto.Stock += d.Cantidad;
                }

                // Borrar detalles y pedido
                _context.DetallesPedido.RemoveRange(detalles);

                var pedido = await _context.Pedidos.FindAsync(id);
                if (pedido != null) _context.Pedidos.Remove(pedido);

                await _context.SaveChangesAsync();
                await tx.CommitAsync();

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                await tx.RollbackAsync();
                throw;
            }
        }

       
        private bool PedidoModelExists(int id) =>
            _context.Pedidos.Any(e => e.Id == id);

        private async Task RecalcularTotalAsync(int pedidoId)
        {
            var total = await _context.DetallesPedido
                .Where(d => d.PedidoId == pedidoId)
                .Select(d => d.PrecioUnitario * d.Cantidad)
                .SumAsync();

            var pedido = await _context.Pedidos.FindAsync(pedidoId);
            if (pedido != null)
            {
                pedido.Total = total;
                await _context.SaveChangesAsync();
            }
        }
    }
}
