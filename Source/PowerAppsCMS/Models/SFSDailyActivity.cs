using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PowerAppsCMS.Models
{

    /// <summary>
    /// Model class untuk menampung data yang sudah dimanipulasi pada class DailyActivity
    /// </summary>
    public class SFSDailyActivity
    {
        public int UnitID { get; set; }
        //public List<DateTime> DateActivity { get; set; }
        public List<DateTime> DateActivity { get; set; }

    }

    /// <summary>
    /// Model class untuk menampung data yang diambil dari SP GetSfsDailyActivity
    /// </summary>
    public class DailyActivity
    {
        public int UnitID { get; set; }
        //public List<DateTime> DateActivity { get; set; }
        public DateTime DateActivity { get; set; }

    }
}