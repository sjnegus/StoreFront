using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace StoreFront.DATA.EF.Models//.Metadata
{
    //internal class Partials
    //{
    //}
    [ModelMetadataType(typeof(CategoryMetadata))]
    public partial class Category { }
    //
    [ModelMetadataType(typeof(OrderMetadata))]
    public partial class Order { }
    //
    [ModelMetadataType(typeof(ProductMetadata))]
    public partial class Product 
    {
        [NotMapped]
        public string SearchString => $"{ProductName} {Category?.CategoryName} {Supplier?.SupplierName}";

        [NotMapped]
        public IFormFile? ImageFile { get; set; }

        [NotMapped]
        [FileExtensions(Extensions = "png,jpeg,jpg,gif", ErrorMessage = ".png, .jpeg, .jpg, .gif")]
        public string? ImageName => ImageFile?.FileName;
    }
    //
    [ModelMetadataType(typeof(ProductOrderMetadata))]
    public partial class ProductOrder { }
    //
    [ModelMetadataType(typeof(ProductStatusMetadata))]
    public partial class ProductStatus { }
    //
    [ModelMetadataType(typeof(SupplierMetadata))]
    public partial class Supplier { }
    //
    [ModelMetadataType(typeof(UserDetailMetadata))]
    public partial class UserDetail
    {

        [Display(Name = "Full Name")]
        [NotMapped]
        public string FullName => $"{FirstName} {LastName}";

    }
}
