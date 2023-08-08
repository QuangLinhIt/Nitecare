using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using nitecare.Model;
using nitecare.ViewModels.Authentication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace nitecare.Controllers
{
    public class NewsController : Controller
    {
        private readonly nitecareContext _context;
        public NewsController(nitecareContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index()
        {
            return View(await _context.Pages.OrderByDescending(x=>x.PageId).ToListAsync());
        }


        public IActionResult Details(int id)
        {
            var page = _context.Pages.AsNoTracking().SingleOrDefault(x=>x.PageId==id);
            if (page == null)
            {
                return (RedirectToAction("Index"));
            }

            var pageList = (from a in _context.Pages
                             orderby a.PageId descending
                           select new Page
                           {
                               PageId=a.PageId,
                               Title = a.Title,
                               PageName = a.PageName,
                               PageImagePath = a.PageImagePath,
                               Description = a.Description
                           }).Take(4).ToList();

            ViewBag.PageList = pageList;
            return View(page);
        }
        public IActionResult GetListNews(string pagename)
        {
            var listPage = _context.Pages.Where(x => x.PageName == pagename).ToList();
            if (listPage == null)
            {
                return (RedirectToAction("Index"));
            }
            return View(listPage);
        }
    }
}
