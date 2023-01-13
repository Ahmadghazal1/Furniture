using System;
using System.Collections.Generic;

#nullable disable

namespace Furniture.Models
{
    public partial class ProductOrder
    {
        public decimal Id { get; set; }
        public decimal ProductId { get; set; }
        public string Status { get; set; }
        public decimal OrderId { get; set; }

        public virtual OrderrProject Order { get; set; }
        public virtual ProductProject Product { get; set; }
    }
}
