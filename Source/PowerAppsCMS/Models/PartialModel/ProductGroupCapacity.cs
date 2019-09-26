using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace PowerAppsCMS.Models
{
    /// <summary>
    /// Partial class product group capacity
    /// </summary>
    [MetadataType(typeof(urlmetadata))]
    public partial class ProductGroupCapacity
    {
    }

    /// <summary>
    /// Data anotation product group capacity
    /// </summary>
    public class urlmetadata
    {
        public int ID { get; set; }
        public int ProductGroupID { get; set; }

        [Required(ErrorMessage = "Capacity is required")]
        public int Capacity { get; set; }

        [Required(ErrorMessage = "Start month is required")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [DataType(DataType.Date)]
        public System.DateTime StartMonth { get; set; }

    }
}