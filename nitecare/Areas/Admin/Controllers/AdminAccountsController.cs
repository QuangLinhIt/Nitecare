using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using nitecare.BaseModel;
using nitecare.Helpper;
using nitecare.Model;

namespace nitecare.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class AdminAccountsController : Controller
    {
        private readonly nitecareContext _context;
        public INotyfService _notyfService { get; }
        public AdminAccountsController(nitecareContext context, INotyfService notyfService)
        {
            _notyfService = notyfService;
            _context = context;
        }

        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterVm registerVm)
        {
            if (ModelState.IsValid)
            {
                if (registerVm.Password == registerVm.ConfirmPassword)
                {
                    var email = _context.Users.FirstOrDefault(x => x.Email == registerVm.Email);
                    if (email == null)
                    {
                        var user = new User();
                        user.UserName = registerVm.LastName + " " + registerVm.FirstName;
                        user.Email = registerVm.Email;
                        user.CreateDate = DateTime.Today;
                        user.Password = HashPassword.Hash(registerVm.Password);
                        user.RoleId = 1;
                        user.Active = true;
                        user.Phone = "0";
                        _context.Add(user);
                        await _context.SaveChangesAsync();
                        _notyfService.Success("Tạo mới tài khoản thành công !");
                        return RedirectToAction(nameof(Login));
                    }
                    else
                    {
                        _notyfService.Error("Tài khoản email đã tồn tại...");
                        return View(registerVm);
                    }
                }
                else
                {
                    _notyfService.Error("Xác nhận mật khẩu không đúng...");
                    return View(registerVm);
                }

            }
            return View(registerVm);
        }

        public IActionResult Login()
        {
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Login(LoginVm loginVm)
        {
            if (HttpContext.Session.GetString("AdminEmail") == null)
            {
                var user = _context.Users.Where(x => x.Email == loginVm.Email).FirstOrDefault();
                if (user != null)
                {
                    var result = HashPassword.Verify(loginVm.Password, user.Password);
                    if (result)
                    {
                        if (user.RoleId == 1)
                        {
                            _notyfService.Success("Đăng nhập thành công");
                            HttpContext.Session.SetString("AdminEmail", loginVm.Email.ToString());
                            return RedirectToAction("Index", "Home");
                        }
                        else
                        {
                            _notyfService.Error("Tài khoản này không có quyền truy cập vào Admin!");
                            return View(loginVm);
                        }

                    }
                    else
                    {
                        _notyfService.Error("Đăng nhập thất bại...");
                        return View(loginVm);
                    }
                }
                else
                {
                    _notyfService.Error("Tài khoản email không đúng!");
                    return View(loginVm);
                }
            }
            return View(loginVm);
        }
        [HttpGet]
        [AdminAuthentication]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            HttpContext.Session.Remove("AdminEmail");
            return RedirectToAction("Login", "AdminAccounts");
        }
        private bool UserExists(int id)
        {
            return _context.Users.Any(e => e.UserId == id);
        }
    }
}
