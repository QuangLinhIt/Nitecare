using System;
using System.Collections.Generic;

#nullable disable

namespace nitecare.Model
{
    public partial class Order
    {
        public Order()
        {
            OrderDetails = new HashSet<OrderDetail>();
        }

        public int OrderId { get; set; }
        public int CustomerId { get; set; }
        public DateTime ShipDate { get; set; }
        public int PaymentId { get; set; }
        public decimal? Voucher { get; set; }
        public decimal Total { get; set; }
        public bool Deleted { get; set; }

        public virtual Customer Customer { get; set; }
        public virtual Payment Payment { get; set; }
        public virtual ICollection<OrderDetail> OrderDetails { get; set; }
    }
}
