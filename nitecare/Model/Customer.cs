using System;
using System.Collections.Generic;

#nullable disable

namespace nitecare.Model
{
    public partial class Customer
    {
        public Customer()
        {
            Orders = new HashSet<Order>();
        }

        public int CustomerId { get; set; }
        public string Address { get; set; }
        public DateTime? Birthday { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string CustomerName { get; set; }

        public virtual ICollection<Order> Orders { get; set; }
    }
}
