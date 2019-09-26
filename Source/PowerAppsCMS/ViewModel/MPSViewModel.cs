using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PowerAppsCMS.Models;


namespace PowerAppsCMS.ViewModel
{
    /// <summary>
    /// Merupakan class model yang digunakan untuk menyimpan beberapa class atau object yang dibutuhkan pada halaman detail MPS
    /// </summary>
    public class MPSViewModel
    {
        /// <summary>
        /// Atribut PROList yang merupakan list of PRO yang berisikan daftar PRO yang akan ditampilkan
        /// </summary>
        public List<PRO> PROList { get; set; }

        /// <summary>
        /// Atribut MonthRangeList yang merupakan list of MonthRange yang berisikan daftar MonthRange yang akan ditampilkan
        /// </summary>
        public List<MonthRange> MonthRangeList { get; set; }
        
        /// <summary>
        /// Fungsi yang dijalankan ketika Class MPSViewModel diinisiasi.
        /// </summary>
        public MPSViewModel()
        {
            PROList = new List<PRO>();
            MonthRangeList = new List<MonthRange>();
        }
    }

    public class PROMPSView
    {
        public PRO PROData { get; set; }
        public List<MasterPlanSchedule> MPSList { get; set; }
        public PROMPSView()
        {
            MPSList = new List<MasterPlanSchedule>();
        }
    }
}