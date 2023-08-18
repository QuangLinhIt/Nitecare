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
            var order = _context.Orders.Include(x => x.OrderDetails).FirstOrDefault(x => x.OrderId == id);
            foreach(var item in order.OrderDetails.ToList())
            {
                var cartVm = new CartVm()
                {
                    ProductId = item.ProductId,
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
            orderVm.CartItems = listCart;
            ViewData["CustomerId"] = new SelectList(_context.Customers, "CustomerId", "Email", order.CustomerId);
            ViewData["FeedbackId"] = new SelectList(_context.Feedbacks, "FeedbackId", "FeedbackContent", order.FeedbackId);
            ViewData["PaymentId"] = new SelectList(_context.Payments, "PaymentId", "PaymentContent", order.PaymentId);
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
                    var order = new Order();
                    var listOrderDetail = new List<OrderDetail>();
                    //find and remove
                    order = _context.Orders.Include(x => x.OrderDetails).FirstOrDefault(x => x.OrderId == id);
                    foreach(var item in order.OrderDetails.ToList())
                    {
                        var orderDetail = new OrderDetail()
                        {
                            OrderId = order.OrderId,
                            ProductId = item.ProductId,
                            Quantity = item.Quantity
                        };
                        listOrderDetail.Add(orderDetail);
                    }
                    _context.OrderDetails.RemoveRange(listOrderDetail);
                    await _context.SaveChangesAsync();

                    //update
                    order.OrderId = orderVm.OrderId;
                    order.ShipDate = orderVm.ShipDate;
                    order.PaymentId = orderVm.PaymentId;
                    order.Total = orderVm.Total;
                    order.Deleted = orderVm.Deleted;
                    order.Status = orderVm.Status;
                    order.FeedbackId = orderVm.FeedbackId;
                    order.PaymentId = orderVm.PaymentId;
                    if (orderVm.CartItems.Count > 0)
                    {
                        foreach(var item in orderVm.CartItems)
                        {
                            listOrderDetail.Add(new OrderDetail()
                            {
                                OrderId = order.OrderId,
                                ProductId=item.ProductId,
                                Quantity=item.Quantity
                            });
                        }
                        order.OrderDetails = listOrderDetail;
                    }
                    _context.SaveChanges();
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
            ViewData["PaymentId"] = new SelectList(_context.Payments, "PaymentId", "PaymentContent", orderVm.PaymentId);
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
