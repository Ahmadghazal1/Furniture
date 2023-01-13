using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

#nullable disable

namespace Furniture.Models
{
    public partial class ProductProject
    {
        public ProductProject()
        {
            ProductOrders = new HashSet<ProductOrder>();
        }

        public decimal Id { get; set; }
        public string ProductName { get; set; }
        public string ImagePath { get; set; }
        public decimal? Price { get; set; }
        public decimal? Valuee { get; set; }
        public decimal CategoryId { get; set; }
        [NotMapped]
        public IFormFile ImageFile { get; set; }
        public virtual CategoryProject Category { get; set; }
        public virtual ICollection<ProductOrder> ProductOrders { get; set; }
    }
}
