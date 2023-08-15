using System;
using System.Collections.Generic;

#nullable disable

namespace nitecare.Model
{
    public partial class Order
    {
        public Order()
        {
            Customers = new HashSet<Customer>();
            OrderDetails = new HashSet<OrderDetail>();
        }

        public int OrderId { get; set; }
        public DateTime ShipDate { get; set; }
        public int PaymentId { get; set; }
        public decimal Total { get; set; }
        public bool Deleted { get; set; }
        public string Status { get; set; }
        public int? FeedbackId { get; set; }
        public int CustomerId { get; set; }

        public virtual Feedback Feedback { get; set; }
        public virtual Payment Payment { get; set; }
        public virtual ICollection<Customer> Customers { get; set; }
        public virtual ICollection<OrderDetail> OrderDetails { get; set; }
    }
}
