using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace StoreFront.DATA.EF.Models//.Metadata
{
    public class CategoryMetadata
    {
        // Skipping ID because it is auto-incrementing
        [Required(ErrorMessage = "* Category name is required.")]
        [StringLength(50)]
        [Display(Name = "Category")]
        public string CategoryName { get; set; } = null!;
    }

    public class OrderMetadata
    {

        [Display(Name = "User")]
        public string UserId { get; set; } = null!;

        [Required]
        [Display(Name = "Order Date")]
        [DisplayFormat(DataFormatString = "{0:d}")]//MM/dd/yyyy
        public DateTime OrderDate { get; set; }

        [StringLength(100)]
        [Display(Name = "Recipient")]
        [DisplayFormat(NullDisplayText = "Not Given")]
        public string? ShipToName { get; set; }

        [StringLength(50)]
        [Display(Name = "City")]
        [DisplayFormat(NullDisplayText = "Not Given")]
        public string? ShipCity { get; set; }

        [StringLength(2, MinimumLength = 2)]
        [Display(Name = "State")]
        [DisplayFormat(NullDisplayText = "Not Given")]
        public string? ShipState { get; set; }

        [DisplayFormat(NullDisplayText = "Not Given")]
        [StringLength(5, MinimumLength = 5)]
        [Display(Name = "Zip")]
        [DataType(DataType.PostalCode)]
        public string? ShipZip { get; set; }
    }

    public class ProductMetadata
    {
        [StringLength(50)]
        [Display(Name = "Product")]
        [Required]
        public string ProductName { get; set; } = null!;
        //
        [DisplayFormat(DataFormatString = "{0:#.##}")]
        [DataType(DataType.Currency)]
        [Range(0, (double)decimal.MaxValue)]
        [Required]
        public decimal? Price { get; set; }
        //
        [Display(Name = "Status")]
        public int? StatusId { get; set; }
        //
        [Display(Name = "Supplier")]
        public int? SupplierId { get; set; }
        //
        [Display(Name = "Category")]
        public int? CategoryId { get; set; }
        //
        [StringLength(75)]
        [Display(Name = "Image")]
        public string? Image { get; set; }
    }

    public class ProductOrderMetadata
    {
        [Required]
        [Display(Name="Product ID")]
        public int ProductId { get; set; }
        //
        [Required]
        [Display(Name = "Category ID")]
        public int OrderId { get; set; }
        //
        [DisplayFormat(ApplyFormatInEditMode = false, DataFormatString = "{0:c}")]
        [DataType(DataType.Currency)]
        [Range(0, (double)decimal.MaxValue)]
        [Required]
        public decimal Price { get; set; }
        //
        [Required]
        public int Quantity { get; set; }
    }

    public class ProductStatusMetadata
    {
        
        public string Status { get; set; } = null!;
    }

    public class SupplierMetadata
    {
        // ID
        [Key]
        [Display(Name = "Supplier")]
        public int SupplierId { get; set; }
        // Name
        [Display(Name = "Supplier")]
        [Required(ErrorMessage = "Supplier is required")]
        [StringLength(100, ErrorMessage = "*100 Char max")]
        public string SupplierName { get; set; } = null!;
        // Main Contact
        [Required(ErrorMessage= "Main Contact is required")]
        [Display(Name = "Main Contact")]
        [StringLength(50)]
        public string MainContact { get; set; } = null!;
        // Phone
        [Required(ErrorMessage= "Phone Number is required")]
        [DataType(DataType.PhoneNumber)]
        public string Phone { get; set; } = null!;
        // Address
        [StringLength(150)]
        public string? Address { get; set; } = null!;
        // City
        [StringLength(100)]
        public string? City { get; set; } = null!;
        // State
        [StringLength(2, MinimumLength = 2)]
        [DisplayFormat(NullDisplayText = "[N/A]")]
        public string? State { get; set; }
        // Zip
        [StringLength(5, MinimumLength = 5)]
        [DataType(DataType.PostalCode)]
        public string? Zip { get; set; }
    }

    public class UserDetailMetadata
    {
        public string UserId { get; set; } = null!;

        [StringLength(50)]
        [Display(Name = "First Name")]
        [Required]
        public string FirstName { get; set; } = null!;

        [StringLength(50)]
        [Display(Name = "Last Name")]
        [Required]
        public string LastName { get; set; } = null!;

        [StringLength(150)]
        public string? Address { get; set; }

        [StringLength(50)]
        public string? City { get; set; }

        [StringLength(2)]
        public string? State { get; set; }

        [StringLength(5)]
        [DataType(DataType.PostalCode)]
        public string? Zip { get; set; }

        [StringLength(24)]
        [DataType(DataType.PhoneNumber)]
        public string? Phone { get; set; }
    }

}
