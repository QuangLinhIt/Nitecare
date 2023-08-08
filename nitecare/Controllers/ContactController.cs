using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace nitecare.Controllers
{
    public class ContactController : Controller
    {
        [Route("contact")]
        public IActionResult Index()
        {
            return View();
        }
    }
}
