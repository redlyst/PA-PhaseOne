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
    /// ReasonIssueController berfungsi sebagai CRUD reason issue
    /// </summary>
    [CustomAuthorize(Roles = RoleNames.Administrator + "," + RoleNames.SuperAdmin)]
    public class ReasonIssueController : Controller
    {
        private PowerAppsCMSEntities db = new PowerAppsCMSEntities();

        /// <summary>
        /// Method Index berfungsi untuk menampilkan list semua reason issue
        /// </summary>
        /// <param name="searchName">Parameter searchName digunakan untuk mencari reason issue berdasarkan nama yang diinput</param>
        /// <param name="currentFilter">Parameter yang digunakan untuk mengatur filter ketika user mebuka halaman saat ini atau berikutnya</param>
        /// <param name="page">Parameter nomor halaman</param>
        /// <returns>Menampilkan list semua reason issue</returns>
        // GET: ReasonIssue
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

            var reasonissueList = from x in db.ReasonIssues
                                  select x;

            if (!String.IsNullOrEmpty(searchName))
            {
                reasonissueList = reasonissueList.Where(x => x.Name.Contains(searchName));
            }

            int pageSize = 10;
            int pageNumber = (page ?? 1);

            ViewBag.PageNumber = pageNumber.ToString();
            ViewBag.ItemperPage = pageSize.ToString();
            ViewBag.Page = page.ToString();

            return View(reasonissueList.OrderBy(x => x.ID).ToPagedList(pageNumber, pageSize)); ;
        }

        /// <summary>
        /// Method Create berfungsi untuk menampilkan halaman create
        /// </summary>
        /// <returns>Menampilkan halaman create</returns>
        // GET: ReasonIssue/Create
        public ActionResult Create()
        {
            return View();
        }

        /// <summary>
        /// Method Post Create berfungsi untuk menambah data baru di reason issue
        /// </summary>
        /// <param name="reasonIssue">Parameter reason issue merupakan parameter dari model reason issue</param>
        /// <returns>Jika data baru berhasil diinput, maka web akan menavigasi ke halaman index</returns>
        // POST: ReasonIssue/Create
        [HttpPost]
        public ActionResult Create(ReasonIssue reasonIssue)
        {
            var username = User.Identity.Name;
            try
            {
                var existReasonIssue = db.ReasonIssues.Where(x => x.Name == reasonIssue.Name).SingleOrDefault();

                if (existReasonIssue != null)
                {
                    ViewBag.ExistReasonIssue = "Reason issue already exist";
                    return View();
                }
                else
                {
                    reasonIssue.Created = DateTime.Now;
                    reasonIssue.CreatedBy = username;
                    reasonIssue.LastModified = DateTime.Now;
                    reasonIssue.LastModifiedBy = username;
                    db.ReasonIssues.Add(reasonIssue);
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
        /// Method Edit berfungsi untuk menampilkan halaman Edit
        /// </summary>
        /// <param name="id">Parameter id merupakan id dari reason issue</param>
        /// <param name="page">Parameter nomor halaman</param>
        /// <returns>Menampilkan halaman edit</returns>
        // GET: ReasonIssue/Edit/5
        public ActionResult Edit(int? id, int? page)
        {
            if (id.HasValue)
            {
                var reasonIssueData = db.ReasonIssues.Find(id);
                if (reasonIssueData != null)
                {
                    ViewBag.Page = page.ToString();
                    return View(reasonIssueData);
                }
                else
                {
                    ViewBag.ErrorMessage = "Sorry we couldn't find this product group";
                    return View("Error");
                }
            }
            else
            {
                return RedirectToAction("Index", "ReasonIssue");
            }

        }

        /// <summary>
        /// Method Post Edit berfungsi untuk mengubah sebuah data di reason issue
        /// </summary>
        /// <param name="id">Parameter id merupakan id dari reason issue</param>
        /// <param name="reasonIssue">Parameter reasonIssue merupakan parameter dari model reasonissue</param>
        /// <returns>Jika data berhasil dirubah, maka web akan menavigasi ke halaman index</returns>
        // POST: ReasonIssue/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, ReasonIssue reasonIssue)
        {
            var username = User.Identity.Name;
            var reasonIssueData = db.ReasonIssues.Find(id);
            try
            {
                var existReasonIssue = db.ReasonIssues.Where(x => x.Name == reasonIssue.Name && x.ID != reasonIssue.ID).SingleOrDefault();

                // Edit Reason Issue
                if (existReasonIssue != null)
                {
                    ViewBag.ExistReasonIssue = "Group product already exist";
                    return View(reasonIssueData);
                }
                else
                {
                    reasonIssueData.Name = reasonIssue.Name;
                    reasonIssueData.LastModified = DateTime.Now;
                    reasonIssueData.LastModifiedBy = username;
                    db.SaveChanges();

                    ViewBag.Message = "Success";
                    return View(reasonIssueData);
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
        /// Method Delete berfungsi untuk menghapus sebuah data dari reason issue
        /// </summary>
        /// <param name="id">Parameter id merupakan id dari reason issue</param>
        /// <returns>Jika data berhasil dihapus, maka web akan menavigasi ke halaman index</returns>
        // GET: ReasonIssue/Delete/5
        public ActionResult Delete(int id)
        {
            try
            {
                var reasonissueData = db.ReasonIssues.Find(id);
                db.ReasonIssues.Remove(reasonissueData);
                db.SaveChanges();

                return RedirectToAction("Index", "ReasonIssue");
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "could not be deleted because there are related Sub Categories on this Reason Issue and try again";
                ViewBag.Exception = ex;
            }
            return View("Error");
        }
        //public ActionResult Delete(int id)
        //{
        //    ReasonIssue reasonIssue = db.ReasonIssues.Find(id);
        //    try
        //    {
        //        var reasonissueData = db.ReasonIssues.Find(id);
        //        db.ReasonIssues.Remove(reasonissueData);
        //        db.SaveChanges();

        //        return RedirectToAction("Index");
        //    }
        //    catch
        //    {
        //        ViewBag.ErrorMessage = "Can't delete this Reason Issue cause has been used by another component";
        //    }
        //    return View("Error");
        //}

        //// POST: ReasonIssue/Delete/5
        //[HttpPost]
        //public ActionResult Delete(int id, FormCollection collection)
        //{
        //    try
        //    {
        //        // TODO: Add delete logic here

        //        return RedirectToAction("Index");
        //    }
        //    catch
        //    {
        //        return View();
        //    }
        //}
    }
}