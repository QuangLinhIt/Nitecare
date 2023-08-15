using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace nitecare.BaseModel
{
    public class ProductVm
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public decimal Price { get; set; }
        public decimal? OriginalPrice { get; set; }
        public int Stock { get; set; }
        public decimal? Voucher { get; set; }
        public string ImageName { get; set; }
        public IFormFile ImageFile { get; set; }
        public List<SelectListItem> ListCategory { get; set; }
        public int[] CategoryIds { get; set; }
    }
}
