using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace PowerAppsCMS.Models
{
    /// <summary>
    /// Partial class model holiday
    /// </summary>
    [MetadataType(typeof(HolidayDataAnotation))]
    public partial class Holiday
    {
    }

    /// <summary>
    /// Data anotaion untuk model holiday
    /// </summary>
    public class HolidayDataAnotation
    {
        [Required(ErrorMessage = "Holiday name is required")]
        public string Name { get; set; }
        [Required(ErrorMessage = "Start date is required")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [DataType(DataType.Date)]
        public System.DateTime StartDate { get; set; }
        [Required(ErrorMessage = "End date is required")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [DataType(DataType.Date)]
        public System.DateTime EndDate { get; set; }
    }

}