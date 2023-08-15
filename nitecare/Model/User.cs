using System;
using System.Collections.Generic;

#nullable disable

namespace nitecare.Model
{
    public partial class User
    {
        public int UserId { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public DateTime CreateDate { get; set; }
        public bool Active { get; set; }
        public int RoleId { get; set; }
        public string Phone { get; set; }
        public string UserName { get; set; }

        public virtual Role Role { get; set; }
    }
}
