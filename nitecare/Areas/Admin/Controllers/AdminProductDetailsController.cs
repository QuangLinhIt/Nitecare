using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using nitecare.Helpper;
using nitecare.Model;

namespace nitecare.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class AdminProductDetailsController : Controller
    {
        private readonly nitecareContext _context;

        public AdminProductDetailsController(nitecareContext context)
        {
            _context = context;
        }

        [HttpGet]
        [AdminAuthentication]
        // GET: Admin/AdminProductDetails
        public async Task<IActionResult> Index()
        {
            var nitecareContext = _context.ProductDetails.Include(p => p.Product);
            return View(await nitecareContext.ToListAsync());
        }

        [HttpGet]
        [AdminAuthentication]
        // GET: Admin/AdminProductDetails/Create
        public IActionResult Create()
        {
            ViewData["ProductId"] = new SelectList(_context.Products, "ProductId", "ProductName");
            return View();
        }

        // POST: Admin/AdminProductDetails/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [AdminAuthentication]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProductDetail productDetail)
        {
            if (ModelState.IsValid)
            {
                _context.Add(productDetail);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewData["ProductId"] = new SelectList(_context.Products, "ProductId", "ProductName", productDetail.ProductId);
            return View(productDetail);
        }

        [HttpGet]
        [AdminAuthentication]
        // GET: Admin/AdminProductDetails/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
           
            var productDetail = await _context.ProductDetails.FindAsync(id);
            if (productDetail == null)
            {
                return NotFound();
            }
            ViewData["ProductId"] = new SelectList(_context.Products, "ProductId", "ProductName", productDetail.ProductId);
            return View(productDetail);
        }

        // POST: Admin/AdminProductDetails/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [AdminAuthentication]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ProductDetail productDetail)
        {
            if (id != productDetail.ProductDetailId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                   
                    _context.Update(productDetail);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductDetailExists(productDetail.ProductDetailId))
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
            ViewData["ProductId"] = new SelectList(_context.Products, "ProductId", "ProductName", productDetail.ProductId);
            return View(productDetail);
        }

        [HttpGet]
        [AdminAuthentication]
        // GET: Admin/AdminProductDetails/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var productDetail = await _context.ProductDetails
                .Include(p => p.Product)
                .FirstOrDefaultAsync(m => m.ProductDetailId == id);
            if (productDetail == null)
            {
                return NotFound();
            }

            return View(productDetail);
        }

        // POST: Admin/AdminProductDetails/Delete/5
        [HttpPost, ActionName("Delete")]
        [AdminAuthentication]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var productDetail = await _context.ProductDetails.FindAsync(id);
            _context.ProductDetails.Remove(productDetail);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProductDetailExists(int id)
        {
            return _context.ProductDetails.Any(e => e.ProductDetailId == id);
        }
    }
}
