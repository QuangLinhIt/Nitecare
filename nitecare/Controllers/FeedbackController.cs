using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using nitecare.Model;
using nitecare.ViewModels.Authentication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace nitecare.Controllers
{
    public class FeedbackController : Controller
    {
        private readonly nitecareContext _context;
        public FeedbackController(nitecareContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            var feedback = _context.Feedbacks.Include(x=>x.User).OrderByDescending(x => x.FeedbackId).ToList();
            return View(feedback);
        }
    }
}
