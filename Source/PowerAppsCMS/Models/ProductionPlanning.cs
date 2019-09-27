using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PowerAppsCMS.Models
{
    /// <summary>
    /// Model yang digunakan untuk meyimpan daftar unit pada halaman group leaders
    /// </summary>
    public class ProductionPlanning
    {
        /// <summary>
        /// ID unit
        /// </summary>
        public int UnitID { get; set; }

        /// <summary>
        /// status unit
        /// </summary>
        //public int MasterProcessID { get; set; }
        public int StatusID { get; set; }

        /// <summary>
        /// Nama status unit
        /// </summary>
        public string StatusName { get; set; }

        /// <summary>
        /// Serial number
        /// </summary>
        //public DateTime StartDate { get; set; }
        public string SerialNumber { get; set; }

        /// <summary>
        /// daftar PRO
        /// </summary>
        public string PRO { get; set; }

        /// <summary>
        /// ID produk
        /// </summary>
        public string Product { get; set; }

        /// <summary>
        /// Daftar pelanggan
        /// </summary>
        public string Customer { get; set; }


        /// <summary>
        /// ID produk
        /// </summary>
        public int ProductID { get; set; }

        ///ID Product Group
        //public int PGID { get; set; }

        /// Product Group Name
        //public string PGName { get; set; }
    }
}