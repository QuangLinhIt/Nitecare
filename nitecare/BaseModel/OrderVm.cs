using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace nitecare.BaseModel
{
    public class OrderVm
    {
        public int OrderId { get; set; }
        public DateTime ShipDate { get; set; }
        public int PaymentId { get; set; }
        public int? FeedbackId { get; set; }
        public int CustomerId { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string City { get; set; }
        public string District { get; set; }
        public string Ward { get; set; }
        public string Road { get; set; }
        public string Name { get; set; }
        public bool Deleted { get; set; }
        public string Status { get; set; }
        public List<CartVm> CartItems { get; set; }
        public decimal Total { get; set; }
    }
}
