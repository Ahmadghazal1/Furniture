using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

#nullable disable

namespace Furniture.Models
{
    public partial class HomePaje
    {
        public decimal Id { get; set; }
        public string Image { get; set; }
        public string Logo { get; set; }
        public string Paragraph { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public string Textt { get; set; }
        [NotMapped]
        public IFormFile ImageFile { get; set; }
    }
}
