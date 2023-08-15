using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using nitecare.Model;
using nitecare.BaseModel;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using Microsoft.AspNetCore.Http;

namespace nitecare.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class AdminProductsController : Controller
    {
        private readonly nitecareContext _context;
        private readonly IWebHostEnvironment _hostEnvironment;
        public AdminProductsController(nitecareContext context, IWebHostEnvironment hostEnvironment)
        {
            _context = context;
            this._hostEnvironment = hostEnvironment;
        }

        // GET: Admin/AdminProducts
        public IActionResult Index()
        {
            var productList = _context.Products.ToList();
            return View(productList);
        }

        // GET: Admin/AdminProducts/Create
        public IActionResult Create()
        {
            var productVm = new ProductVm();
            var categoryIds = new List<int>();

            productVm.ListCategory = _context.Categories.Select(x => new SelectListItem { Text = x.CategoryName, Value = x.CategoryId.ToString() }).ToList();

            return View(productVm);
        }


        // POST: Admin/AdminProducts/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(ProductVm productVm)
        {
            var product = new Product();
            var productCategories = new List<ProductCategory>();

            product.ProductId = productVm.ProductId;
            product.ProductName = productVm.ProductName;
            product.Price = productVm.Price;
            product.OriginalPrice = productVm.OriginalPrice;
            product.Stock = productVm.Stock;
            product.Voucher = productVm.Voucher;
            product.ProductImage = ProcessUploadedFile(productVm);

            if (productVm.CategoryIds.Length > 0)
            {
                foreach (var categoryid in productVm.CategoryIds)
                {
                    productCategories.Add(new ProductCategory { ProductId = productVm.ProductId, CategoryId = categoryid });
                }
                product.ProductCategories = productCategories;
            }
            _context.SaveChanges();
            _context.Products.Add(product);
            _context.SaveChanges();

            return RedirectToAction(nameof(Index));
        }
        // GET: Admin/AdminProducts/Edit/5
        public IActionResult Edit(int? Id)
        {
            var productVm = new ProductVm();
            var categoryIds = new List<int>();
            if (Id.HasValue)
            {
                //get product
                var product = _context.Products.Include("ProductCategories").FirstOrDefault(x => x.ProductId == Id.Value);
                //get product categories and add each categoryId into categoryIds list
                product.ProductCategories.ToList().ForEach(result => categoryIds.Add(result.CategoryId));
                //bind productVm
                productVm.ListCategory = _context.Categories.Select(x => new SelectListItem { Text = x.CategoryName, Value = x.CategoryId.ToString() }).ToList();
                productVm.ProductId = product.ProductId;
                productVm.ProductName = product.ProductName;
                productVm.Price = product.Price;
                productVm.OriginalPrice = product.OriginalPrice;
                productVm.Stock = product.Stock;
                productVm.Voucher = product.Voucher;
                productVm.ImageName = product.ProductImage;
                productVm.CategoryIds = categoryIds.ToArray();
            }
            else
            {
                return RedirectToAction(nameof(Index));
            }
            return View(productVm);
        }

        // POST: Admin/AdminProducts/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, ProductVm productVm)
        {
            if (id != productVm.ProductId)
            {
                return NotFound();
            }

            var product = new Product();
            var productCategories = new List<ProductCategory>();

            //first find product categories list and then remove all from db
            product = _context.Products.Include("ProductCategories").FirstOrDefault(x => x.ProductId == productVm.ProductId); ;
            product.ProductCategories.ToList().ForEach(result => productCategories.Add(result));
            _context.ProductCategories.RemoveRange(productCategories);
            _context.SaveChanges();

            //now update product
            product.ProductId = productVm.ProductId;
            product.ProductName = productVm.ProductName;
            product.Price = productVm.Price;
            product.OriginalPrice = productVm.OriginalPrice;
            product.Stock = productVm.Stock;
            product.Voucher = productVm.Voucher;
            if (productVm.ImageFile != null)
            {
                string filePath = Path.Combine(_hostEnvironment.WebRootPath, "product", product.ProductImage);
                System.IO.File.Delete(filePath);
                product.ProductImage = ProcessUploadedFile(productVm);
            }

            if (productVm.CategoryIds.Length > 0)
            {
                productCategories = new List<ProductCategory>();
                foreach (var categoryid in productVm.CategoryIds)
                {
                    productCategories.Add(new ProductCategory { ProductId = productVm.ProductId, CategoryId = categoryid });
                }
                product.ProductCategories = productCategories;
            }
            _context.SaveChanges();

            return RedirectToAction(nameof(Index));
        }

        // GET: Admin/AdminProducts/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .FirstOrDefaultAsync(m => m.ProductId == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // POST: Admin/AdminProducts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var productCategories = _context.ProductCategories.Where(x => x.ProductId == id).ToList();
            _context.ProductCategories.RemoveRange(productCategories);
            await _context.SaveChangesAsync();
            var product = await _context.Products.FindAsync(id);
            //delete from wwwroot
            var CurrentImage = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\product", product.ProductImage);
            _context.Products.RemoveRange(product);
            if (await _context.SaveChangesAsync() > 0)
            {
                if (System.IO.File.Exists(CurrentImage))
                {
                    System.IO.File.Delete(CurrentImage);
                }
            }
            return RedirectToAction(nameof(Index));
        }

        private bool ProductExists(int id)
        {
            return _context.Products.Any(e => e.ProductId == id);
        }
        private string ProcessUploadedFile(ProductVm productVm)
        {
            if (productVm.ImageFile != null)
            {
                string uploadsFolder = Path.Combine(_hostEnvironment.WebRootPath, "product");
                productVm.ImageName = Guid.NewGuid().ToString() + "_" + productVm.ImageFile.FileName;
                string filePath = Path.Combine(uploadsFolder, productVm.ImageName);
                using var fileStream = new FileStream(filePath, FileMode.Create);
                productVm.ImageFile.CopyTo(fileStream);
            }

            return productVm.ImageName;
        }
    }
}
