using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PowerAppsCMS.Models
{
    /// <summary>
    /// Class model yang menyimpan informasi aktivitas pekerjaan operator
    /// </summary>
    public class WorkingLog
    {
        /// <summary>
        /// Nama aktivitas operator atau alasan pause operator
        /// </summary>
        public string Remark { get; set; }

        /// <summary>
        /// waktu aktivitas operator
        /// </summary>
        public string ActivityDateTime { get; set; }
        
    }
}