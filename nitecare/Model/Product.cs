using System;
using System.Collections.Generic;

#nullable disable

namespace nitecare.Model
{
    public partial class Product
    {
        public Product()
        {
            OrderDetails = new HashSet<OrderDetail>();
            ProductCategories = new HashSet<ProductCategory>();
        }

        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public decimal Price { get; set; }
        public decimal? OriginalPrice { get; set; }
        public decimal? Voucher { get; set; }
        public int Stock { get; set; }
        public string ProductImage { get; set; }
        public string ShortDes { get; set; }
        public string Description { get; set; }

        public virtual ICollection<OrderDetail> OrderDetails { get; set; }
        public virtual ICollection<ProductCategory> ProductCategories { get; set; }
    }
}
