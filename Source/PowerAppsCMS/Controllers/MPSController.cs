using PowerAppsCMS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PowerAppsCMS.ViewModel;
using System.Web.Mvc;
using System.Globalization;
using PowerAppsCMS.Constants;
using PowerAppsCMS.CustomAuthentication;

namespace PowerAppsCMS.Controllers
{
    /// <summary>
    /// Merupakan Class Controller yang berisikan fungsi fungsi untuk mengelola data MPS
    /// </summary>
    public class MPSController : Controller
    {
        private PowerAppsCMSEntities db = new PowerAppsCMSEntities();
        //List<MPSActual> mpsActualList = new List<MPSActual>();
        private string debugViewMessage = string.Empty;
        // GET: MPS

        /// <summary>
        /// Merupakan fungsi yang menampilkan halaman index dari modul MPS yang bersikan daftar nama-nama product yang ada.
        /// </summary>
        /// <returns>View Index yang berisikan daftar Product</returns>
        [CustomAuthorize(Roles = RoleNames.SuperAdmin + "," + RoleNames.Administrator + "," + RoleNames.PE + "," + RoleNames.PPC + "," + RoleNames.Supervisor + "," + RoleNames.GroupLeaderPB + "," + RoleNames.Inspector)]
        public ActionResult Index()
        {
            return View(db.ProductGroups.OrderBy(x => x.Name).ToList());
        }

