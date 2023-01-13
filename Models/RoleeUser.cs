using System;
using System.Collections.Generic;

#nullable disable

namespace Furniture.Models
{
    public partial class RoleeUser
    {
        public RoleeUser()
        {
            UserAccounts = new HashSet<UserAccount>();
        }

        public decimal Id { get; set; }
        public string Rolename { get; set; }

        public virtual ICollection<UserAccount> UserAccounts { get; set; }
    }
}
