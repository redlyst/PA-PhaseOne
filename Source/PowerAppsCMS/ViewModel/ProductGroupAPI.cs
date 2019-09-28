using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PowerAppsCMS.Models
{
    /// <summary>
    /// Model yang digunakan untuk meyimpan daftar unit pada halaman group leaders
    /// </summary>
    public class ProductGroupAPI
    {
        /// <summary>
        /// ID unit
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// status unit
        /// </summary>
        //public int MasterProcessID { get; set; }
        public string Name { get; set; }

        /// <summary>
        /// Nama status unit
        /// </summary>
        public string Desc { get; set; }

       
    }
}