using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

#nullable disable

namespace Furniture.Models
{
    public partial class CategoryProject
    {
        public CategoryProject()
        {
            ProductProjects = new HashSet<ProductProject>();
        }

        public decimal Id { get; set; }
       
        public string Categoryname { get; set; }
      
        public string ImagePath { get; set; }
        [NotMapped]
        public IFormFile ImageFile { get; set; }
        public virtual ICollection<ProductProject> ProductProjects { get; set; }
    }
}
