using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using nitecare.BaseModel;
using nitecare.Model;

namespace nitecare.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class AdminOrdersController : Controller
    {
        private readonly nitecareContext _context;

        public AdminOrdersController(nitecareContext context)
        {
            _context = context;
        }

        // GET: Admin/AdminOrders
        public IActionResult Index()
        {
            var nitecareContext = _context.Orders.Include(o => o.Feedback).Include(o => o.Payment);
            var order = _context.Orders
                .Include(x => x.Customer)
                .Include(x => x.Payment)
                .Include(x => x.Feedback)
                .Include(x => x.OrderDetails)
                .ThenInclude(x => x.Product)
                .OrderByDescending(x=>x.OrderId)
                .ToList();
            return View(order);
        }

        // GET: Admin/AdminOrders/Edit/5
        public IActionResult Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var listCart = new List<CartVm>();
            var orderVm = new OrderVm();
            var order = _context.Orders
                                .Include(x => x.Customer)
                                .Include(x => x.OrderDetails)
                                .ThenInclude(x => x.Product)
                                .Where(x => x.OrderId == id)
                                .FirstOrDefault();
            foreach (var item in order.OrderDetails.ToList())
            {
                var cartVm = new CartVm()
                {
                    ProductId = item.ProductId,
                    ProductName = item.Product.ProductName,
                    ProductImage = item.Product.ProductImage,
                    Price = item.Product.Price,
                    OriginalPrice = item.Product.OriginalPrice,
                    Quantity = item.Quantity
                };
                listCart.Add(cartVm);
            };
            orderVm.OrderId = order.OrderId;
            orderVm.ShipDate = order.ShipDate;
            orderVm.PaymentId = order.PaymentId;
            orderVm.Deleted = order.Deleted;
            orderVm.Total = order.Total;
            orderVm.Status = order.Status;
            orderVm.FeedbackId = order.FeedbackId;
            orderVm.CustomerId = order.CustomerId;
            orderVm.Name = order.Customer.Name;
            orderVm.Email = order.Customer.Email;
            orderVm.Phone = order.Customer.Phone;
            orderVm.City = order.Customer.City;
            orderVm.District = order.Customer.District;
            orderVm.Ward = order.Customer.Ward;
            orderVm.Road = order.Customer.Road;
            orderVm.CartItems = listCart;
            ViewData["CustomerId"] = new SelectList(_context.Customers, "CustomerId", "Email", order.CustomerId);
            ViewData["FeedbackId"] = new SelectList(_context.Feedbacks, "FeedbackId", "FeedbackContent", order.FeedbackId);
            ViewData["PaymentId"] = new SelectList(_context.Payments, "PaymentId", "PaymentName", order.PaymentId);
            var product = (from p in _context.Products
                           orderby p.ProductId descending
                           select new Product()
                           {
                               ProductId = p.ProductId,
                               ProductName = p.ProductName,
                               ProductImage = p.ProductImage,
                               Price = p.Price,
                               OriginalPrice = p.OriginalPrice,
                               Voucher = p.Voucher
                           }).ToList();
            ViewBag.ListProduct = product;
            var status = new List<SelectListItem>()
            {
                new SelectListItem { Value = "Chờ xác nhận", Text = "Chờ xác nhận" },
                new SelectListItem { Value = "Đang giao hàng", Text = "Đang giao hàng" },
                new SelectListItem { Value = "Giao hàng thành công", Text = "Giao hàng thành công" },
            };

            ViewData["Status"] = new SelectList(status, "Value", "Text");

            return View(orderVm);
        }

        // POST: Admin/AdminOrders/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, OrderVm orderVm)
        {
            if (id != orderVm.OrderId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // data productid + quantity
                    var product = (from p in _context.Products
                                   orderby p.ProductId descending
                                   select new Product()
                                   {
                                       ProductId = p.ProductId,
                                       ProductImage = p.ProductImage,
                                       ProductName = p.ProductName,
                                       Price = p.Price,
                                       OriginalPrice = p.OriginalPrice,
                                       Voucher = p.Voucher
                                   }).ToList();
                    ViewBag.ListProduct = product;
                    //find customer ->update
                    var customer = _context.Customers.Where(x => x.CustomerId == orderVm.CustomerId).FirstOrDefault();
                    customer.Name = orderVm.Name;
                    customer.Email = orderVm.Email;
                    customer.Phone = orderVm.Phone;
                    customer.City = orderVm.City;
                    customer.District = orderVm.District;
                    customer.Ward = orderVm.Ward;
                    customer.Road = orderVm.Road;
                    _context.Customers.Update(customer);

                    var listOrderDetail = new List<OrderDetail>();
                    //find order -> remove
                    var order = _context.Orders.Include(x => x.OrderDetails).FirstOrDefault(x => x.OrderId ==orderVm.OrderId );
                    order.OrderDetails.ToList().ForEach(result => listOrderDetail.Add(result));
                    _context.OrderDetails.RemoveRange(listOrderDetail);
                    await _context.SaveChangesAsync();

                    //update order
                    order.OrderId = orderVm.OrderId;
                    order.ShipDate = orderVm.ShipDate;
                    order.PaymentId = orderVm.PaymentId;
                    order.Deleted = orderVm.Deleted;
                    order.Status = orderVm.Status;
                    order.FeedbackId = orderVm.FeedbackId;
                    order.PaymentId = orderVm.PaymentId;
                    if (orderVm.CartItems.Count > 0)
                    {
                        decimal totalHtml = 0;
                        listOrderDetail = new List<OrderDetail>();
                        foreach (var item in orderVm.CartItems)
                        {
                            totalHtml += item.Price * item.Quantity;
                            listOrderDetail.Add(new OrderDetail()
                            {
                                OrderId = order.OrderId,
                                ProductId = item.ProductId,
                                Quantity = item.Quantity,
                            });
                        }
                        order.Total = totalHtml;
                        //update orderDetails
                        _context.OrderDetails.AddRange(listOrderDetail);
                        _context.SaveChanges();
                        _context.Orders.Update(order);
                        _context.SaveChanges();

                    }
                   
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!OrderExists(orderVm.OrderId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["CustomerId"] = new SelectList(_context.Customers, "CustomerId", "Email", orderVm.CustomerId);
            ViewData["FeedbackId"] = new SelectList(_context.Feedbacks, "FeedbackId", "FeedbackContent", orderVm.FeedbackId);
            ViewData["PaymentId"] = new SelectList(_context.Payments, "PaymentId", "PaymentName", orderVm.PaymentId);
            var status = new List<SelectListItem>()
            {
                new SelectListItem { Value = "Chờ xác nhận", Text = "Chờ xác nhận" },
                new SelectListItem { Value = "Đang giao hàng", Text = "Đang giao hàng" },
                new SelectListItem { Value = "Giao hàng thành công", Text = "Giao hàng thành công" },
            };

            ViewData["Status"] = new SelectList(status, "Value", "Text"); 
            return View(orderVm);
        }
        // GET: Admin/AdminOrders/Delete/5
        public IActionResult Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = _context.Orders.Include(x => x.OrderDetails).FirstOrDefault(x => x.OrderId == id);
            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        // POST: Admin/AdminOrders/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var orderDetail = _context.OrderDetails.Where(x => x.OrderId == id).ToList();
            _context.OrderDetails.RemoveRange(orderDetail);
            await _context.SaveChangesAsync();
            var order = _context.Orders.FirstOrDefault(x => x.OrderId == id);
            _context.Orders.Remove(order);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool OrderExists(int id)
        {
            return _context.Orders.Any(e => e.OrderId == id);
        }
    }
}
