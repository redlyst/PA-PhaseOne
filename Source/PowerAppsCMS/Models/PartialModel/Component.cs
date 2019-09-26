using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace PowerAppsCMS.Models
{
    /// <summary>
    /// Partial class Component berfungsi sebagai partial class dari model Component
    /// </summary>
    [MetadataType(typeof(ComponentDataAnotation))]
    public partial class Component
    {
        /// <summary>
        /// Menampilkan data long dengan tambahan satuan milimeter
        /// </summary>
        public string LongDisplayText
        {
            get
            {
                return this.Long + " mm";
            }
        }

        /// <summary>
        /// Menampilkan data width dengan tambahan satuan milimeter
        /// </summary>
        public string WidthDisplayText
        {
            get
            {
                return this.Width + " mm";
            }
        }

        /// <summary>
        /// Menampilkan data thickness dengan tambahan satuan milimeter
        /// </summary>
        public string ThicknessDisplayText
        {
            get
            {
                return this.Thickness + " mm";
            }
        }
    }

    /// <summary>
    /// Data anotation untuk model component
    /// </summary>
    public class ComponentDataAnotation
    {
        [Required(ErrorMessage = "Part Number is required")]
        public string PartNumber { get; set; }
        [Required(ErrorMessage = "Part name is required")]
        public string PartName { get; set; }
        [Required(ErrorMessage = "Material must be selected")]
        public int MaterialID { get; set; }
        [Required(ErrorMessage = "Long is required")]
        public decimal Long { get; set; }
        [Required(ErrorMessage = "Width is required")]
        public decimal Width { get; set; }
        [Required(ErrorMessage = "Thickness is required")]
        public decimal Thickness { get; set; }
        [Required(ErrorMessage = "Outer Diameter is required")]
        public decimal OuterDiameter { get; set; }
        [Required(ErrorMessage = "Inner Diameter is required")]
        public decimal InnerDiameter { get; set; }
    }
}