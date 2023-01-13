using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace Furniture.Models.ViewModel
{
    public class ProductViewModel
    {
        public List<ProductProject>? products { get; set; }
        public SelectList? productss { get; set; }
        public string? productname { get; set; }
        public string? SearchString { get; set; }
    }
}

