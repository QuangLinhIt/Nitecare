using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using nitecare.Model;
using nitecare.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace nitecare.Controllers
{
    public class ProductController : Controller
    {
        private readonly nitecareContext _context;
        public ProductController(nitecareContext context)
        {
            _context = context;
        }
        [HttpGet]
        [Route("product")]
        public IActionResult Index()
        {
            var product = (from p in _context.Products
                           orderby p.ProductId descending
                           select new Product()
                           {
                               ProductId = p.ProductId,
                               ProductName = p.ProductName,
                               Price = p.Price,
                               OriginalPrice = p.OriginalPrice,
                               Voucher = p.Voucher,
                               Stock = p.Stock,
                               ProductImage = p.ProductImage,
                           }).ToList();
            ViewBag.SanPham = product;
            var category = (from c in _context.Categories
                            orderby c.CategoryId descending
                            select new Category()
                            {
                                CategoryId = c.CategoryId,
                                CategoryName = c.CategoryName,
                                ParentId = c.ParentId,
                            }).ToList();
            ViewBag.DanhMuc = category;
            return View();
        }
        [HttpGet]
        [Route("product-category")]
        public IActionResult GetProductByCategory(int categoryId)
        {
            var category = (from c in _context.Categories
                            orderby c.CategoryId descending
                            select new Category()
                            {
                                CategoryId = c.CategoryId,
                                CategoryName = c.CategoryName,
                                ParentId = c.ParentId,
                            }).ToList();
            ViewBag.DanhMuc = category;
            var productDto = (from c in _context.Categories
                              join pc in _context.ProductCategories on c.CategoryId equals pc.CategoryId
                              join p in _context.Products on pc.ProductId equals p.ProductId
                              where c.CategoryId == categoryId
                              select new ProductDto()
                              {
                                  ProductId = p.ProductId,
                                  ProductName = p.ProductName,
                                  Price = p.Price,
                                  OriginalPrice = p.OriginalPrice,
                                  Voucher = p.Voucher,
                                  Stock = p.Stock,
                                  ProductImage = p.ProductImage,
                                  CategoryId = c.CategoryId,
                                  CategoryName = c.CategoryName,
                                  ParentId = c.ParentId,
                              }).OrderByDescending(x => x.ProductId).ToList();
            return View(productDto);
        }
        [HttpGet]
        [Route("product-detail")]
        public IActionResult GetDetailProduct(int productId)
        {
            var productList = (from p in _context.Products
                               orderby p.ProductId descending
                               select new ProductDto()
                               {
                                   ProductId = p.ProductId,
                                   ProductName = p.ProductName,
                                   Price = p.Price,
                                   OriginalPrice = p.OriginalPrice,
                                   Voucher = p.Voucher,
                                   Stock = p.Stock,
                                   ProductImage = p.ProductImage,
                               }).Take(4).ToList();
            ViewBag.ProductList = productList;
            var feedbackList = (from c in _context.Customers
                                join o in _context.Orders on c.OrderId equals o.OrderId
                                join f in _context.Feedbacks on o.FeedbackId equals f.FeedbackId
                                orderby f.FeedbackId descending
                                select new FeedbackDto()
                                {
                                    FeedbackId = f.FeedbackId,
                                    Name = c.Email,
                                    FeedbackContent = f.FeedbackContent,
                                    FeedbackImage = f.FeedbackImage,
                                    FeedbackAvatar = f.FeedbackAvatar
                                }).ToList();
            ViewBag.FeedBackList = feedbackList;
            var category = (from c in _context.Categories
                            orderby c.CategoryId descending
                            select new Category()
                            {
                                CategoryId = c.CategoryId,
                                CategoryName = c.CategoryName,
                                ParentId = c.ParentId,
                            }).ToList();
            ViewBag.DanhMuc = category;
            var productDetail = _context.ProductDetails.Include(x=>x.Product).Where(x => x.ProductId == productId).FirstOrDefault();
            return View(productDetail);
        }

        public JsonResult GetProductById(int productId)
        {
            var product = _context.Products.Where(x => x.ProductId == productId).FirstOrDefault();
            return Json(product);
        }
    }
}
