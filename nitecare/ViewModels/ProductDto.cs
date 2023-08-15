using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace nitecare.ViewModels
{
    public class ProductDto
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public decimal Price { get; set; }
        public decimal? OriginalPrice { get; set; }
        public decimal? Voucher { get; set; }
        public int Stock { get; set; }
        public string ProductImage { get; set; }
        public string ShortDes { get; set; }
        public string Description { get; set; }
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
        public int? ParentId { get; set; }
        public string CateDes { get; set; }
    }
}
