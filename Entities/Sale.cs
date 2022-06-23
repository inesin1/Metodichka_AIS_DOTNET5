using System;
using System.Collections.Generic;

#nullable disable

namespace Metodichka_AIS.Entities
{
    public partial class Sale
    {
        public int Id { get; set; }
        public int User { get; set; }
        public int Product { get; set; }
        public DateTime Date { get; set; }

        public virtual Product ProductNavigation { get; set; }
        public virtual UsersM UserNavigation { get; set; }
    }
}
