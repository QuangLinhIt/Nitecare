using System;
using System.Collections.Generic;

#nullable disable

namespace nitecare.Model
{
    public partial class Contact
    {
        public int ContactId { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
        public string ContactName { get; set; }
    }
}
