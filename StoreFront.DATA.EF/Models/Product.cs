using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace StoreFront.DATA.EF.Models
{
    public partial class Product
    {
        public Product()
        {
            ProductOrders = new HashSet<ProductOrder>();
        }

        public int ProductId { get; set; }
        [Display(Name = "Product")]
        public string ProductName { get; set; } = null!;
        public decimal? Price { get; set; }
        public int? StatusId { get; set; }
        public int? SupplierId { get; set; }
        public int? CategoryId { get; set; }
        public string? Image { get; set; }

        public virtual Category? Category { get; set; }
        public virtual ProductStatus? Status { get; set; }
        public virtual Supplier? Supplier { get; set; }
        public virtual ICollection<ProductOrder> ProductOrders { get; set; }
    }
}
