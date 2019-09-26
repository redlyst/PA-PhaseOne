using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PowerAppsCMS.Models
{

    /// <summary>
    /// Model class untuk menampung data yang diambil dari SP GetSfsProcess
    /// </summary>
    public class SFSProcess
    {
        public int ProcessID { get; set; }
        public int UnitID { get; set; }
        public int MasterProcessID { get; set; }
        public DateTime ActualDate { get; set; }
    }
}