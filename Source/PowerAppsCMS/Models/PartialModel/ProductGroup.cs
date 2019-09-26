using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace PowerAppsCMS.Models
{
    /// <summary>
    /// Partial class model product group
    /// </summary>
    [MetadataType(typeof(ProductGroupDataAnotation))]
    public partial class ProductGroup
    {
    }

    /// <summary>
    /// Data anotation product group
    /// </summary>
    public class ProductGroupDataAnotation
    {
        [Required(ErrorMessage = "Group product is required")]
        public string Name { get; set; }
        [Required(ErrorMessage = "Description is required")]
        public string Description { get; set; }
    }
}