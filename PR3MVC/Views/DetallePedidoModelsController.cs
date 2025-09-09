using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Practica3Modelo.Data;
using Practica3Modelo.Models;

namespace PR3MVC.Views
{
    public class DetallePedidoModelsController : Controller
    {
        private readonly ArtesaniasDbContext _context;

        public DetallePedidoModelsController(ArtesaniasDbContext context)
        {
            _context = context;
        }

        // GET: DetallePedidoModels
        public async Task<IActionResult> Index()
        {
            var artesaniasDbContext = _context.DetallesPedido.Include(d => d.Pedido).Include(d => d.Producto);
            return View(await artesaniasDbContext.ToListAsync());
        }

        // GET: DetallePedidoModels/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var detallePedidoModel = await _context.DetallesPedido
                .Include(d => d.Pedido)
                .Include(d => d.Producto)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (detallePedidoModel == null)
            {
                return NotFound();
            }

            return View(detallePedidoModel);
        }

        // GET: DetallePedidoModels/Create
        public IActionResult Create()
        {
            ViewData["PedidoId"] = new SelectList(_context.Pedidos, "Id", "Id");
            ViewData["ProductoId"] = new SelectList(_context.Productos, "Id", "Nombre");
            return View();
        }

        // POST: DetallePedidoModels/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,PedidoId,ProductoId,Cantidad,PrecioUnitario")] DetallePedidoModel detallePedidoModel)
        {
            if (ModelState.IsValid)
            {
                _context.Add(detallePedidoModel);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["PedidoId"] = new SelectList(_context.Pedidos, "Id", "Id", detallePedidoModel.PedidoId);
            ViewData["ProductoId"] = new SelectList(_context.Productos, "Id", "Nombre", detallePedidoModel.ProductoId);
            return View(detallePedidoModel);
        }

        // GET: DetallePedidoModels/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var detallePedidoModel = await _context.DetallesPedido.FindAsync(id);
            if (detallePedidoModel == null)
            {
                return NotFound();
            }
            ViewData["PedidoId"] = new SelectList(_context.Pedidos, "Id", "Id", detallePedidoModel.PedidoId);
            ViewData["ProductoId"] = new SelectList(_context.Productos, "Id", "Nombre", detallePedidoModel.ProductoId);
            return View(detallePedidoModel);
        }

        // POST: DetallePedidoModels/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,PedidoId,ProductoId,Cantidad,PrecioUnitario")] DetallePedidoModel detallePedidoModel)
        {
            if (id != detallePedidoModel.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(detallePedidoModel);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DetallePedidoModelExists(detallePedidoModel.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["PedidoId"] = new SelectList(_context.Pedidos, "Id", "Id", detallePedidoModel.PedidoId);
            ViewData["ProductoId"] = new SelectList(_context.Productos, "Id", "Nombre", detallePedidoModel.ProductoId);
            return View(detallePedidoModel);
        }

        // GET: DetallePedidoModels/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var detallePedidoModel = await _context.DetallesPedido
                .Include(d => d.Pedido)
                .Include(d => d.Producto)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (detallePedidoModel == null)
            {
                return NotFound();
            }

            return View(detallePedidoModel);
        }

        // POST: DetallePedidoModels/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var detallePedidoModel = await _context.DetallesPedido.FindAsync(id);
            if (detallePedidoModel != null)
            {
                _context.DetallesPedido.Remove(detallePedidoModel);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool DetallePedidoModelExists(int id)
        {
            return _context.DetallesPedido.Any(e => e.Id == id);
        }
    }
}
