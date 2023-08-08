using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using nitecare.Model;
using PagedList.Core;

namespace nitecare.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class AdminCategoriesController : Controller
    {
        private readonly nitecareContext _context;
        public INotyfService _notyfService { get; }
        public AdminCategoriesController(nitecareContext context,INotyfService notyfService)
        {
            _context = context;
            _notyfService = notyfService;
        }

        // GET: Admin/AdminCategories
        public IActionResult Index(int? page)
        {
            var pageNumber = page == null || page <= 0 ? 1 : page.Value;
            var pageSize = 10;
            var ListCustomer = _context.Categories
                .AsNoTracking()
                .OrderByDescending(x => x.CategoryId);
           var models = new PagedList<Category>(ListCustomer, pageNumber, pageSize);
            ViewBag.CurrentPage = pageNumber;
            return View(models);
        }

        // GET: Admin/AdminCategories/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Admin/AdminCategories/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Category category)
        {
            if (ModelState.IsValid)
            {
                
                _context.Add(category);
                await _context.SaveChangesAsync();
                _notyfService.Success("Tạo mới danh mục sản phẩm thành công");
                return RedirectToAction(nameof(Index));
            }
            return View(category);
        }

        // GET: Admin/AdminCategories/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var category = await _context.Categories.FindAsync(id);
            if (category == null)
            {
                _notyfService.Error("Không tìm thấy danh mục sản phẩm theo id");
                return NotFound();
            }
            return View(category);
        }

        // POST: Admin/AdminCategories/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, Category category)
        {
            if (id != category.CategoryId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Categories.Update(category);
                    _context.SaveChanges();
                   
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CategoryExists(category.CategoryId))
                    {
                        _notyfService.Error("Không tìm thấy danh mục sản phẩm theo id");
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(category);
        }

        // GET: Admin/AdminCategories/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var category = await _context.Categories
                .FirstOrDefaultAsync(m => m.CategoryId == id);
            if (category == null)
            {
                return NotFound();
            }

            return View(category);
        }

        // POST: Admin/AdminCategories/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var product = (from a in _context.Products
                           join b in _context.ProductCategories on a.ProductId equals b.ProductId
                           where b.CategoryId == id
                           select new Product
                           {
                               ProductId = a.ProductId
                           }
                         ).ToList();

            var productCategories = _context.ProductCategories.Where(x => x.CategoryId == id).ToList();

            var category = _context.Categories.Where(x => x.CategoryId == id).FirstOrDefault();
            if (category == null)
            {
                _notyfService.Error("Không tìm thấy danh mục sản phẩm theo id");
                return RedirectToAction(nameof(Index));
            }
            else
            {
                _context.ProductCategories.RemoveRange(productCategories);
                _context.SaveChanges();
                _context.Products.RemoveRange(product);
                _context.SaveChanges();
                _context.Categories.Remove(category);
                _context.SaveChanges();
                _notyfService.Success("Xóa danh mục sản phẩm thành công");
                return RedirectToAction(nameof(Index));
            }
           
        }

        private bool CategoryExists(int id)
        {
            return _context.Categories.Any(e => e.CategoryId == id);
        }
    }
}
