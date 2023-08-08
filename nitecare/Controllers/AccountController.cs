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
        public AccountController(nitecareContext context)
        {
            _context = context;
        }
       [HttpGet]
       [Route("login")]
        public IActionResult Login()
        {
                return View();
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
    }
}
