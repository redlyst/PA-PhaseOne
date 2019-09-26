using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PowerAppsCMS.Models
{
    /// <summary>
    /// Model yang digunakan untuk menyimpan data memo component pada halaman PB
    /// </summary>
    public class MemoComponentModel
    {
        /// <summary>
        /// ID memo komponen
        /// </summary>
        public int MemoComponentID { get; set; }

        /// <summary>
        /// ID memo
        /// </summary>
        public int MemoID { get; set; }

        /// <summary>
        /// ID komponen
        /// </summary>
        public int ComponentID { get; set; }

        /// <summary>
        /// ID status memo
        /// </summary>
        public int StatusID { get; set; }

        /// <summary>
        /// Nama status memo
        /// </summary>
        public string StatusName { get; set; }

        /// <summary>
        /// Part number
        /// </summary>
        public string PartNumber { get; set; }

        /// <summary>
        /// Part name
        /// </summary>
        public string PartName { get; set; }

        /// <summary>
        /// Total jumlah komponen
        /// </summary>
        public int TotalComponentQuantity { get; set; }

        /// <summary>
        /// Daftar PRO
        /// </summary>
        public string PRONumberDisplayText { get; set; }

        /// <summary>
        /// Tanggal memo dibuat
        /// </summary>
        public string MemoDateCreated { get; set; }
    }
}