using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WAREHOUSE_MANAGEMENT_SYSTEM.Data;
using WAREHOUSE_MANAGEMENT_SYSTEM.Data.Models;
using WAREHOUSE_MANAGEMENT_SYSTEM.Models;
using WAREHOUSE_MANAGEMENT_SYSTEM.Services;
using System.IO;

namespace WAREHOUSE_MANAGEMENT_SYSTEM.Controllers
{
    public class ProductsController : Controller
    {
        private readonly ApplicationDbContext _context;
        

        public ProductsController(ApplicationDbContext context)
        {
            _context = context;
        }

       
        private static readonly string logFilePath = "Logs/HistoryLog.txt";

        // Action để hiển thị lịch sử
        public async Task<IActionResult> History()
        {
            if (!System.IO.File.Exists(logFilePath))
            {
                return View("History", new List<string> { "Không có dữ liệu lịch sử." });
            }

            var logs = await System.IO.File.ReadAllLinesAsync(logFilePath);
            return View("History", logs.ToList());
        }

        public IActionResult InventoryHistory(string search = "")
        {
            var allLogs = InventoryLogHelper.GetHistory(); // Thay đổi từ ReadAllLogs() sang GetHistory()

            if (!string.IsNullOrWhiteSpace(search))
            {
                allLogs = allLogs.Where(x =>
                    x.ProductName.Contains(search, StringComparison.OrdinalIgnoreCase) || // Sửa từ Product sang ProductName
                    x.Action.Contains(search, StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }

            return View(allLogs);
        }

        // GET: Products/MovementHistory/5
        public async Task<IActionResult> MovementHistory(Guid id)
        {
            var movements = await _context.StockMovements
                .Include(s => s.Product)
                .Where(s => s.ProductId == id)
                .OrderByDescending(s => s.MovementDate)
                .ToListAsync();

            if (!movements.Any())
            {
                return View("NoMovementHistory");
            }

            return View(movements);
        }


        // GET: Products
        [Authorize]
        /* public async Task<IActionResult> Index(string sortOrder, int pg = 1)
         {
             var applicationDbContext = _context.Products.Include(p => p.Category);


             var products = _context.Products.ToList();
             var categories = _context.Products.AsQueryable();

             ViewData["NameOrder"] = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
             switch (sortOrder)
             {
                 case "name_desc":
                     categories = categories.OrderByDescending(a => a.Category);
                     break;
                 default:
                     categories = categories.OrderBy(a => a.Category);
                     break;
             }
             const int pageSize = 5;
             if (pg < 1)
                 pg = 1;

             int recsCount = products.Count();

             var pager = new Pager(recsCount, pg, pageSize);

             int recSkip = (pg - 1) * pageSize;

             var data = applicationDbContext.Skip(recSkip).Take(pager.PageSize).ToList();

             this.ViewBag.Pager = pager;

             //return View(products);
             return View(data);

            // return View(await applicationDbContext.ToListAsync());
         }*/
        public async Task<IActionResult> Index(string sortOrder, int pg = 1)
        {
            var products = _context.Products.AsQueryable();

            sortOrder = string.IsNullOrEmpty(sortOrder) ? "name_asc" : sortOrder;
            pg = pg < 1 ? 1 : pg;
            const int pageSize = 5;
            products = SortProducts(products, sortOrder);
            var pager = new Pager(await products.CountAsync(), pg, pageSize);
            products = products.Skip((pager.CurrentPage - 1) * pager.PageSize).Take(pager.PageSize);

            ViewData["SortOrder"] = sortOrder;
            ViewData["Pager"] = pager;

            return View(await products.Include(p => p.Category).ToListAsync());
        }

        private IQueryable<Product> SortProducts(IQueryable<Product> products, string sortOrder)
        {
            switch (sortOrder)
            {
                case "name_desc":
                    return products.OrderByDescending(p => p.Category);
                case "category_asc":
                    return products.OrderBy(p => p.Category.Name);
                case "category_desc":
                    return products.OrderByDescending(p => p.Category.Name);
                default:
                    return products.OrderBy(p => p.Category);
            }
        }
        //GET: Products/SearchForm
        [Authorize]
        public async Task<IActionResult> SearchForm ()
        {
            return View();
        }
        //POST: Products/ShowSearchResults
        public async Task<IActionResult> ShowSearchResults(String SearchProduct)
        {
            return View("Index", await _context.Products.Where(  p =>  p.Name.StartsWith(SearchProduct))  .ToListAsync());
            
        }

        // GET: Products/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .Include(p => p.Category)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // GET: Products/Create
        public IActionResult Create() //FE Part Visualization
        {
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name");
            return View(); // returns localhost:44444/Products/Create
        }

        // POST: Products/Create
        [HttpPost]
        [ValidateAntiForgeryToken]  //BE Part 
        public async Task<IActionResult> Create([Bind("Name,Description,Count,Cost,Price,ImageUrl,CategoryId,Id")] Product product)
        {
            if (ModelState.IsValid)
            {
                product.Id = Guid.NewGuid();
                _context.Add(product);
                await _context.SaveChangesAsync();

                // Ghi vào file log
                LogHelper.WriteLog("Thêm", product.Id, product.Name, product.Count);

                return RedirectToAction(nameof(Index));

            }
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", product.CategoryId);
            return View(product);
        }

        // GET: Products/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", product.CategoryId);
            return View(product);
        }

        // POST: Products/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("Name,Description,Count,EntryDate,ExitDate,Cost,Price,ImageUrl,CategoryId,Id")] Product product)
        {
            if (id != product.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Lấy thông tin cũ trước khi cập nhật
                    var oldProduct = await _context.Products.AsNoTracking().FirstOrDefaultAsync(p => p.Id == id);
                    if (oldProduct == null)
                    {
                        return NotFound();
                    }

                    _context.Update(product);
                    await _context.SaveChangesAsync();

                    // Ghi log thay đổi
                    LogHelper.WriteLog("Sửa", product.Id, product.Name, product.Count);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductExists(product.Id))
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
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", product.CategoryId);
            return View(product);
        }

        // GET: Products/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .Include(p => p.Category)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // POST: Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product != null)
            {
                // Ghi log trước khi xóa
                LogHelper.WriteLog("Xóa", product.Id, product.Name, product.Count);

                _context.Products.Remove(product);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        private bool ProductExists(Guid id)
        {
            return _context.Products.Any(e => e.Id == id);
        }
    }
}
