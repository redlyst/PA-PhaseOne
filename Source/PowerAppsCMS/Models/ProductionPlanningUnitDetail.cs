using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PowerAppsCMS.Models
{
    /// <summary>
    /// Model yang digunakan untuk menyimpan daftar proses dari unit yang dipilih pada halaman group leaders
    /// </summary>
    public class ProductionPlanningUnitDetail
    {
        /// <summary>
        /// ID proses
        /// </summary>
        public int ProcessID { get; set; }

        /// <summary>
        /// ID grup proses
        /// </summary>
        public int ProcessGroupID { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public bool IsCanAccess { get; set; }

        /// <summary>
        /// ID status proses
        /// </summary>
        public int StatusID { get; set; }

        /// <summary>
        /// Nama status proses
        /// </summary>
        public string StatusName { get; set; }

        /// <summary>
        /// Nama proses
        /// </summary>
        public string ProcessName { get; set; }

        /// <summary>
        /// Jumlah jam kerja operator
        /// </summary>
        public decimal ManHour { get; set; }

        /// <summary>
        /// Jumlah operator
        /// </summary>
        public int ManPower { get; set; }

        /// <summary>
        /// ID master proses
        /// </summary>
        public int MasterProcessID { get; set; }

        /// <summary>
        /// Total jumlah jam kerja operator
        /// </summary>
        public Nullable<decimal> ManHourActual { get; set; }
        //public Boolean IsStopByOperator { get; set; }

        public string LastModifiedby { get; set; }
    }
}