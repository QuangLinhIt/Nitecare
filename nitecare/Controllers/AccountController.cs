using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using nitecare.Helpper;
using nitecare.Model;
using nitecare.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace nitecare.Controllers
{
    public class AccountController : Controller
    {
        private readonly nitecareContext _context;
        public INotyfService _notyfService { get; }
        public AccountController(nitecareContext context, INotyfService notyfService)
        {
            _notyfService = notyfService;
            _context = context;
        }
        [HttpGet]
        [Route("login")]
        public IActionResult Login()
        {
            if (HttpContext.Session.GetString("Email") == null)
            {
                return View();
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }
        [HttpPost]
        [Route("login")]
        public IActionResult Login(UserDto userDto)
        {
            if (HttpContext.Session.GetString("Email") == null)
            {
                var user = _context.Users.Where(x => x.Email == userDto.Email).FirstOrDefault();
                if (user != null)
                {
                    var result = HashPassword.Verify(userDto.Password, user.Password);
                    if (result == true)
                    {
                        HttpContext.Session.SetString("Email", user.Email.ToString());
                        _notyfService.Success("Đăng nhập thành công");
                        return RedirectToAction("Index", "Home");
                    }
                    else
                    {
                        _notyfService.Error("Mật khẩu không đúng!");
                    }
                }
                else
                {
                    _notyfService.Error("Email không hợp lệ!");
                    return View(userDto);
                }
            }
            return View(userDto);
        }


        [HttpGet]
        [Route("register")]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [Route("register")]
        public IActionResult Register(UserDto userDto)
        {
            if (ModelState.IsValid)
            {
                var checkEmail = _context.Users.Where(x => x.Email == userDto.Email).FirstOrDefault();
                if (checkEmail == null)
                {
                    var user = new User();
                    user.UserId = userDto.UserId;
                    user.UserName = userDto.UserName;
                    user.Email = userDto.Email;
                    user.Phone = userDto.Phone;
                    user.Password = HashPassword.Hash(userDto.Password);
                    user.CreateDate = DateTime.Now;
                    user.Active = true;
                    user.RoleId = 2;
                    _context.Users.Add(user);
                    _context.SaveChanges();
                    return RedirectToAction("Login", "Account");
                }

            }
            return View(userDto);
        }
        [HttpGet]
        [Route("forgot")]
        public IActionResult Forgot()
        {
            return View();
        }
        [HttpPost]
        [Route("forgot")]
        public IActionResult Forgot(UserDto userDto)
        {
            return View();
        }
    }
}
