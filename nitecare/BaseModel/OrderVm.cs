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
        public int CustomerId { get; set; }
        public DateTime ShipDate { get; set; }
        public int PaymentId { get; set; }
        public decimal? Voucher { get; set; }
        public bool Deleted { get; set; }
        public decimal Total { get; set; }
        public List<SelectListItem> ListProductItem { get; set; }
        public int[] ProductItemIds { get; set; }
    }
}
