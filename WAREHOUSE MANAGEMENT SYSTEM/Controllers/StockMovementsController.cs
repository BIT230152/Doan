using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WAREHOUSE_MANAGEMENT_SYSTEM.Data;
using WAREHOUSE_MANAGEMENT_SYSTEM.Data.Models;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Linq;
using WAREHOUSE_MANAGEMENT_SYSTEM.Services;
using System;

namespace WAREHOUSE_MANAGEMENT_SYSTEM.Controllers
{
    [Authorize]
    public class StockMovementsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public StockMovementsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: StockMovements
        public async Task<IActionResult> Index()
        {
            var movements = await _context.StockMovements
                .Include(s => s.Product)
                .OrderByDescending(s => s.MovementDate)
                .ToListAsync();
            return View(movements);
        }

        // GET: StockMovements/Create
        public IActionResult Create()
        {
            ViewData["ProductId"] = new SelectList(_context.Products, "Id", "Name");
            return View();
        }

        // POST: StockMovements/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ProductId,MovementType,Quantity,MovementDate,Country")] StockMovement movement)
        {
            if (ModelState.IsValid)
            {
                var product = await _context.Products.FindAsync(movement.ProductId);
                if (product == null)
                {
                    return NotFound();
                }

                // Cập nhật số lượng sản phẩm
                if (movement.MovementType == MovementType.Import)
                {
                    product.Count += movement.Quantity;
                }
                else // Export
                {
                    if (product.Count < movement.Quantity)
                    {
                        ModelState.AddModelError("Quantity", "Số lượng xuất vượt quá số lượng tồn kho");
                        ViewData["ProductId"] = new SelectList(_context.Products, "Id", "Name", movement.ProductId);
                        return View(movement);
                    }
                    product.Count -= movement.Quantity;
                }

                _context.Add(movement);
                await _context.SaveChangesAsync();

                // Ghi log
              

                InventoryLogHelper.LogMovement(movement, product); // Ghi log với Country
                return RedirectToAction("Index");

            }
            ViewData["ProductId"] = new SelectList(_context.Products, "Id", "Name", movement.ProductId);
            return View(movement);
        }
    }
}