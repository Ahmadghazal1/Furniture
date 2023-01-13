using System;
using System.Collections.Generic;

#nullable disable

namespace Furniture.Models
{
    public partial class Login
    {
        public string Username { get; set; }
        public string Passwordd { get; set; }
        public decimal UserId { get; set; }
        public decimal Id { get; set; }

        public virtual UserAccount User { get; set; }
    }
}
