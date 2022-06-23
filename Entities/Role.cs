using System;
using System.Collections.Generic;

#nullable disable

namespace Metodichka_AIS.Entities
{
    public partial class Role
    {
        public Role()
        {
            UsersMs = new HashSet<UsersM>();
        }

        public int Id { get; set; }
        public string Name { get; set; }

        public virtual ICollection<UsersM> UsersMs { get; set; }
    }
}
