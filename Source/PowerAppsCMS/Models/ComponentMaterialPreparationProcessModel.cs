using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PowerAppsCMS.Models
{
    /// <summary>
    /// Model yang digunakan pada halaman Memo Jobcard
    /// </summary>
    public class ComponentMaterialPreparationProcessModel
    {
        /// <summary>
        /// ID proses komponen
        /// </summary>
        public int PBProcessID { get; set; }

        /// <summary>
        /// Nama proses komponen
        /// </summary>
        public string ProcessName { get; set; }
    }
}