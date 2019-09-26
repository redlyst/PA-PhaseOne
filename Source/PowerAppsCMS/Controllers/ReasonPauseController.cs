using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PowerAppsCMS.Models;
using PagedList;
using PowerAppsCMS.CustomAuthentication;
using PowerAppsCMS.Constants;

namespace PowerAppsCMS.Controllers
{
    /// <summary>
    /// ReasonPauseController berfungsi sebagai CRUD reason pause
    /// </summary>
    [CustomAuthorize(Roles = RoleNames.Administrator + "," + RoleNames.SuperAdmin)]
    public class ReasonPauseController : Controller
    {
        private PowerAppsCMSEntities db = new PowerAppsCMSEntities();

        /// <summary>
        /// Method Index berfungsi untuk menampilkan list semua reason pause
        /// </summary>
        /// <param name="searchName">Parameter searchName digunakan untuk mencari reason pause berdasarkan nama yang di input</param>
        /// <param name="currentFilter">Parameter yang digunakan untuk mengatur filter ketika user membuka halaman saat ini atau berikutnya</param>
        /// <param name="page">Parameter nomor halaman</param>
        /// <returns>Menampilkan list semua reason pause</returns>
        // GET: ReasonPause
        public ActionResult Index(string searchName, string currentFilter, int? page)
        {
            if (searchName != null)
            {
                page = 1;
            }
            else
            {
                searchName = currentFilter;
            }

            ViewBag.CurrentFilter = searchName;

            var reasonpauseList = from x in db.ReasonPauses
                                  select x;

            if (!String.IsNullOrEmpty(searchName))
            {
                reasonpauseList = reasonpauseList.Where(x => x.Name.Contains(searchName));
            }

            int pageSize = 10;
            int pageNumber = (page ?? 1);

            ViewBag.PageNumber = pageNumber.ToString();
            ViewBag.ItemperPage = pageSize.ToString();
            ViewBag.Page = page.ToString();

            return View(reasonpauseList.OrderBy(x => x.ID).ToPagedList(pageNumber, pageSize)); ;
        }

        /// <summary>
        /// Method Create berfungsi untuk menampilkan halaman create
        /// </summary>
        /// <returns>Menampilkan halaman create</returns>
        // GET: ReasonPause/Create
        public ActionResult Create()
        {
            return View();
        }

        /// <summary>
        /// Method Post Create berfungsi untuk menambah data baru di reason pause
        /// </summary>
        /// <param name="reasonPause">Parameter reason pause merupakan parameter dari model reason pause</param>
        /// <returns>Jika data baru berhasil diinput, maka web akan menavigasikan ke halaman index</returns>
        // POST: ReasonPause/Create
        [HttpPost]
        public ActionResult Create(ReasonPause reasonPause)
        {
            var username = User.Identity.Name;
            try
            {
                var existReasonPause = db.ReasonPauses.Where(x => x.Name == reasonPause.Name).SingleOrDefault();

                if (existReasonPause != null)
                {
                    ViewBag.ExistReasonPause = "Reason issue already exist";
                    return View();
                }
                else
                {
                    reasonPause.Created = DateTime.Now;
                    reasonPause.CreatedBy = username;
                    reasonPause.LastModified = DateTime.Now;
                    reasonPause.LastModifiedBy = username;
                    db.ReasonPauses.Add(reasonPause);
                    db.SaveChanges();

                    ViewBag.Message = "Success";
                    return View();
                }
            }
            catch (Exception ex)
            {
                ViewBag.Exception = ex;
                ViewBag.ErrorMessage = "An error occured, please check your data input and try again";
            }
            return View("Error");
        }

        /// <summary>
        /// Method Edit berfungsi untuk menampilkan halaman edit
        /// </summary>
        /// <param name="id">Parameter id merupakan id dari reason pause</param>
        /// <param name="page">Parameter nomor halaman</param>
        /// <returns>Menampilkan halaman edit</returns>
        // GET: ReasonPause/Edit/5
        public ActionResult Edit(int? id, int? page)
        {
            if (id.HasValue)
            {
                var reasonPauseData = db.ReasonPauses.Find(id);
                if (reasonPauseData != null)
                {
                    ViewBag.Page = page.ToString();
                    return View(reasonPauseData);
                }
                else
                {
                    ViewBag.ErrorMessage = "Sorry we couldn't find this product group";
                    return View("Error");
                }
            }
            else
            {
                return RedirectToAction("Index", "ReasonPause");
            }

        }

        /// <summary>
        /// Method Post Edit berfungsi untuk mengubah sebuah data di reason pause
        /// </summary>
        /// <param name="id">Parameter id merupakan id dari reason pause</param>
        /// <param name="reasonPause">Parameter reason pause merupakan parameter dari model reason pause</param>
        /// <returns>Jika data berhasil dirubah, maka web akan menavigasikan kehalaman index</returns>
        // POST: ReasonPause/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, ReasonPause reasonPause)
        {
            var username = User.Identity.Name;
            var reasonPauseData = db.ReasonPauses.Find(id);
            try
            {
                var existReasonPause = db.ReasonPauses.Where(x => x.Name == reasonPause.Name && x.ID != reasonPause.ID).SingleOrDefault();

                // Edit Reason Issue
                if (existReasonPause != null)
                {
                    ViewBag.ExistReasonPause = "Group product already exist";
                    return View(reasonPauseData);
                }
                else
                {
                    reasonPauseData.Name = reasonPause.Name;
                    reasonPauseData.LastModified = DateTime.Now;
                    reasonPauseData.LastModifiedBy = username;
                    db.SaveChanges();

                    ViewBag.Message = "Success";
                    return View(reasonPauseData);
                }
            }
            catch (Exception ex)
            {
                ViewBag.Exception = ex;
                ViewBag.ErrorMessage = "An error occured, please check your data input and try again";
            }
            return View("Error");
        }

        /// <summary>
        /// Method Delete berfungsi untuk menghapus sebuah data dari reason pause
        /// </summary>
        /// <param name="id">Parameter id merupakan id dari reason pause</param>
        /// <returns>Jika data berhasil dihapus, maka web akan menavigasikan ke halaman index</returns>
        // GET: ReasonIssue/Delete/5
        public ActionResult Delete(int id)
        {
            try
            {
                var reasonpauseData = db.ReasonPauses.Find(id);
                db.ReasonPauses.Remove(reasonpauseData);
                db.SaveChanges();

                return RedirectToAction("Index", "ReasonPause");
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "could not be deleted because there are related Sub Categories on this Reason Pause and try again";
                ViewBag.Exception = ex;
            }
            return View("Error");
        }
    }
}