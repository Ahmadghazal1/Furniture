using System;
using System.Collections.Generic;

#nullable disable

namespace Furniture.Models
{
    public partial class Testimonial
    {
        public decimal Id { get; set; }
        public string Message { get; set; }
        public decimal UserId { get; set; }
        public string Status { get; set; }

        public virtual UserAccount User { get; set; }
    }
}
