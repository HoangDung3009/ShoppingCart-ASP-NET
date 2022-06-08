using System;
using System.Collections.Generic;

#nullable disable

namespace eCommerce.Models
{
    public partial class Product
    {
        public Product()
        {
            OrderDetails = new HashSet<OrderDetail>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public string Thumbnail { get; set; }
        public bool IsAvailable { get; set; }
        public string Description { get; set; }
        public int CategoriesId { get; set; }
        public int TagsId { get; set; }

        public virtual Category Categories { get; set; }
        public virtual Tag Tags { get; set; }
        public virtual ICollection<OrderDetail> OrderDetails { get; set; }
    }
}
