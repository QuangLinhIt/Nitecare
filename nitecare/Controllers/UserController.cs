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
    public class UserController : Controller
    {
        private readonly nitecareContext _context;
        public INotyfService _notyfService { get; }
        public UserController(nitecareContext context,INotyfService notyfService)
        {
            _notyfService = notyfService;
            _context = context;
        }
        [HttpGet]
        [Route("user")]
        [Authentication]
        public IActionResult Index()
        {
            var email = HttpContext.Session.GetString("Email");
            var user = _context.Users.Where(x => x.Email == email).FirstOrDefault();
            var userDto = new UserDto();
            userDto.UserId = user.UserId;
            userDto.Email = email;
            userDto.UserName = user.UserName;
            userDto.Phone = user.Phone;
            return View(userDto);
        }
        [HttpPost]
        [Route("user")]
        [Authentication]
        public IActionResult Index(UserDto userDto)
        {
            //var email = HttpContext.Session.GetString("Email");
            var user = _context.Users.Where(x => x.Email == userDto.Email).FirstOrDefault();
            var check = HashPassword.Verify(userDto.Password, user.Password);
            if (check == true)
            {
                if (userDto.NewPassword == userDto.ConfirmNewPassword)
                {
                    user.Password = HashPassword.Hash(userDto.NewPassword);
                    _context.Users.Update(user);
                    _context.SaveChanges();
                    _notyfService.Success("Thay đổi mật khẩu thành công");
                    return RedirectToAction("Login", "Account");
                }
                else
                {
                    _notyfService.Error("Xác nhận mật khẩu không đúng");
                    return View(userDto);
                }
            }
            else
            {
                _notyfService.Error("Nhập sai mật khẩu!");
                return View(userDto);
            }
        }
        [Route("logout")]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            HttpContext.Session.Remove("Email");
            return RedirectToAction("Login", "Account");
        }
    }
   
}
