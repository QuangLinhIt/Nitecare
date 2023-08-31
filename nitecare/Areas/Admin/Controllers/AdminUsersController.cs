using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Threading.Tasks;
using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.EntityFrameworkCore;
using nitecare.Model;
using PagedList.Core;
using nitecare.Helpper;

namespace nitecare.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class AdminUsersController : Controller
    {
        private readonly nitecareContext _context;
        public INotyfService _notyfService { get; }

        public AdminUsersController(nitecareContext context, INotyfService notyfService)
        {
            _context = context;
            _notyfService = notyfService;
        }

        [HttpGet]
        [AdminAuthentication]
        // GET: Admin/AdminUsers
        public IActionResult Index(int page=1,int RoleId=0)
        {
            ViewData["Quyentruycap"] = new SelectList(_context.Roles, "RoleId", "RoleName", RoleId);
            ViewBag.CurrrentRoleId = RoleId;
            var pageNumber = page;
            var pageSize = 10;
            var ListUser = new List<User>();
            if (RoleId != 0)
            {
               ListUser = _context.Users
                .AsNoTracking()
                .Where(x=>x.RoleId==RoleId)
                .Include(x=>x.Role)
                .OrderByDescending(x => x.UserId)
                .ToList();
            }
            else
            {
                ListUser = _context.Users
                .AsNoTracking()
                .Include(x => x.Role)
                .OrderByDescending(x => x.UserId)
                .ToList();
            }
            
            PagedList<User> models = new PagedList<User>(ListUser.AsQueryable(), pageNumber, pageSize);
            ViewBag.CurrentPage = pageNumber;
            return View(models);
        }
        public IActionResult Filtter(int RoleId = 0)
        {
            var url = $"/Admin/AdminUsers?RoleId={RoleId}";
            if (RoleId == 0)
                url = $"/Admin/AdminUsers";
            return Json(new { status = "success", redirectUrl = url });
        }

        [HttpGet]
        [AdminAuthentication]
        // GET: Admin/AdminUsers/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.Users.FindAsync(id);
            
            if (user == null)
            {
                return NotFound();
            }
            ViewData["RoleId"] = new SelectList(_context.Roles, "RoleId", "RoleName", user.RoleId);
            return View(user);
        }

        // POST: Admin/AdminUsers/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [AdminAuthentication]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id,  User user)
        {
            if (id != user.UserId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.UpdateRange(user);
                    await _context.SaveChangesAsync();
                    _notyfService.Success("Chỉnh sửa người dùng thành công");
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UserExists(user.UserId))
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
            ViewData["RoleId"] = new SelectList(_context.Roles, "RoleId", "RoleName", user.RoleId);
            return View(user);
        }
        
        [HttpGet]
        [AdminAuthentication]
        public async Task<IActionResult> ForgotPassword(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user =await _context.Users.FindAsync(id);

            if (user == null)
            {
                return NotFound();
            }
            return View(user);
        }
        [HttpPost]
        [AdminAuthentication]
        [ValidateAntiForgeryToken]
        public IActionResult ForgotPassword(int id,User user)
        {
            if (id != user.UserId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    user.Password = HashPassword.Hash(user.Password);
                    _context.UpdateRange(user);
                    _context.SaveChangesAsync();
                    _notyfService.Success("Chỉnh sửa người dùng thành công");
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UserExists(user.UserId))
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
            return View(user);
        }

        [HttpGet]
        [AdminAuthentication]
        // GET: Admin/AdminUsers/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.Users
                .Include(u => u.Role)
                .FirstOrDefaultAsync(m => m.UserId == id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // POST: Admin/AdminUsers/Delete/5
        [HttpPost, ActionName("Delete")]
        [AdminAuthentication]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var user = await _context.Users.FindAsync(id);
            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            _notyfService.Success("Xóa người dùng thành công");
            return RedirectToAction(nameof(Index));
        }

        private bool UserExists(int id)
        {
            return _context.Users.Any(e => e.UserId == id);
        }
    }
}
