using System;
using System.Collections.Generic;

#nullable disable

namespace eCommerce.Models
{
    public partial class Tag
    {
        public Tag()
        {
            Products = new HashSet<Product>();
        }

        public int Id { get; set; }
        public string Name { get; set; }

        public virtual ICollection<Product> Products { get; set; }
    }
}
