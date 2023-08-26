using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace nitecare.ViewModels
{
    public class ManagerOrderDto
    {
        public int OrderId { get; set; }
        public string StatusOrder { get; set; }
        public DateTime ShipDate { get; set; }
        public string StatusPayment { get; set; }
        public decimal Total { get; set; }
    }
}
