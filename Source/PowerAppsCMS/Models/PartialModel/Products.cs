using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace PowerAppsCMS.Models
{
    /// <summary>
    /// Partial class model product
    /// </summary>
    [MetadataType(typeof(ProductDataAnotation))]
    public partial class Products
    {
        /// <summary>
        /// atribut yang menampilkan partnumber dan nama dari Product
        /// </summary>
        public string PartNumberName
        {
            get
            {
                return this.PN + " " + "-" + " " + this.Name;
            }
            set
            {
                return;
            }
        }

        /// <summary>
        /// atribut yang menampilkan nilai maksimal day yang digunakan oleh sebuah produk
        /// </summary>
        public decimal MaxDayUsed
        {
            get
            {
                if (this.MasterProcess.Count > 0)
                {
                    return this.MasterProcess.Max(x => x.MaxDayUsed);
                }
                else
                {
                    return 0;
                }

            }
        }

        /// <summary>
        /// atribut yang menampilkan nilai maxdayused + satuannya
        /// </summary>
        public string MaxDayUsedDisplayText
        {
            get
            {
                return this.MaxDayUsed + " Days";
            }
        }
    }

    /// <summary>
    /// Data anotation model product
    /// </summary>
    public class ProductDataAnotation
    {
        [Required(ErrorMessage = "Product sub group must be selected")]
        public int ProductSubGroupID { get; set; }
        [Required(ErrorMessage = "Product is required")]
        public string Name { get; set; }
        [Required(ErrorMessage = "Part Number is required")]
        public string PN { get; set; }
        public Nullable<int> TotalDay { get; set; }
    }
}