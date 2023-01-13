using System;
using System.Collections.Generic;

#nullable disable

namespace Furniture.Models
{
    public partial class Bank
    {
        public decimal Id { get; set; }
        public string Cardnumber { get; set; }
        public string Cvv { get; set; }
        public decimal? Amount { get; set; }
    }
}
