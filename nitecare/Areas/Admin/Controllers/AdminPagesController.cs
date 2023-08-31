using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using nitecare.BaseModel;
using nitecare.Helpper;
using nitecare.Model;

namespace nitecare.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class AdminPagesController : Controller
    {
        private readonly nitecareContext _context;
        private readonly IWebHostEnvironment _hostEnvironment;
        public AdminPagesController(nitecareContext context, IWebHostEnvironment hostEnvironment)
        {
            _context = context;
            this._hostEnvironment = hostEnvironment;
        }

        [HttpGet]
        [AdminAuthentication]
        // GET: Admin/AdminPages
        public async Task<IActionResult> Index()
        {
            return View(await _context.Pages.ToListAsync());
        }

        [HttpGet]
        [AdminAuthentication]
        // GET: Admin/AdminPages/Create
        public IActionResult Create()
        {
            var pageName = new List<SelectListItem>()
            {
                new SelectListItem { Value = "Cẩm nang", Text = "Cẩm nang" },
                new SelectListItem { Value = "Sống khỏe", Text = "Sống khỏe" },
                new SelectListItem { Value = "Nhà cửa và đời sống", Text = "Nhà cửa và đời sống" },
                new SelectListItem { Value = "FAQ's", Text = "FAQ's" }
            };

            ViewData["PageName"] = new SelectList(pageName, "Value", "Text");
            return View();
        }

        // POST: Admin/AdminPages/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [AdminAuthentication]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(PageVm pageVm)
        {
            List<SelectListItem> pageName = new List<SelectListItem>()
            {         
                new SelectListItem { Value = "Cẩm nang", Text = "Cẩm nang" },
                new SelectListItem { Value = "Sống khỏe", Text = "Sống khỏe" },
                new SelectListItem { Value = "Nhà cửa và đời sống", Text = "Nhà cửa và đời sống" },
                new SelectListItem { Value = "FAQ's", Text = "FAQ's" }
            };
            ViewData["PageName"] = new SelectList(pageName, "Value", "Text");
            var page = new Page();
            page.PageId = pageVm.PageId;
            page.PageName = pageVm.PageName;
            page.Title = pageVm.Title;
            page.Description = pageVm.Description;
            page.PageContent = pageVm.PageContent;
            page.PageImagePath = ProcessUploadedFile(pageVm);
            _context.Add(page);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        [AdminAuthentication]
        // GET: Admin/AdminPages/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            List<SelectListItem> pageName = new List<SelectListItem>()
            {
                new SelectListItem { Value = "Cẩm nang", Text = "Cẩm nang" },
                new SelectListItem { Value = "Sống khỏe", Text = "Sống khỏe" },
                new SelectListItem { Value = "Nhà cửa và đời sống", Text = "Nhà cửa và đời sống" },
                new SelectListItem { Value = "FAQ's", Text = "FAQ's" }
            };
            ViewData["PageName"] = new SelectList(pageName, "Value", "Text");
            var page = await _context.Pages.AsNoTracking().SingleOrDefaultAsync(x => x.PageId == id);
            var pageVm = new PageVm();
            pageVm.PageId = page.PageId;
            pageVm.PageName = page.PageName;
            pageVm.Title = page.Title;
            pageVm.Description = page.Description;
            pageVm.PageContent = page.PageContent;
            pageVm.ImageName = page.PageImagePath;

            if (page == null)
            {
                return NotFound();
            }
            return View(pageVm);
        }

        // POST: Admin/AdminPages/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [AdminAuthentication]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, PageVm pageVm)
        {
            if (id != pageVm.PageId)
            {
                return NotFound();
            }

            try
            {
                List<SelectListItem> pageName = new List<SelectListItem>()
            {
                new SelectListItem { Value = "Cẩm nang", Text = "Cẩm nang" },
                new SelectListItem { Value = "Sống khỏe", Text = "Sống khỏe" },
                new SelectListItem { Value = "Nhà cửa và đời sống", Text = "Nhà cửa và đời sống" },
                new SelectListItem { Value = "FAQ's", Text = "FAQ's" }
            };

                ViewData["PageName"] = new SelectList(pageName, "Value", "Text");
                var page = _context.Pages.FirstOrDefault(x => x.PageId == pageVm.PageId);
                page.PageName = pageVm.PageName;
                page.Title = pageVm.Title;
                page.PageContent = pageVm.PageContent;
                page.Description = pageVm.Description;
                if (pageVm.ImageFile != null)
                {
                    string filePath = Path.Combine(_hostEnvironment.WebRootPath, "news", page.PageImagePath);
                    System.IO.File.Delete(filePath);
                    page.PageImagePath = ProcessUploadedFile(pageVm);
                }

                _context.Pages.Update(page);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PageExists(pageVm.PageId))
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

        [HttpGet]
        [AdminAuthentication]
        // GET: Admin/AdminPages/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var page = await _context.Pages
                .FirstOrDefaultAsync(m => m.PageId == id);
            if (page == null)
            {
                return NotFound();
            }

            return View(page);
        }

        // POST: Admin/AdminPages/Delete/5
        [HttpPost, ActionName("Delete")]
        [AdminAuthentication]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var page = await _context.Pages.FindAsync(id);
            var CurrentImage = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\news", page.PageImagePath);
            _context.Pages.Remove(page);
            if (await _context.SaveChangesAsync() > 0)
            {
                if (System.IO.File.Exists(CurrentImage))
                {
                    System.IO.File.Delete(CurrentImage);
                }
            }
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PageExists(int id)
        {
            return _context.Pages.Any(e => e.PageId == id);
        }
        private string ProcessUploadedFile(PageVm pageVm)
        {
            if (pageVm.ImageFile != null)
            {
                string uploadsFolder = Path.Combine(_hostEnvironment.WebRootPath, "news");
                pageVm.ImageName = Guid.NewGuid().ToString() + "_" + pageVm.ImageFile.FileName;
                string filePath = Path.Combine(uploadsFolder, pageVm.ImageName);
                using var fileStream = new FileStream(filePath, FileMode.Create);
                pageVm.ImageFile.CopyTo(fileStream);
            }

            return pageVm.ImageName;
        }
    }
}
