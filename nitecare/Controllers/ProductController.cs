using Microsoft.AspNetCore.Mvc;
using nitecare.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace nitecare.Controllers
{
    public class ProductController : Controller
    {
        private readonly nitecareContext _context;
        public  ProductController (nitecareContext context){
            _context = context;
            }
        [Route("product")]
        public IActionResult Index()
        {
            var product = _context.Products.OrderByDescending(x => x.ProductId).ToList();
            return View(product);
        }
    }
}
