using PowerAppsCMS.Constants;
using PowerAppsCMS.CustomAuthentication;
using PowerAppsCMS.Models;
using PowerAppsCMS.ViewModel;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PowerAppsCMS.Controllers
{
    /// <summary>
    /// Controller untuk modul SFS
    /// </summary>
    [CustomAuthorize(Roles = RoleNames.Administrator + "," + RoleNames.SuperAdmin + "," + RoleNames.PPC + "," + RoleNames.PE + "," + RoleNames.Supervisor + "," + RoleNames.Foreman + "," + RoleNames.GroupLeaderPB + "," + RoleNames.Inspector)]
    public class SFSController : Controller
    {
        private PowerAppsCMSEntities db = new PowerAppsCMSEntities();

        /// <summary>
        /// Method Index, method yang pertama kali dijalankan ketika user memilih SFS dari 3 pilihan yang ada [MPS, SFS, Master Data]
        /// Memfilter Role apa saja yang bisa mengakses SFS
        /// Menampilkan Grup Produk yang akan dipilih untuk pengaturan SFS
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            SetAuthorize();

            return View(db.ProductGroups.OrderBy(x => x.Name).ToList());
        }

        /// <summary>
        /// Method SetAuthorize, mengatur autorisasi pengguna yang memiliki hak akses terhadap SFS
        /// </summary>
        private void SetAuthorize()
        {
            CustomAuthentication.CustomRole customeRole = new CustomRole();
            bool isPPC = customeRole.IsUserInRole(User.Identity.Name, Constants.RoleNames.PPC);
            bool isAdmin = customeRole.IsUserInRole(User.Identity.Name, Constants.RoleNames.Administrator);
            bool isPE = customeRole.IsUserInRole(User.Identity.Name, Constants.RoleNames.PE);
            bool isForeman = customeRole.IsUserInRole(User.Identity.Name, Constants.RoleNames.Foreman);
            bool isInspector = customeRole.IsUserInRole(User.Identity.Name, RoleNames.Inspector);

            Session["IsViewOnly"] = isPPC || isAdmin || isPE || isForeman || isInspector;
        }

        /// <summary>
        /// Method Details, menampilkan SFS secara rinci untuk setiap Produk Grup yang dipilih pada halaman utama SFS
        /// Memfilter Role apa saja yang bisa mengakses halaman Details SFS
        /// Menampilkan semua produk yang termasuk ke dalam Grup Produk yang dipilih, untuk mengatur jadwal pengerjaan masing-masing produk
        /// </summary>
        /// <param name="id">Parameter id dengan tipe data integer, yang dikirim dari halaman utama SFS ketika memilih Grup Produk</param>
        /// <returns></returns>
        public ActionResult Details(int id)
        {
            SetAuthorize();
            SFSViewModel sfs = GetSFSBy(id);
            return View(sfs);
        }

        /// <summary>
        /// Method GenerateDateForTableHeader, menghasilkan daftar Bulan-Tahun dengan ketentuan 2 bulan sebelum Bulan Sekarang dan 4 bulan setelah Bulan Sekarang
        /// </summary>
        /// <returns></returns>
        private static List<DateTime> GenerateDateForTableHeader()
        {
            DateTime dueDate = DateTime.Now;
            List<DateTime> dateList = new List<DateTime>();

            DateTime loopFromDate = dueDate;// dueDate.AddMonths(-2);
            DateTime loopToDate = dueDate.AddMonths(3);
            //int loopFrom = dueDate.AddMonths(-2).Month;
            //int loopTo = dueDate.AddMonths(4).Month;

            if (loopToDate.Month < loopFromDate.Month)
            {
                for (var m = loopFromDate.Month; m <= 12; m++)
                {
                    DateTime date = new DateTime(loopFromDate.Year, Convert.ToInt16(m), 1);
                    dateList.Add(date);
                }
                //loopFrom = 1;
                for (var m = 1; m <= loopToDate.Month; m++)
                {
                    DateTime date = new DateTime(loopToDate.Year, Convert.ToInt16(m), 1);//.AddYears(1);
                    dateList.Add(date);
                }
            }
            else
            {
                for (var m = loopFromDate.Month; m <= loopToDate.Month; m++)
                {
                    DateTime date = new DateTime(loopToDate.Year, Convert.ToInt16(m), 1);
                    dateList.Add(date);
                }
            }

            //int loopFrom = dueDate.AddMonths(-2).Month;
            //int loopTo = dueDate.AddMonths(4).Month;

            //if (loopTo < loopFrom)
            //{
            //    for (var m = loopFrom; m <= 12; m++)
            //    {
            //        DateTime date = new DateTime(DateTime.Now.Year, Convert.ToInt16(m), 1);
            //        dateList.Add(date);
            //    }
            //    loopFrom = 1;
            //    for (var m = loopFrom; m <= loopTo; m++)
            //    {
            //        DateTime date = new DateTime(DateTime.Now.Year, Convert.ToInt16(m), 1).AddYears(1);
            //        dateList.Add(date);
            //    }
            //}
            //else
            //{
            //    for (var m = loopFrom; m <= loopTo; m++)
            //    {
            //        DateTime date = new DateTime(DateTime.Now.Year, Convert.ToInt16(m), 1);
            //        dateList.Add(date);
            //    }
            //}

            return dateList;
        }

        /// <summary>
        /// Method UpdateDueDate, mengubah Batas Tanggal Terakhir SFS pada pengaturan jadwal pengerjaan SFS untuk setiap Unit
        /// Mengubah Batas Tanggal Akhir (SFSDueDate) pada tabel Units
        /// Menghapus Isu Proses jika ada
        /// Menghapus Proses jika ada
        /// Menambahkan Proses untuk setiap Unit
        /// </summary>
        /// <param name="dueDate">Parameter dueDate dengan tipe data DateTime, Batas Tanggal Terakhir</param>
        /// <param name="idUnit">Parameter idUnit dengan tipe data integer, ID untuk setiap Unit yang diubah SFSnya</param>
        /// <param name="dataProcess">Parameter dataProcess dengan tipe data string, rangkaian proses-proses untuk setiap Unit yang diubah SFSnya</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult UpdateDueDate(DateTime dueDate, int idUnit, string dataProcess)
        {
            var existingUnit = db.Units.Where(a => a.ID == idUnit).SingleOrDefault();
            existingUnit.SFSDueDate = dueDate;
            existingUnit.LastModified = DateTime.Now;
            existingUnit.LastModifiedBy = User.Identity.Name;

            var existingProcessList = db.Processes.Where(a => a.UnitID == idUnit).ToList();

            if (existingProcessList != null)
            {
                foreach (var item in existingProcessList)
                {
                    if (item.IsHaveProcessIssue)
                        db.ProcessIssues.RemoveRange(item.ProcessIssues.Where(a => a.ProcessID == item.ID));

                    db.Processes.Remove(item);
                }
            }

            db.SaveChanges();

            var process = dataProcess.Split(',').ToList();

            var id = db.Products.Where(a => a.ID == existingUnit.ProductID).SingleOrDefault().ProductSubGroups.ProductGroupID;
            SFSViewModel sfsList = GetSFSBy(id);

            //List<SFSDailyActivity> activityList = new List<SFSDailyActivity>();
            var processList = new List<ViewProductProcessDailySchedule>();

            //if (sfsList != null)
            //{
            processList = sfsList.DailySchedule.Where(a => a.ProductID == existingUnit.ProductID).ToList();

            //foreach (var item in sfsList)
            //{
            //activityList = sfsList.SFSDailyActivity;
            //}
            //}

            //var activityListByUnit = activityList.Where(a => a.UnitID == idUnit).OrderBy(a => a.DateActivity);
            int? totalDay = db.Products.Where(a => a.ID == existingUnit.ProductID).SingleOrDefault().TotalDay;
            int count = sfsList.SFSDailyActivity.Where(a => a.UnitID == idUnit).SingleOrDefault().DateActivity.ToArray().Count();
            DateTime startDate = sfsList.SFSDailyActivity.Where(a => a.UnitID == idUnit).SingleOrDefault().DateActivity.ElementAt(count - Convert.ToInt32(totalDay));

            if (process != null)
            {
                foreach (var item in process)
                {
                    var processMatch = processList.Where(a => a.MasterProcessID == Convert.ToInt32(item));
                    var processMatchStart = processList.Where(a => a.MasterProcessID == Convert.ToInt32(item)).First();
                    var processMatchEnd = processList.Where(a => a.MasterProcessID == Convert.ToInt32(item)).Last();

                    Process newProcess = new Process();
                    newProcess.UnitID = existingUnit.ID;
                    newProcess.Status = 0;
                    newProcess.MasterProcessID = Convert.ToInt32(item);
                    newProcess.PlanStartDate = startDate.AddDays(processMatchStart.Day - 1);// activityList.Where(a => a.UnitID == idUnit).SingleOrDefault().DateActivity.First();
                    newProcess.PlanEndDate = startDate.AddDays(processMatchEnd.Day - 1);
                    newProcess.Created = DateTime.Now;
                    newProcess.CreatedBy = User.Identity.Name;
                    newProcess.LastModified = DateTime.Now;
                    newProcess.LastModifiedBy = User.Identity.Name;
                    db.Processes.Add(newProcess);
                }
            }

            db.SaveChanges();

            return View("Details", sfsList);
            //return View("Details3", sfsList);
        }

        /// <summary>
        /// Method GetSFSBy, menampilkan semua produk yang ada pada Grup Produk yang dipilih, dimulai dari mengambil PRO, membuat header tabel yang berupa tanggal, mengambil Proses untuk setiap produk, mengambil Aktifitas Harian SFS berdasarkan Unit
        /// </summary>
        /// <param name="id">Parameter id dengan tipe data integer, yang dikirim dari halaman utama SFS ketika memilih Grup Produk</param>
        /// <returns>Return SFSViewModel</returns>
        private SFSViewModel GetSFSBy(int id)
        {
            SFSViewModel sfs = new SFSViewModel();

            ProSP pro = new ProSP();
            //var unitList = db.Units.Where(a => a.MPSDueDate != null).ToList();
            var proList = pro.GetProByProductGroupID(id);// db.Pros.Where(a => a.Product.ProductSubGroup.ProductGroupID == id).OrderBy(a => a.DueDate).ToList();

            if (proList.Count() > 0)
            {
                var groupProductData = db.ProductGroups.Find(id);
                ViewBag.GroupProductName = groupProductData.Name;

                List<DateTime> dateList = GenerateDateForTableHeader();
                sfs.DueDateHeader = dateList;

                sfs.ProList = proList;// pro.GetProByProductGroupID(id);// proList.Except(exceptProList).ToList();
                sfs.HolidayList = db.Holidays.ToList();
                sfs.DailySchedule = db.ViewProductProcessDailySchedules.ToList();

                sfs.SFSProcessList = db.Database.SqlQuery<SFSProcess>(@"exec [dbo].[GetSfsProcess]").ToList<SFSProcess>();// sfsProcessList;

                List<SFSDailyActivity> SFSDailyActivityList = new List<Models.SFSDailyActivity>();

                foreach (var u in proList)
                {
                    //var proSalesOrderList = db.PROSalesOrders.Where(a => a.PROID == u.ID).Distinct().ToList();
                    sfs.ProSalesOrderList = db.PROSalesOrders.Where(a => a.PROID == u.ID).Distinct().ToList();// proSalesOrderList;

                    var sfsSpList = db.Database.SqlQuery<DailyActivity>(@"exec GetSfsDailyActivity @id", new SqlParameter("@id", u.ID)).ToList();// db.GetSfsDailyActivity(u.ID);

                    if (sfsSpList.Count > 0)
                    {
                        int unitIdTemp = 0;
                        List<DateTime> dateListTemp = new List<DateTime>();
                        SFSDailyActivity newDaily = new SFSDailyActivity();

                        foreach (var item in sfsSpList)
                        {
                            if (unitIdTemp == 0)
                            {
                                unitIdTemp = item.UnitID;
                                dateListTemp.Add(item.DateActivity);
                            }
                            else
                            {
                                if (unitIdTemp == item.UnitID)
                                    dateListTemp.Add(item.DateActivity);
                                else
                                {
                                    unitIdTemp = item.UnitID;
                                    dateListTemp.Add(item.DateActivity);
                                }

                            }
                            if (dateListTemp.Count == sfsSpList.Where(a => a.UnitID == unitIdTemp).Count())
                            {
                                newDaily.UnitID = unitIdTemp;
                                newDaily.DateActivity = dateListTemp;
                                SFSDailyActivityList.Add(newDaily);
                                newDaily = new SFSDailyActivity();
                                dateListTemp = new List<DateTime>();
                                unitIdTemp = item.UnitID;
                            }
                        }
                    }
                    sfs.SFSDailyActivity = SFSDailyActivityList;

                    foreach (var i in u.Units.Where(a => a.PROID == u.ID && a.MPSDueDate != null))
                    {
                        var scheduleList = sfs.DailySchedule.Where(a => a.ProductID == i.ProductID);
                        string process = string.Empty;
                        if (scheduleList != null)
                        {
                            foreach (var item in scheduleList)
                            {
                                if (!process.Contains(item.MasterProcessID.ToString()))
                                    process = string.IsNullOrEmpty(process) ? item.MasterProcessID.ToString() : process + ',' + item.MasterProcessID;
                            }
                            i.Process = process;
                        }
                    }
                }
            }

            return sfs;
        }
    }
}