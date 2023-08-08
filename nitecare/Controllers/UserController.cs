using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace nitecare.Controllers
{
    public class UserController : Controller
    {
        [Route("user")]
        public IActionResult Index()
        {
            return View();
        }
    }
}
