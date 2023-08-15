using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using nitecare.Model;
using nitecare.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace nitecare.Controllers
{
    public class OrderController : Controller
    {
        private readonly nitecareContext _context;
        public OrderController(nitecareContext context)
        {
            _context = context;
        }
        [HttpGet]
        [Route("cart")]
        public IActionResult Index()
        {
            return View();
        }
        [HttpGet]
        [Route("order")]
        public IActionResult Order()
        {
            var orderDto =new OrderDto();
            var listPayment = (from p in _context.Payments
                               select new Payment()
                               {
                                   PaymentId = p.PaymentId,
                                   PaymentName = p.PaymentName,
                                   PaymentContent = p.PaymentContent
                               }).ToList();
            ViewBag.Payment = listPayment;
            var email = HttpContext.Session.GetString("Email");
            var user = _context.Users.Where(x => x.Email == email).FirstOrDefault();
            var customer = _context.Customers.Where(x => x.Email == email).FirstOrDefault();
            if (email != null)
            {
                orderDto.CustomerId = customer.CustomerId;
                orderDto.City = customer.City;
                orderDto.District = customer.District;
                orderDto.Ward = customer.Ward;
                orderDto.Road = customer.Road;
                orderDto.Email = user.Email;
                orderDto.Name = user.UserName;
                orderDto.Phone = user.Phone;
            }
            return View(orderDto);
        }
        [HttpPost]
        [Route("order")]
        public IActionResult Order(OrderDto orderDto)
        {
            var listPayment = (from p in _context.Payments
                               select new Payment()
                               {
                                   PaymentId = p.PaymentId,
                                   PaymentName = p.PaymentName,
                                   PaymentContent = p.PaymentContent
                               }).ToList();
            ViewBag.Payment = listPayment;
            var email = HttpContext.Session.GetString("Email");
            var existingCustomer = _context.Customers.Where(x => x.Email == email).FirstOrDefault();
            if (existingCustomer != null)
            {
                existingCustomer.CustomerId = orderDto.CustomerId;
                existingCustomer.Email = orderDto.Email;
                existingCustomer.Phone = orderDto.Phone;
                existingCustomer.City = orderDto.City;
                existingCustomer.District = orderDto.District;
                existingCustomer.Ward = orderDto.Ward;
                existingCustomer.Name = orderDto.Name;
                _context.Customers.Update(existingCustomer);
                _context.SaveChanges();
            }
            else
            {
                var customer = new Customer();
                customer.CustomerId = orderDto.CustomerId;
                customer.Email = orderDto.Email;
                customer.Phone = orderDto.Phone;
                customer.City = orderDto.City;
                customer.District = orderDto.District;
                customer.Ward = orderDto.Ward;
                customer.Name = orderDto.Name;
                _context.Customers.Update(customer);
                _context.SaveChanges();
            }
            
            var order = new Order();
            order.OrderId = orderDto.OrderId;
            order.ShipDate = DateTime.Now.AddDays(1);
            order.PaymentId = orderDto.PaymentId;
            order.Total = orderDto.Total;
            order.Deleted = false;
            order.Status = "Chờ xác nhận";
            order.FeedbackId = null;
            order.CustomerId = orderDto.CustomerId;
            order.PaymentId = orderDto.PaymentId;
            var orderCarts = new List<OrderDetail>();
            foreach (var item in orderDto.ProductItems)
            {
                orderCarts.Add(new OrderDetail
                {
                    OrderId = orderDto.OrderId,
                    ProductId = item.ProductId,
                    Quantity = item.Quantity
                });
            }
            order.OrderDetails = orderCarts;
            _context.SaveChanges();
            _context.Orders.Add(order);
            _context.SaveChanges();
            return RedirectToAction("Index", "Order");
        }
    }
}
