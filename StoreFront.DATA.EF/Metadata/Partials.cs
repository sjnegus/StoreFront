using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace StoreFront.DATA.EF//.Metadata
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
    public partial class Product { }
    //
    [ModelMetadataType(typeof(ProductOrderMetadata))]
    public partial class ProductOrder { }
    //
    [ModelMetadataType(typeof(ProductStatusMetadata))]
    public partial class ProductStatus { }
    //
    [ModelMetadataType(typeof(SupplierMetadata))]
    public partial class Supplier { }
}
