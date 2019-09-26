using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PowerAppsCMS.Models
{
    /// <summary>
    /// Partial class model product composition
    /// </summary>
    public partial class ProductComposition
    {
        /// <summary>
        /// Menampilkan data part number component dengan part name component
        /// </summary>
        public string ComponentPartNumber
        {
            get
            {
                return this.Component.PartNumber + " " + "-" + " " + this.Component.PartName;
            }
            set
            {
                return;
            }
        }
    }
}