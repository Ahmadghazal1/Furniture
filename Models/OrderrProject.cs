using System;
using System.Collections.Generic;

#nullable disable

namespace Furniture.Models
{
    public partial class OrderrProject
    {
        public OrderrProject()
        {
            ProductOrders = new HashSet<ProductOrder>();
        }

        public decimal Id { get; set; }
        public DateTime? CreatedDate { get; set; }
        public decimal UserId { get; set; }

        public virtual UserAccount User { get; set; }
        public virtual ICollection<ProductOrder> ProductOrders { get; set; }
    }
}
