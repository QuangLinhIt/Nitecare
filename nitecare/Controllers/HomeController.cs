﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using nitecare.Model;
using nitecare.Models;
using nitecare.ViewModels;
using nitecare.ViewModels.Authentication;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace nitecare.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly nitecareContext _context;
        public HomeController(ILogger<HomeController> logger, nitecareContext context)
        {
            _context = context;
            _logger = logger;
        }
        public IActionResult Index()
        {
            var pageList = (from a in _context.Pages
                            orderby a.PageId descending
                            select new Page
                            {
                                PageId = a.PageId,
                                Title = a.Title,
                                PageName = a.PageName,
                                PageImagePath = a.PageImagePath,
                                Description = a.Description
                            }).Take(4).ToList();

            ViewBag.PageList = pageList;
            var productList = (from p in _context.Products
                               orderby p.ProductId descending
                               select new ProductDto()
                               {
                                   ProductId = p.ProductId,
                                   ProductName = p.ProductName,
                                   Price = p.Price,
                                   OriginalPrice = p.OriginalPrice,
                                   Voucher = p.Voucher,
                                   Stock = p.Stock,
                                   ProductImage = p.ProductImage,
                                   ShortDes = p.ShortDes,
                                   Description = p.Description
                               }).Take(4).ToList();
            ViewBag.ProductList = productList;
            var feedbackList = (from f in _context.Feedbacks
                                join u in _context.Users on f.UserId equals u.UserId
                                orderby f.FeedbackId descending
                                select new FeedbackDto()
                                {
                                    FeedbackId = f.FeedbackId,
                                    UserName = u.UserName,
                                    FeedbackContent = f.FeedbackContent,
                                    FeedbackImage = f.FeedbackImage,
                                    FeedbackAvatar = f.FeedbackAvatar
                                }).ToList();
            ViewBag.FeedBackList = feedbackList;
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
