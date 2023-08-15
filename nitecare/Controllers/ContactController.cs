using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using nitecare.Helpper;
using nitecare.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace nitecare.Controllers
{
    public class ContactController : Controller
    {
        private readonly nitecareContext _context;
        public ContactController(nitecareContext context)
        {
            _context = context;
        }

        [HttpGet]
        [Route("contact")]
        [Authentication]
        public IActionResult Index()
        {
            var email = HttpContext.Session.GetString("Email");
            var user = _context.Users.Where(x => x.Email == email).FirstOrDefault();
            var contact = new Contact()
            {
                Name = user.UserName,
                Phone = user.Phone,
                Email = user.Email
            };
            return View(contact);
        }
        [HttpPost]
        [Route("contact")]
        [Authentication]
        public IActionResult Index(Contact contact)
        {
            _context.Contacts.Update(contact);
            _context.SaveChanges();
            return RedirectToAction("Index", "Home");
        }
    }
}