        // GET: MPS/Details/{id}
        /// <summary>
        /// Merupakan sebuah fungsi yang menampilkan halaman berisikan daftar PRO dan MPS yang dimiliki sebuah product yang di
        /// </summary>
        /// <param name="id">sebuah bilangan integer yang berarti id dari sebuah product</param>
        /// <returns>Sebuah ActionResult yang menampilkan halaman MPS berdasarkan product yang dipilih</returns>
        [CustomAuthorize(Roles = RoleNames.SuperAdmin + "," + RoleNames.Administrator + "," + RoleNames.PE + "," + RoleNames.PPC + "," + RoleNames.Supervisor + "," + RoleNames.GroupLeaderPB + "," + RoleNames.Inspector)]
        public ActionResult Details(int? id)
        {
            if (id != null)
            {
                ViewBag.canCRUD = false;
                CustomRole customeRole = new CustomRole();
                ViewBag.canCRUD = customeRole.IsUserInRole(User.Identity.Name, RoleNames.PPC) || customeRole.IsUserInRole(User.Identity.Name, RoleNames.SuperAdmin);//canCRUD;

                DateTime startDate = DateTime.Now.AddMonths(-1);
                startDate = startDate.AddMilliseconds(0 - startDate.Millisecond);
                startDate = startDate.AddSeconds(0 - startDate.Second);
                startDate = startDate.AddMinutes(0 - startDate.Minute);
                startDate = startDate.AddHours(0 - startDate.Hour);
                startDate = startDate.AddDays(1 - startDate.Day);
                DateTime endDate = DateTime.Now.AddMonths(4);


                MPSViewModel mpsData = new MPSViewModel();
                List<PRO> listPRO = new List<PRO>();

                ProductGroup groupProductData = db.ProductGroups.Find(id);
                ViewBag.GroupProductName = groupProductData.Name;
                ViewBag.week = 0;
                ViewBag.tanggal = DateTime.Now;
                ViewBag.ReasonIssue = new SelectList(db.ReasonIssues, "ID", "Name");

                #region CountHolidayTotalInRange
                List<DateTime> holidayDateList = db.HolidayDetails.Where(x => startDate >= x.HolidayDate && endDate <= x.HolidayDate).Select(x => x.HolidayDate).Distinct().ToList();

                int totalHoliday = 0;
                foreach (DateTime item in holidayDateList)
                {
                    if (item.DayOfWeek != DayOfWeek.Saturday && item.DayOfWeek != DayOfWeek.Sunday)
                    {
                        totalHoliday++;
                    }
                }
                #endregion

                #region SetWeekNumberDataHeaderForEachMonth
                CultureInfo ci = new CultureInfo("en-US");
                DateTime incDateTime = startDate;
                do
                {
                    MonthRange monthRangeData = new MonthRange();
                    //int? currentCapacity 
                    ProductGroupCapacity productGroupCapacity = groupProductData.ProductGroupCapacities.Where(x => x.StartMonth.AddDays(1 - x.StartMonth.Day).Date <= incDateTime.Date).OrderByDescending(o => o.StartMonth).FirstOrDefault();
                    monthRangeData.Month = incDateTime.Month;
                    monthRangeData.Year = incDateTime.Year;
                    monthRangeData.MonthDisplayText = incDateTime.ToString("MMMM", ci) + " " + incDateTime.Year.ToString();
                    monthRangeData.WeekNumberList.AddRange(GetWeekNumberListofMonths(incDateTime.Month, incDateTime.Year));
                    monthRangeData.Capacity = null;
                    if (productGroupCapacity != null)
                    {
                        monthRangeData.Capacity = productGroupCapacity.Capacity;
                    }
                    mpsData.MonthRangeList.Add(monthRangeData);
                    incDateTime = incDateTime.AddMonths(1);
                } while (incDateTime <= endDate);
                #endregion

                listPRO = db.Pros.Where(x => x.Products.ProductSubGroups.ProductGroup.ID == id).ToList();
                listPRO = listPRO.Where(x => x.DueDate.Date >= startDate.Date).OrderBy(x => x.Products.Name).ThenBy(x => x.DueDate).ToList();


                #region setPROMinimunDueDate
                foreach (PRO item in listPRO)
                {
                    int processDay = 0;
                    int pbDays = 0;

                    if (item.Products.ProductComposition.Count() > 0)
                    {
                        pbDays = 10;
                    }

                    DateTime currentDateTime = DateTime.Now;
                    if (item.Products.TotalDay != null)
                    {
                        processDay = (int)item.Products.TotalDay + pbDays;
                    }
                    int totalWeek = (processDay / 5);
                    int sisaHari = processDay % 5;

                    currentDateTime = currentDateTime.AddDays(processDay + (totalWeek * 2) + totalHoliday);

                    item.MinimumDueDate = currentDateTime;

                    DateTime currentMonthFirstDate = startDate.AddMonths(1);
                    int totalActualUnitUntilCurrentMonth = item.Units.Where(x => x.ActualDeliveryDate < currentMonthFirstDate).Count();
                    int totalActualUnitUntilLastMonth = item.Units.Where(x => x.ActualDeliveryDate < startDate).Count();
                    List<MasterPlanSchedule> listMPSUntilCurrentMonth = item.MasterPlanSchedules.Where(x => x.EndWorkingDate < currentMonthFirstDate).ToList();
                    List<MasterPlanSchedule> listMPSUntilLastMonth = listMPSUntilCurrentMonth.Where(x => x.EndWorkingDate < startDate).ToList();
                    int totalPlanUntilCurrentMonth = listMPSUntilCurrentMonth.Sum(x => x.CurrentPlannedQuantity);
                    int totalPlanUntilLastMonth = listMPSUntilLastMonth.Sum(x => x.CurrentPlannedQuantity);

                    item.CurrentMonthCarryOver = totalPlanUntilCurrentMonth - totalActualUnitUntilCurrentMonth;
                    item.LastMonthCarryOver = totalPlanUntilLastMonth - totalActualUnitUntilLastMonth;

                    item.MaximumPlanQuantity = item.UnHoldUnitCount - item.Units.Where(x => x.MPSDueDate < startDate && x.IsHold == false).Count();
                }
                #endregion
                //listPRO = listPRO.OrderBy(x => x.Product.Name).ThenBy(x => x.DueDate).ToList();
                mpsData.PROList = listPRO;
                ViewBag.TotalCarryOver = listPRO.Sum(x => x.LastMonthCarryOver);
                ViewBag.TestPesan = debugViewMessage;

                return View(mpsData);
            }
            else
            {
                return RedirectToAction("Index");
            }
        }

