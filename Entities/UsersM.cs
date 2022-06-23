using System;
using System.Collections.Generic;

#nullable disable

namespace Metodichka_AIS.Entities
{
    public partial class UsersM
    {
        public UsersM()
        {
            Sales = new HashSet<Sale>();
        }

        public int Id { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }
        public int RoleId { get; set; }

        public virtual Role Role { get; set; }
        public virtual ICollection<Sale> Sales { get; set; }
    }
}
