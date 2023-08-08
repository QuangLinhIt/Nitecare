using Microsoft.AspNetCore.Mvc;
using nitecare.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace nitecare.Components
{
    public class CategoryComponent:ViewComponent
    {
        private readonly nitecareContext _context;
        public CategoryComponent(nitecareContext context)
        {
            _context = context;
        }
        public IViewComponentResult Invoke()
        {
            var category = _context.Categories.OrderByDescending(x => x.CategoryId).ToList();
            return View(category);
        }
    }
}