        /// <summary>
        /// Merupakan fungsi untuk melakukan update nilai dari MPS
        /// </summary>
        /// <param name="masterPlanScheduleList">list of MPS yang berisikan daftar MPS yang akan di update valuenya</param>
        /// <returns>JSON yang berisikan pesan dan kondisi dari proses update berhasil</returns>
        [CustomAuthorize(Roles = RoleNames.SuperAdmin + "," + RoleNames.PPC)]
        public ActionResult UpdateMPSValue(List<MasterPlanSchedule> masterPlanScheduleList)
        {
            try
            {
                var username = User.Identity.Name;
                DateTime now = DateTime.Now;
                db = new PowerAppsCMSEntities();

                if (masterPlanScheduleList != null)
                {
                    int proid = 0;
                    MasterPlanSchedule firstMPSData = masterPlanScheduleList.FirstOrDefault();
                    MasterPlanSchedule lastMPSData = masterPlanScheduleList.LastOrDefault();

                    foreach (MasterPlanSchedule item in masterPlanScheduleList)
                    {
                        proid = item.PROID;

                        MasterPlanSchedule currenDataMPS = new MasterPlanSchedule();

                        if (item.ID != 0)//if mps ID has value (not equal zero), find the mps data base on ID in Database
                        {
                            currenDataMPS = db.MasterPlanSchedules.Find(item.ID);
                        }

                        if (item.ID == 0 || currenDataMPS == null) //if mps ID has zero value or currentMPSData is Null, find the mps data base on proID, week, month and year data in Database
                        {
                            currenDataMPS = db.MasterPlanSchedules.Where(x => x.PROID == item.PROID && x.Week == item.Week && x.Month == item.Month && x.Year == item.Year).OrderByDescending(x => x.Created).FirstOrDefault();
                        }

                        if (currenDataMPS == null)
                        {
                            if (item.PlannedQuantity > 0) //if currentDataMPS is not exist but newData have planQuantity, so we add new mps data in database
                            {
                                //AddMPSData(item, username, now);
                                MasterPlanSchedule newMPSData = new MasterPlanSchedule();

                                newMPSData.PROID = item.PROID;
                                newMPSData.Week = item.Week;
                                newMPSData.Month = item.Month;
                                newMPSData.Year = item.Year;
                                newMPSData.PlannedQuantity = item.PlannedQuantity;
                                newMPSData.EndWorkingDate = item.EndWorkingDate;
                                newMPSData.CreatedBy = newMPSData.LastModifiedBy = username;
                                newMPSData.Created = newMPSData.LastModified = now;

                                db.MasterPlanSchedules.Add(newMPSData);
                            }
                        }
                        else
                        {
                            if (item.PlannedQuantity > 0) //if currentDataMPS is exist and newData have planQuantity, so we update the mps data in database
                            {
                                //currenDataMPS = UpdateMPSData(item, currenDataMPS,username,now);
                                currenDataMPS.PlannedQuantity = item.PlannedQuantity;
                                currenDataMPS.EndWorkingDate = item.EndWorkingDate;
                                currenDataMPS.LastModified = now;
                                currenDataMPS.LastModifiedBy = username;
                            }

                            if (item.PlannedQuantity == 0) //if currentDataMPS is exist but new plannedQuantity is zero, so we delete the mps data in database
                            {
                                //DeleteMPSData(currenDataMPS, username, now);
                                List<Process> processWillbeDeleted = new List<Process>();
                                foreach (Unit unitItem in currenDataMPS.Units)
                                {
                                    unitItem.MPSID = null;
                                    unitItem.MPSDueDate = null;
                                    unitItem.SFSDueDate = null;
                                    unitItem.LastModified = now;
                                    unitItem.LastModifiedBy = username;

                                    processWillbeDeleted.AddRange(unitItem.Processes);
                                }

                                db.Processes.RemoveRange(processWillbeDeleted);
                                db.MasterPlanSchedules.Remove(currenDataMPS);
                            }
                        }
                        #region Old Code
                        //if (item.ID == 0 && item.PlannedQuantity > 0)
                        //{
                        //    MasterPlanSchedule newMPSData = new MasterPlanSchedule();

                        //    newMPSData = db.MasterPlanSchedules.Where(x => x.PROID == item.PROID && x.Week == item.Week && x.Month == item.Month && x.Year == item.Year).SingleOrDefault();

                        //    if (newMPSData == null)
                        //    {
                        //        newMPSData = new MasterPlanSchedule();

                        //        newMPSData.PROID = item.PROID;
                        //        newMPSData.Week = item.Week;
                        //        newMPSData.Month = item.Month;
                        //        newMPSData.Year = item.Year;
                        //        newMPSData.PlannedQuantity = item.PlannedQuantity;
                        //        newMPSData.EndWorkingDate = item.EndWorkingDate;
                        //        newMPSData.CreatedBy = newMPSData.LastModifiedBy = username;
                        //        newMPSData.Created = newMPSData.LastModified = now;

                        //        db.MasterPlanSchedules.Add(newMPSData);
                        //    }
                        //    else
                        //    {
                        //        newMPSData.PlannedQuantity = item.PlannedQuantity;
                        //        newMPSData.EndWorkingDate = item.EndWorkingDate;
                        //        newMPSData.LastModified = now;
                        //        newMPSData.LastModifiedBy = username;
                        //    }
                        //}
                        //else if (item.ID == 0 && item.PlannedQuantity == 0)
                        //{
                        //    MasterPlanSchedule newMPSData = new MasterPlanSchedule();

                        //    newMPSData = db.MasterPlanSchedules.Where(x => x.PROID == item.PROID && x.Week == item.Week && x.Month == item.Month && x.Year == item.Year).SingleOrDefault();

                        //    if (newMPSData != null)
                        //    {
                        //        List<Process> processWillbeDeleted = new List<Process>();
                        //        foreach (Unit unitItem in newMPSData.Units)
                        //        {
                        //            unitItem.MPSID = null;
                        //            unitItem.MPSDueDate = null;
                        //            unitItem.SFSDueDate = null;
                        //            unitItem.LastModified = now;
                        //            unitItem.LastModifiedBy = username;

                        //            processWillbeDeleted.AddRange(unitItem.Processes);
                        //        }

                        //        db.Processes.RemoveRange(processWillbeDeleted);
                        //        db.MasterPlanSchedules.Remove(newMPSData);
                        //    }
                        //}
                        //else
                        //{
                        //    MasterPlanSchedule currentMPSData = db.MasterPlanSchedules.Find(item.ID);

                        //    if (currentMPSData == null)
                        //    {
                        //        currentMPSData = db.MasterPlanSchedules.Where(x => x.PROID == item.PROID && x.Week == item.Week && x.Month == item.Month && x.Year == item.Year).SingleOrDefault();
                        //    }

                        //    if (currentMPSData != null)
                        //    {
                        //        if (item.PlannedQuantity > 0)
                        //        {
                        //            currentMPSData.PlannedQuantity = item.PlannedQuantity;
                        //            currentMPSData.EndWorkingDate = item.EndWorkingDate;
                        //            currentMPSData.LastModified = now;
                        //            currentMPSData.LastModifiedBy = username;
                        //        }

                        //        if (item.PlannedQuantity == 0)
                        //        {
                        //            List<Process> processWillbeDeleted = new List<Process>();
                        //            foreach (Unit unitItem in currentMPSData.Units)
                        //            {
                        //                unitItem.MPSID = null;
                        //                unitItem.MPSDueDate = null;
                        //                unitItem.SFSDueDate = null;
                        //                unitItem.LastModified = now;
                        //                unitItem.LastModifiedBy = username;

                        //                processWillbeDeleted.AddRange(unitItem.Processes);
                        //            }

                        //            db.Processes.RemoveRange(processWillbeDeleted);
                        //            db.MasterPlanSchedules.Remove(currentMPSData);

                        //        }
                        //    }
                        //    else
                        //    {
                        //        MasterPlanSchedule newMPSData = new MasterPlanSchedule();

                        //        newMPSData.PROID = item.PROID;
                        //        newMPSData.Week = item.Week;
                        //        newMPSData.Month = item.Month;
                        //        newMPSData.Year = item.Year;
                        //        newMPSData.PlannedQuantity = item.PlannedQuantity;
                        //        newMPSData.EndWorkingDate = item.EndWorkingDate;
                        //        newMPSData.CreatedBy = newMPSData.LastModifiedBy = username;
                        //        newMPSData.Created = newMPSData.LastModified = now;

                        //        db.MasterPlanSchedules.Add(newMPSData);
                        //    }
                        //} 
                        #endregion
                    }

                    if (db.SaveChanges() > 0)//if database update is happened
                    {
                        List<MasterPlanSchedule> listCurrentMPS = db.MasterPlanSchedules.Where(x => x.PROID == proid).ToList();
                        List<MasterPlanSchedule> listNotUpdateMPS = listCurrentMPS.Where(x => x.EndWorkingDate < firstMPSData.EndWorkingDate || x.EndWorkingDate >= lastMPSData.EndWorkingDate).ToList();
                        listCurrentMPS = listCurrentMPS.Where(x => x.EndWorkingDate.Date >= firstMPSData.EndWorkingDate.Date && x.EndWorkingDate.Date <= lastMPSData.EndWorkingDate.Date).OrderBy(o => o.EndWorkingDate).ToList();

                        List<Unit> listUnit = db.Units.Where(x => x.PROID == proid && x.IsHold == false).ToList();
                        listUnit = listUnit.Where(x => !listNotUpdateMPS.Contains(x.MasterPlanSchedule)).ToList();
                        listUnit = listUnit.Where(x => x.IsHaveProcessAssign == false).OrderBy(o => o.SerialNumber).ToList();

                        int indexData = 0;
                        foreach (MasterPlanSchedule item in listCurrentMPS)
                        {
                            int unAssignedQuantity = 0;
                            unAssignedQuantity = item.PlannedQuantity - item.AssignedPlanCount;

                            if (unAssignedQuantity > 0)
                            {
                                DateTime mpsDueDate = Convert.ToDateTime(item.EndWorkingDate);

                                if (item.PRO != null)
                                {
                                    if (mpsDueDate.Date > item.PRO.DueDate.Date)
                                    {
                                        mpsDueDate = item.PRO.DueDate;
                                    }
                                }
                                else
                                {
                                    PRO currentPRO = db.Pros.Find(item.PROID);
                                    if (currentPRO != null && mpsDueDate.Date > currentPRO.DueDate.Date)
                                    {
                                        mpsDueDate = currentPRO.DueDate;
                                    }
                                }

                                Holiday holidayDate = db.Holidays.Where(x => x.StartDate <= mpsDueDate && x.EndDate >= mpsDueDate).OrderBy(o => o.StartDate).FirstOrDefault();

                                if (holidayDate != null)
                                {
                                    mpsDueDate = holidayDate.StartDate.AddDays(-1);

                                    if (mpsDueDate.DayOfWeek == DayOfWeek.Sunday)
                                    {
                                        mpsDueDate = mpsDueDate.AddDays(-2);
                                    }
                                    else if (mpsDueDate.DayOfWeek == DayOfWeek.Saturday)
                                    {
                                        mpsDueDate = mpsDueDate.AddDays(-1);
                                    }
                                }

                                for (int i = 0; i < unAssignedQuantity; i++)
                                {
                                    Unit dataUnit = listUnit.ElementAtOrDefault(indexData);
                                    if (dataUnit != null)
                                    {
                                        dataUnit.MPSID = item.ID;
                                        dataUnit.MPSDueDate = mpsDueDate;
                                    }
                                    indexData++;
                                }
                            }
                        }

                        if (listUnit.ElementAtOrDefault(indexData) != null)
                        {
                            for (int i = indexData; i < listUnit.Count(); i++)
                            {
                                Unit dataUnit = listUnit.ElementAt(i);
                                dataUnit.MPSDueDate = null;
                            }
                        }
                        db.SaveChanges();
                    }
                    return Json(new { success = true, responseText = "Value successfully updated" }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { success = true, responseText = "No Value updated" }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                ViewBag.Exception(ex);
            }
            return View("Error");
        }


        /// <summary>
        /// Merupakan fungsi yang digunakan untuk menambahkan data MPS ke table MasterPlanSchedule
        /// </summary>
        /// <param name="dataMPS">Sebuah object MasterPlanSchedule yang berisikan data-data MasterPlanSchedule yang akan ditambahkan</param>
        /// <param name="username">sebuah string yang berisikan nama username dari user yang sedang login</param>
        /// <param name="now">sebuh data bertipe datetime yang bersisikan tanggal sekarang</param>
        public void AddMPSData(MasterPlanSchedule dataMPS, string username, DateTime now)
        {
            MasterPlanSchedule newMPSData = new MasterPlanSchedule();

            newMPSData.PROID = dataMPS.PROID;
            newMPSData.Week = dataMPS.Week;
            newMPSData.Month = dataMPS.Month;
            newMPSData.Year = dataMPS.Year;
            newMPSData.PlannedQuantity = dataMPS.PlannedQuantity;
            newMPSData.EndWorkingDate = dataMPS.EndWorkingDate;
            newMPSData.CreatedBy = newMPSData.LastModifiedBy = username;
            newMPSData.Created = newMPSData.LastModified = now;

            db.MasterPlanSchedules.Add(newMPSData);
        }

        /// <summary>
        /// Merupakan fungsi yang digunakan untuk mengubah/memperbaharui data dari sebuah MPS
        /// </summary>
        /// <param name="newDataMPS">sebuah object bertipe MasterPlanSchedule yang menyimpan informasi data MPS yang baru</param>
        /// <param name="currenDataMPS">sebuah object bertipe MasterPlanSchedule yang menyimpan informasi data MPS yang saat ini di database</param>
        /// <param name="username">sebuah string yang berisikan nama username dari user yang sedang login</param>
        /// <param name="now">sebuh data bertipe datetime yang bersisikan tanggal sekarang</param>
        /// <returns>sebuah object MasterPlanSchedule yang berisikan data MPS yang terbaru</returns>
        public MasterPlanSchedule UpdateMPSData(MasterPlanSchedule newDataMPS, MasterPlanSchedule currenDataMPS, string username, DateTime now)
        {
            currenDataMPS.PlannedQuantity = newDataMPS.PlannedQuantity;
            currenDataMPS.EndWorkingDate = newDataMPS.EndWorkingDate;
            currenDataMPS.LastModified = now;
            currenDataMPS.LastModifiedBy = username;

            return currenDataMPS;
        }

        /// <summary>
        /// Merupakan fungsi yang digunakan untuk menghapus data dari sebuah MPS
        /// </summary>
        /// <param name="currenDataMPS">sebuah object bertipe MasterPlanSchedule yang menyimpan informasi data MPS yang akan dihapus</param>
        /// <param name="username">sebuah string yang berisikan nama username dari user yang sedang login</param>
        /// <param name="now">sebuh data bertipe datetime yang bersisikan tanggal sekarang</param>
        public void DeleteMPSData(MasterPlanSchedule currenDataMPS, string username, DateTime now)
        {
            List<Process> processWillbeDeleted = new List<Process>();
            foreach (Unit unitItem in currenDataMPS.Units)
            {
                unitItem.MPSID = null;
                unitItem.MPSDueDate = null;
                unitItem.SFSDueDate = null;
                unitItem.LastModified = now;
                unitItem.LastModifiedBy = username;

                processWillbeDeleted.AddRange(unitItem.Processes);
            }

            db.Processes.RemoveRange(processWillbeDeleted);
            db.MasterPlanSchedules.Remove(currenDataMPS);
        }

        /// <summary>
        /// Merupakan fungsi yang digunakan untuk mengubah data unit-unit menjadi tidak aktif
        /// </summary>
        /// <param name="unitList">sebuah list of unit yang berisikan daftar unit yang akan diubah jadi tidak aktif</param>
        /// <returns>sebuah action result yang mengembalikan json yang berisikan status perubahan apakah berhasil atau tidak</returns>
        [CustomAuthorize(Roles = RoleNames.SuperAdmin + "," + RoleNames.PPC)]
        public ActionResult DeactivateUnit(List<Unit> unitList)
        {
            try
            {
                var username = User.Identity.Name;
                DateTime now = DateTime.Now;

                foreach (Unit item in unitList)
                {
                    Unit unitData = db.Units.Find(item.ID);
                    unitData.IsHold = true;
                    unitData.ReasonIssueID = item.ReasonIssueID;
                    unitData.LastModified = now;
                    unitData.LastModifiedBy = username;

                    if (unitData.MasterPlanSchedule != null)
                    {
                        unitData.MasterPlanSchedule.PlannedQuantity -= 1;
                        unitData.MasterPlanSchedule.LastModified = now;
                        unitData.MasterPlanSchedule.LastModifiedBy = username;
                    }
                }
                db.SaveChanges();

                return Json(new { success = true, responseText = "Value successfully updated" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ViewBag.Exception(ex);
            }
            return View("Error");
        }

        /// <summary>
        /// Merupakan fungsi yang digunakan untuk mengubah data unit-unit menjadi aktif
        /// </summary>
        /// <param name="unitID">sebuah bilangan integer yang berarti id dari Unit yang akan diubah</param>
        /// <returns>sebuah action result yang mengembalikan json yang berisikan status perubahan apakah berhasil atau tidak</returns>
        [CustomAuthorize(Roles = RoleNames.SuperAdmin + "," + RoleNames.PPC)]
        public ActionResult ReactivateUnit(int unitID)
        {
            try
            {
                var username = User.Identity.Name;
                DateTime now = DateTime.Now;

                Unit unitData = db.Units.Find(unitID);
                if (unitData != null)
                {
                    unitData.IsHold = false;
                    unitData.LastModified = now;
                    unitData.LastModifiedBy = username;

                    if (unitData.MasterPlanSchedule != null)
                    {
                        unitData.MasterPlanSchedule.PlannedQuantity += 1;
                        unitData.MasterPlanSchedule.LastModified = now;
                        unitData.MasterPlanSchedule.LastModifiedBy = username;
                    }
                    db.SaveChanges();

                    return Json(new { success = true, responseText = "Value successfully updated", newCSSClass = unitData.MPSCategoryCSSClass }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { success = true, responseText = "Value not successfully updated" }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                ViewBag.Exception(ex);
            }
            return View("Error");
        }

        /// <summary>
        /// Merupakan fungsi yang digunakan untuk mendapatkan daftar data minggu number dalam sebuah bulan
        /// </summary>
        /// <param name="month">sebuah bilangan integer yang berarti bulan</param>
        /// <param name="year">sebuah bilangan integer yang berarti tahun</param>
        /// <returns>Daftar Week Number dalam sebuah bulan dan tahun yang udah dipilih</returns>
        private List<WeekNumber> GetWeekNumberListofMonths(int month, int year)
        {
            List<WeekNumber> weekNumberListOfMonth = new List<WeekNumber>();

            CultureInfo indonesiaCI = new CultureInfo("en-US");
            var calendar = indonesiaCI.Calendar;

            var firstDayOfWeek = indonesiaCI.DateTimeFormat.FirstDayOfWeek;
            var weekPeriods =
            Enumerable.Range(1, calendar.GetDaysInMonth(year, month))
                      .Select(d =>
                      {
                          var date = new DateTime(year, month, d);
                          var weekNumInYear = calendar.GetWeekOfYear(date, CalendarWeekRule.FirstDay, firstDayOfWeek);
                          return new { date, weekNumInYear };
                      })
                      .GroupBy(x => x.weekNumInYear)
                      .Select(x => new { DateFrom = x.First().date, To = x.Last().date })
                      .ToList();

            int weekNumber = 0;
            DateTime startDateWeek;
            DateTime endDateWeek;
            DateTime startWorkDate;
            DateTime endWorkDate;

            //mpsActualList.Add(new MPSActual { Week = 0, Month = month, Year = year });
            foreach (var weekGroup in weekPeriods)
            {
                //debugViewMessage += " <br> =ini masuk kok "+weekGroup.DateFrom.ToShortDateString()+"--"+weekGroup.To.ToShortDateString();
                if (Convert.ToInt32(weekGroup.DateFrom.DayOfWeek) <= 3 && Convert.ToInt32(weekGroup.To.DayOfWeek) >= 3)
                {
                    weekNumber++;
                    //debugViewMessage += "#week (" + weekNumber + ") ";
                    endWorkDate = weekGroup.To.AddDays((int)DayOfWeek.Friday - Convert.ToInt32(weekGroup.To.DayOfWeek));
                    startWorkDate = weekGroup.DateFrom.AddDays((int)DayOfWeek.Monday - Convert.ToInt32(weekGroup.DateFrom.DayOfWeek));
                    endDateWeek = weekGroup.To.AddDays((int)DayOfWeek.Saturday - Convert.ToInt32(weekGroup.To.DayOfWeek));
                    startDateWeek = weekGroup.DateFrom.AddDays((int)DayOfWeek.Sunday - Convert.ToInt32(weekGroup.DateFrom.DayOfWeek));

                    weekNumberListOfMonth.Add(new WeekNumber { Number = weekNumber, EndsWorkingDate = endWorkDate, StartsWorkingDate = startWorkDate, EndsDateWeek = endDateWeek, StartsDateWeek = startDateWeek });
                    //mpsActualList.Add(new MPSActual { Week = weekNumber, Month = month, Year = year, EndsDateWeek = endDateWeek, StartsDateWeek = startDateWeek });
                }
            }

            return weekNumberListOfMonth;
        }
    }
}