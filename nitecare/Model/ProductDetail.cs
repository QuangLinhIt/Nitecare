using System;
using System.Collections.Generic;

#nullable disable

namespace nitecare.Model
{
    public partial class ProductDetail
    {
        public int ProductDetailId { get; set; }
        public int ProductId { get; set; }
        public string Title { get; set; }
        public string ProductContent { get; set; }
        public string Description { get; set; }

        public virtual Product Product { get; set; }
    }
}
