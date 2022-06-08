using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

#nullable disable

namespace eCommerce.Models
{
    public partial class OrderDetail
    {
        public int Id { get; set; }

        [Required]
        public int quantity { get; set; }
        public int ProductId { get; set; }
        public int OrderId { get; set; }

        public virtual Order Order { get; set; }
        public virtual Product Product { get; set; }
    }
}
