using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace PowerAppsCMS.Models
{
    /// <summary>
    /// Partial class model product sub group
    /// </summary>
    [MetadataType(typeof(ProductSubGroupDataAnotation))]
    public partial class ProductSubGroup
    {
    }

    /// <summary>
    /// Data anotation product sub group
    /// </summary>
    public class ProductSubGroupDataAnotation
    {
        [Required(ErrorMessage = "Product sub group name is required")]
        public string Name { get; set; }
        public int ProductGroupID { get; set; }
        [Required(ErrorMessage = "SAP Code is required")]
        public string SAPCode { get; set; }
    }
}