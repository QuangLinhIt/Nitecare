using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using nitecare.Helpper;
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
        public INotyfService _notyfService { get; }

        public OrderController(nitecareContext context, INotyfService notyfService)
        {
            _notyfService = notyfService;
            _context = context;
        }
        [HttpGet]
        [Route("cart")]
        public IActionResult Cart()
        {
            return View();
        }
        [HttpGet]
        [Route("order")]
        public IActionResult SaveOrder()
        {
            var orderDto = new OrderDto();
            var listPayment = (from p in _context.Payments
                               select new Payment()
                               {
                                   PaymentId = p.PaymentId,
                                   PaymentName = p.PaymentName,
                                   PaymentContent = p.PaymentContent
                               }).ToList();
            ViewBag.ThanhToan = listPayment;
            var email = HttpContext.Session.GetString("Email");
            var user = _context.Users.Where(x => x.Email == email).FirstOrDefault();
            var customer = _context.Customers.Where(x => x.Email == email).FirstOrDefault();
            if (user != null)
            {
                orderDto.Email = user.Email;
                orderDto.Name = user.UserName;
                orderDto.Phone = user.Phone;
            }
            if (customer != null)
            {
                orderDto.City = customer.City;
                orderDto.District = customer.District;
                orderDto.Ward = customer.Ward;
                orderDto.Road = customer.Road;
            }
            return View(orderDto);
        }
        [HttpPost]
        [Route("order")]
        public IActionResult SaveOrder(OrderDto orderDto)
        {
            if (orderDto.ProductItems != null && orderDto.Email != null
                && orderDto.Phone != null && orderDto.Name != null
                && orderDto.City != null && orderDto.District != null
                && orderDto.Ward != null && orderDto.Road != null)
            {
                var listPayment = (from p in _context.Payments
                                   select new Payment()
                                   {
                                       PaymentId = p.PaymentId,
                                       PaymentName = p.PaymentName,
                                       PaymentContent = p.PaymentContent
                                   }).ToList();
                ViewBag.ThanhToan = listPayment;
                var selectedPaymentId = Request.Form["flexRadioDefault"];
                var email = HttpContext.Session.GetString("Email");
                var existingCustomer = _context.Customers.Where(x => x.Email == email).FirstOrDefault();
                var order = new Order();
                if (existingCustomer != null)
                {
                    existingCustomer.Email = orderDto.Email;
                    existingCustomer.Phone = orderDto.Phone;
                    existingCustomer.City = orderDto.City;
                    existingCustomer.District = orderDto.District;
                    existingCustomer.Ward = orderDto.Ward;
                    existingCustomer.Road = orderDto.Road;
                    existingCustomer.Name = orderDto.Name;
                    order.CustomerId = existingCustomer.CustomerId;
                    _context.Customers.Update(existingCustomer);
                    _context.SaveChanges();
                }
                else
                {
                    var customer = new Customer();
                    customer.Email = orderDto.Email;
                    customer.Phone = orderDto.Phone;
                    customer.City = orderDto.City;
                    customer.District = orderDto.District;
                    customer.Ward = orderDto.Ward;
                    customer.Road = orderDto.Road;
                    customer.Name = orderDto.Name;
                    _context.Customers.Add(customer);
                    _context.SaveChanges();
                    order.CustomerId = customer.CustomerId;
                }

                order.ShipDate = DateTime.Today;
                order.PaymentId = orderDto.PaymentId;
                order.Total = orderDto.Total;
                order.Deleted = false;
                order.Status = "Chờ xác nhận";
                order.FeedbackId = null;
                order.PaymentId = Convert.ToInt32(selectedPaymentId);
                var orderCarts = new List<OrderDetail>();

                foreach (var item in orderDto.ProductItems)
                {
                    orderCarts.Add(new OrderDetail
                    {
                        OrderId = order.OrderId,
                        ProductId = item.ProductId,
                        Quantity = item.Quantity
                    });
                }
                order.OrderDetails = orderCarts;
                _context.SaveChanges();
                _context.Orders.Add(order);
                _context.SaveChanges();
                _notyfService.Success("Đặt hàng thành công!");
                ViewBag.Success = "true";
                return RedirectToAction("CartDone", "Order",new { orderId=order.OrderId});
            }
            else
            {
                ViewBag.Success = "false";
                _notyfService.Warning("Vui lòng chọn sản phẩm");
                return RedirectToAction("Index", "Product");
            }
        }
        [HttpGet]
        [Route("cart-done")]
        public IActionResult CartDone(int orderId)
        {
            var order = _context.Orders.Include(x => x.Customer).Where(x => x.OrderId == orderId).FirstOrDefault();
            return View(order);
        }
        [Route("manager")]
        [Authentication]
        public IActionResult ManagerOrder()
        {
            var email = HttpContext.Session.GetString("Email");
            var orders = (from o in _context.Orders
                         join c in _context.Customers on o.CustomerId equals c.CustomerId
                         join p in _context.Payments on o.PaymentId equals p.PaymentId
                         where c.Email == email
                         select new ManagerOrderDto()
                         {
                             OrderId = o.OrderId,
                             ShipDate = o.ShipDate,
                             StatusOrder = o.Status,
                             StatusPayment = p.PaymentName,
                             Total=o.Total
                         }).ToList();
            return View(orders);
        }
    }
}
