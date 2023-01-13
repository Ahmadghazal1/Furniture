using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

#nullable disable

namespace Furniture.Models
{
    public partial class UserAccount
    {
        public UserAccount()
        {
            Logins = new HashSet<Login>();
            OrderrProjects = new HashSet<OrderrProject>();
            Payments = new HashSet<Payment>();
            Testimonials = new HashSet<Testimonial>();
        }

        public decimal Id { get; set; }
        public string Fullname { get; set; }
        public string Phone { get; set; }
        public string ImagePath { get; set; }
        public string Email { get; set; }
        public decimal RoleId { get; set; }
        [NotMapped]
        public IFormFile ImageFile { get; set; }
        public virtual RoleeUser Role { get; set; }
        public virtual ICollection<Login> Logins { get; set; }
        public virtual ICollection<OrderrProject> OrderrProjects { get; set; }
        public virtual ICollection<Payment> Payments { get; set; }
        public virtual ICollection<Testimonial> Testimonials { get; set; }
    }
}
