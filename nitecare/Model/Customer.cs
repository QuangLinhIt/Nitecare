using System;
using System.Collections.Generic;

#nullable disable

namespace nitecare.Model
{
    public partial class Customer
    {
        public int CustomerId { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string City { get; set; }
        public string District { get; set; }
        public string Ward { get; set; }
        public string Road { get; set; }
        public int OrderId { get; set; }
        public string Name { get; set; }

        public virtual Order Order { get; set; }
    }
}
