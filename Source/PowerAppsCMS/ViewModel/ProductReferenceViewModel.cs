using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PowerAppsCMS.Models;

namespace PowerAppsCMS.ViewModel
{
    public class ProductReferenceViewModel
    {
        public ProductReferences ProductReferences { get; set; }
        public List<Products> ProductList { get; set; } 

        public int[] SelectedProducts { get; set; }
    }
}