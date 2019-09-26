using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PowerAppsCMS.Models
{
    /// <summary>
    /// Model yang digunakan untuk menyimpan data actual Memo Jobcard
    /// </summary>
    public class MemoJobCardModel
    {
        /// <summary>
        /// ID memo jobcard
        /// </summary>
        public int JobCardID { get; set; }

        /// <summary>
        /// ID proses komponen
        /// </summary>
        public int PBProcessID { get; set; }

        /// <summary>
        /// Nama proses komponen
        /// </summary>
        public string PBProcessName { get; set; }

        /// <summary>
        /// jumlah yang dibutuhkan
        /// </summary>
        public int Quantity { get; set; }

        /// <summary>
        /// username 
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// Tanggal jobcard dibuat
        /// </summary>
        public string Date { get; set; }
    }
}