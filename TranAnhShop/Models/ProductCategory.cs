using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TranAnhShop.Models
{
    public class ProductCategory
    {
        public int ProductID { get; set; }
        public string ProductName { get; set; }
        public string ProductImage { get; set; }
        public int ProductStatus { get; set; }
        public string CategoryName { get; set; }

    }
}