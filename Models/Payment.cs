using System;
using System.Collections.Generic;

#nullable disable

namespace Furniture.Models
{
    public partial class Payment
    {
        public decimal Id { get; set; }
        public decimal UserId { get; set; }
        public decimal? Amount { get; set; }
        public DateTime? PayDate { get; set; }

        public virtual UserAccount User { get; set; }
    }
}
