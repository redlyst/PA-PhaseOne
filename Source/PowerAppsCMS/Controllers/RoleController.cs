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
    /// RoleController berfungsi sebagai CRUD role
    /// </summary>
    [CustomAuthorize(Roles = RoleNames.Administrator + "," + RoleNames.SuperAdmin)]
    public class RoleController : Controller
    {
        private PowerAppsCMSEntities db = new PowerAppsCMSEntities();

        /// <summary>
        /// Method Index berfungsi untuk menampilkan list role
        /// </summary>
        /// <param name="searchName">Parameter nama role yang digunakan untuk mencari role berdasarkan nama yang di input</param>
        /// <param name="currentFilter">Parameter yang digunakan untuk mengatur filter ketika user membuka halaman saat ini atau berikutnya</param>
        /// <param name="page">Parameter nomor halaman</param>
        /// <returns>List semua role</returns>
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

            var RoleList = from x in db.Roles
                           where x.ID > 0
                           select x;

            if (!String.IsNullOrEmpty(searchName))
            {
                RoleList = RoleList.Where(x => x.Name.Contains(searchName));
            }

            int pageSize = 10;
            int pageNumber = (page ?? 1);

            ViewBag.PageNumber = pageNumber;
            ViewBag.ItemperPage = pageSize;
            ViewBag.Page = page.ToString();
            return View(RoleList.OrderBy(x => x.Name).ToPagedList(pageNumber, pageSize));
        }

        /// <summary>
        /// Method Create berfungsi untuk menampilkan halaman create role
        /// </summary>
        /// <returns>Menampilkan halaman create role</returns>
        public ActionResult Create()
        {
            ViewBag.ParentID = new SelectList(db.Roles.Where(x => x.ID > 0).OrderBy(x => x.Name), "ID", "Name");
            return View();
        }

        /// <summary>
        /// Method Post Create berfungsi untuk menambah role baru
        /// </summary>
        /// <param name="role">Parameter role digunakan oleh object role yang isinya adalah data yang berhubungan dengan role</param>
        /// <returns>Jika data berhasil di masukkan, maka akan dinavigasikan ke halaman index</returns>
        [HttpPost]
        public ActionResult Create(Role role)
        {
            ViewBag.ParentID = new SelectList(db.Roles.Where(x => x.ID > 0).OrderBy(x => x.Name), "ID", "Name");
            try
            {
                // Add Role
                var username = User.Identity.Name;
                Role existRole = db.Roles.Where(x => x.Name == role.Name).SingleOrDefault();

                if (existRole == null)
                {
                    role.Created = DateTime.Now;
                    role.CreatedBy = username;
                    role.LastModified = DateTime.Now;
                    role.LastModifiedBy = username;
                    db.Roles.Add(role);
                    db.SaveChanges();

                    ViewBag.Message = "Success";
                    return View();
                }
                else
                {
                    ViewBag.ExistRole = "Role already exist";
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
        /// <param name="id">parameter id yang merupakan id dari role</param>
        /// <param name="page">parameter nomor halaman</param>
        /// <returns>menampilkan halaman edit</returns>
        public ActionResult Edit(int? id, int? page)
        {
            if (id.HasValue)
            {
                var roleData = db.Roles.Find(id);
                if (roleData != null)
                {
                    ViewBag.ParentID = new SelectList(db.Roles.Where(x => x.ID > 0).OrderBy(x => x.Name), "ID", "Name", roleData.ParentID);
                    ViewBag.Page = page.ToString();
                    return View(roleData);
                }
                else
                {
                    ViewBag.ErrorMessage = "Sorry we couldn't find the role";
                    return View("Error");
                }
            }
            else
            {
                return RedirectToAction("Index", "Role");
            }
        }

        /// <summary>
        /// Method Post Edit berfungsi untuk mengubah data dari role
        /// </summary>
        /// <param name="id">parameter id yang merupakan id dari role</param>
        /// <param name="role">parameter role yang digunakan oleh object Role yang isinya data yang berhubungan dengan role</param>
        /// <returns>Jika data berhasil di rubah, maka akan dinavigasikan ke halaman index</returns>
        [HttpPost]
        public ActionResult Edit(int id, Role role)
        {
            Role roleData = db.Roles.Find(id);
            ViewBag.ParentID = new SelectList(db.Roles.Where(x => x.ID > 0).OrderBy(x => x.Name).OrderBy(x => x.Name), "ID", "Name", roleData.ParentID);

            try
            {
                var username = User.Identity.Name;
                Role existRole = db.Roles.Where(x => x.Name == role.Name && x.ID != role.ID).SingleOrDefault();

                // Edit Role
                if (existRole == null)
                {
                    roleData.Name = role.Name;
                    roleData.ParentID = role.ParentID;
                    roleData.HaveAccessPowerApps = role.HaveAccessPowerApps;
                    roleData.LastModified = DateTime.Now;
                    roleData.LastModifiedBy = username;
                    db.SaveChanges();

                    ViewBag.Message = "success";
                    return View(roleData);
                }
                else
                {
                    ViewBag.Message = "Role already exist";
                    return View(roleData);
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
        /// Method Delete berfungsi untuk menghapus data role
        /// </summary>
        /// <param name="id">parameter id yang merupakan id dari role</param>
        /// <returns>jika data berhasil di hapus, maka akan dinavigasikan ke halaman index</returns>
        public ActionResult Delete(int id)
        {
            try
            {
                Role role = db.Roles.Find(id);
                db.Roles.Remove(role);
                db.SaveChanges();
                return RedirectToAction("Index", "Role");
            }
            catch
            {
                ViewBag.ErrorMessage = "Role could not be deleted because there are some employees are using this role";
                return View("Error");
            }
        }
    }
}
