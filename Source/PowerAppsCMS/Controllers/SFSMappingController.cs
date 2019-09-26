using System;
using System.Linq;
using System.Web.Mvc;
using PowerAppsCMS.Models;
using PagedList;
using PowerAppsCMS.CustomAuthentication;
using PowerAppsCMS.Constants;

namespace PowerAppsCMS.Controllers
{
    /// <summary>
    /// SFSMappingController berfungsing untuk memapping antara PRO suatu product dengan chasis number
    /// </summary>
    /// <remarks>Di dalam SFSMappingController terdapat method edit chasis number dan delete</remarks>
    [CustomAuthorize(Roles = RoleNames.PPC + "," + RoleNames.SuperAdmin)]
    public class SFSMappingController : Controller
    {
        private PowerAppsCMSEntities db = new PowerAppsCMSEntities();

        /// <summary>
        /// Method Index berfungsi untuk menampilkan list semua unit
        /// </summary>
        /// <param name="searchSerialNumber">Parameter searchSerialNumber digunakan untuk mencari unit berdasarkan serial number yang di input</param>
        /// <param name="searchChasisNumber">Parameter searchChasisNumber digunakan untuk mencari unit berdasarkan chasis number yang di input</param>
        /// <param name="currentFilter"></param>
        /// <param name="page">Parameter nomor halaman</param>
        /// <returns>Menampilkan list semua unit</returns>
        public ActionResult Index(string searchSerialNumber, string searchChasisNumber, string currentFilterSearchSerialNumber, string currentFilterSearchChasisNumber, int? page)
        {
            if (searchSerialNumber != null && searchChasisNumber != null)
            {
                page = 1;
            }
            else
            {
                searchSerialNumber = currentFilterSearchSerialNumber;
                searchChasisNumber = currentFilterSearchChasisNumber;
            }

            ViewBag.CurrentFilterSearchSerialNumber = searchSerialNumber;
            ViewBag.CurrentFilterSearchChasisNumber = searchChasisNumber;
            var unitList = from x in db.Units
                           select x;

            if (!string.IsNullOrEmpty(searchSerialNumber) && !string.IsNullOrEmpty(searchChasisNumber))
            {
                unitList = unitList.Where(x => x.SerialNumber.Contains(searchSerialNumber) && x.ChasisNumber.Contains(searchChasisNumber));
            }
            else if (!string.IsNullOrEmpty(searchSerialNumber))
            {
                unitList = unitList.Where(x => x.SerialNumber.Contains(searchSerialNumber));
            }
            else if (!string.IsNullOrEmpty(searchChasisNumber))
            {
                unitList = unitList.Where(x => x.ChasisNumber.Contains(searchChasisNumber));
            }
            int pageSize = 10;
            int pageNumber = (page ?? 1);

            ViewBag.PageNumber = pageNumber.ToString();
            ViewBag.ItemperPage = pageSize.ToString();

            return View(unitList.OrderByDescending(x => x.Created).ToPagedList(pageNumber, pageSize));
        }

        /// <summary>
        /// Method EditChasisNumber berfungsi untuk mengubah chasis number di unit
        /// </summary>
        /// <param name="unit">Parameter unit merupakan parameter model dari unit</param>
        /// <returns>Chasis number di unit berhasil dirubah</returns>
        public ActionResult EditChasisNumber(Unit unit)
        {
            var existChasisNumber = db.Units.Where(x => x.ChasisNumber == unit.ChasisNumber).FirstOrDefault();
            var unitData = db.Units.Find(unit.ID);
            var username = User.Identity.Name;
            try
            {
                if (existChasisNumber == null)
                {
                    unitData.ChasisNumber = unit.ChasisNumber;
                    unitData.LastModified = DateTime.Now;
                    unitData.LastModifiedBy = username;
                    db.SaveChanges();
                    return Json(new { success = true, responseText = "Chasis number successfully updated", chasisNumber = unitData.ChasisNumber }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { success = false, responseText = "Update chasis number failed, because chasis number already exist" }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                ViewBag.Exception = ex;
            }
            return View("Error");
        }
    }
}