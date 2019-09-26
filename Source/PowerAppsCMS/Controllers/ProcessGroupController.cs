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
    /// ProcessGroupController berfungsi sebagai CRUD Process Group
    /// </summary>
    [CustomAuthorize(Roles = RoleNames.PE + "," + RoleNames.SuperAdmin)]
    public class ProcessGroupController : Controller
    {
        private PowerAppsCMSEntities db = new PowerAppsCMSEntities();

        /// <summary>
        /// Method Index befungsi untuk menampilkan list dari process group
        /// </summary>
        /// <param name="searchName">Parameter searchName digunakan untuk mencari process group berdasarkan nama yang di input</param>
        /// <param name="currentFilter">Parameter yang digunakan untuk mengatur filter ketika user membuka halaman saat ini atau berikutnya</param>
        /// <param name="page">Parameter nomor halaman</param>
        /// <returns>List semua process group</returns>
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

            var processGroupList = from x in db.ProcessGroups
                                   select x;
            if (!String.IsNullOrEmpty(searchName))
            {
                processGroupList = processGroupList.Where(x => x.Name.Contains(searchName));
            }

            int pageSize = 10;
            int pageNumber = (page ?? 1);

            ViewBag.PageNumber = pageNumber.ToString();
            ViewBag.ItemperPage = pageSize.ToString();
            ViewBag.Page = page.ToString();

            return View(processGroupList.OrderBy(x => x.Name).ToPagedList(pageNumber, pageSize));
        }

        /// <summary>
        /// Method Create berfungsi untuk menampilkan halaman create process group
        /// </summary>
        /// <returns>Menampilkan halaman create</returns>
        public ActionResult Create()
        {
            return View();
        }

        /// <summary>
        /// Method Post ProcessGroup berfungsi untuk menambah process group baru
        /// </summary>
        /// <param name="processGroup">parameter model dari process group</param>
        /// <returns>Jika data baru berhasil di input, maka user web akan menavigasikan ke halaman index</returns>
        [HttpPost]
        public ActionResult Create(ProcessGroup processGroup)
        {
            try
            {
                var username = User.Identity.Name;
                var existProcessGroup = db.ProcessGroups.Where(x => x.Name == processGroup.Name).SingleOrDefault();

                if (existProcessGroup != null)
                {
                    ViewBag.ProcessGroupExist = "Process Group already exist";
                    return View();
                }
                else
                {
                    processGroup.Created = DateTime.Now;
                    processGroup.CreatedBy = username;
                    processGroup.LastModified = DateTime.Now;
                    processGroup.LastModifiedBy = username;
                    db.ProcessGroups.Add(processGroup);
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
        /// <param name="id">Parameter id yang merupakan id dari ProcessGroup</param>
        /// <param name="page">Parameter nomor halaman</param>
        /// <returns>Menampilkan halaman edit</returns>
        public ActionResult Edit(int? id, int? page)
        {
            if (id.HasValue)
            {
                var processGroupData = db.ProcessGroups.Find(id);
                if (processGroupData != null)
                {
                    ViewBag.Page = page.ToString();
                    return View(processGroupData);
                }
                else
                {
                    ViewBag.ErrorMessage = "Sorry we couldn't find this process group";
                    return View("Error");
                }
            }
            else
            {
                return RedirectToAction("Index", "ProcessGroup");
            }

        }

        /// <summary>
        /// Method Post Edit berfungsi untuk mengubah data dari process group
        /// </summary>
        /// <param name="id">parameter id yang merupakan id dari process group</param>
        /// <param name="processGroup">parameter model dari process group</param>
        /// <returns>Jika data berhasil di rubah, maka web akan menavigasikan ke halaman index</returns>
        [HttpPost]
        public ActionResult Edit(int id, ProcessGroup processGroup)
        {
            try
            {
                var username = User.Identity.Name;
                var existProcessGroup = db.ProcessGroups.Where(x => x.ID != processGroup.ID && x.Name == processGroup.Name).FirstOrDefault();

                if (existProcessGroup != null)
                {
                    ViewBag.ProcessGroupExist = "Process Group already exist";
                    return View();
                }
                else
                {
                    // TODO: Edit ProcessGroup
                    var processGroupData = db.ProcessGroups.Find(id);
                    processGroupData.Name = processGroup.Name;
                    processGroupData.LastModified = DateTime.Now;
                    processGroupData.LastModifiedBy = username;
                    db.SaveChanges();

                    ViewBag.Message = "success";
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
        /// Method Delete berfungsi untuk menghapus data process group
        /// </summary>
        /// <param name="id">parameter id yang merupakan id dari process group</param>
        /// <returns></returns>
        public ActionResult Delete(int id)
        {
            ProcessGroup processGroup = db.ProcessGroups.Find(id);
            try
            {
                if (processGroup.MasterProcesses.Count() > 0)
                {
                    ViewBag.ErrorMessage = processGroup.Name + " " + "can't be deleted, because there are some process on this process group";
                    return View("Error");
                }
                else
                {
                    db.ProcessGroups.Remove(processGroup);
                    db.SaveChanges();
                    return RedirectToAction("Index", "ProcessGroup");
                }
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "An error occured, please check your data input and try again";
                ViewBag.Exception = ex;
            }
            return View("Error");
        }
    }
}
