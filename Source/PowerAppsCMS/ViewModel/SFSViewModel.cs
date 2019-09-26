using PowerAppsCMS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PowerAppsCMS.ViewModel
{
    /// <summary>
    /// Model class untuk menampung data pada modul SFS yang terdiri dari ::
    /// List PRO
    /// List PRO Sales Order
    /// List Hari Libur
    /// List Skedul Harian
    /// List Aktifitas Harian
    /// List Proses SFS
    /// List Header Tanggal
    /// </summary>
    public class SFSViewModel
    {
        public List<PROSalesOrder> ProSalesOrderList { get; set; }
        public List<PRO> ProList { get; set; }
        public List<DateTime> DueDateHeader { get; set; }
        public List<Holiday> HolidayList { get; set; }
        public List<ViewProductProcessDailySchedule> DailySchedule { get; set; }
        public List<SFSDailyActivity> SFSDailyActivity { get; set; }
        public List<SFSProcess> SFSProcessList { get; set; }
    }
}