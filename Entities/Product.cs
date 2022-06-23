using System;
using System.Collections.Generic;

#nullable disable

namespace Metodichka_AIS.Entities
{
    public partial class Product
    {
        public Product()
        {
            Sales = new HashSet<Sale>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public double Price { get; set; }

        public virtual ICollection<Sale> Sales { get; set; }
    }
}
