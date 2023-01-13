using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

#nullable disable

namespace Furniture.Models
{
    public partial class AboutU
    {
        public decimal Id { get; set; }
        public string Image { get; set; }
        public string Pargraph1 { get; set; }
        public string Pargraph2 { get; set; }
        public string Pargraph3 { get; set; }
        [NotMapped]
        public IFormFile ImageFile { get; set; }
    }
}
