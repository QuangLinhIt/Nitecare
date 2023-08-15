using System;
using System.Collections.Generic;

#nullable disable

namespace nitecare.Model
{
    public partial class Store
    {
        public int Id { get; set; }
        public string StoreName { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string Website { get; set; }
    }
}
