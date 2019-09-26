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
    /// MaterialController berfungsi untuk CRUD material
    /// </summary>
    [CustomAuthorize(Roles = RoleNames.PE + "," + RoleNames.SuperAdmin)]
    public class MaterialController : Controller
    {
        private PowerAppsCMSEntities db = new PowerAppsCMSEntities();

        /// <summary>
        /// Method Index berfungsi untuk menampilkan list semua material
        /// </summary>
        /// <param name="searchName">Parameter searchName digunakan untuk mencari material berdasarkan nama yang di input</param>
        /// <param name="currentFilter">Parameter yang digunakan untuk mengatur filter ketika user membuka halaman saat ini atau berikutnya</param>
        /// <param name="page">Parameter nomor halaman</param>
        /// <returns>Menampilkan list semua material</returns>
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

            var materialList = from x in db.Materials
                               select x;
            if (!String.IsNullOrEmpty(searchName))
            {
                materialList = materialList.Where(x => x.Name.Contains(searchName));
            }

            int pageSize = 10;
            int pageNumber = (page ?? 1);

            ViewBag.PageNumber = pageNumber.ToString();
            ViewBag.ItemperPage = pageSize.ToString();
            ViewBag.Page = page.ToString();

            return View(materialList.OrderBy(x => x.Name).ToPagedList(pageNumber, pageSize));
        }

        /// <summary>
        /// Method Details berfungsi untuk menampilkan detail dari sebuah material
        /// </summary>
        /// <param name="id">Parameter id merupakan id dari material</param>
        /// <returns>Menampilkan detail dari sebuah material</returns>
        public ActionResult Details(int id)
        {
            return View();
        }

        /// <summary>
        /// Method Create berfungsi untuk menampilkan halaman create
        /// </summary>
        /// <returns>Menampilkan halaman create</returns>
        public ActionResult Create()
        {
            return View();
        }

        /// <summary>
        /// Method Post Create berfungsi untuk menambah data baru kedalam material
        /// </summary>
        /// <param name="material">Parameter material merupakan model dari material</param>
        /// <returns>Jika data baru berhasil di input, maka web akan menavigasikan ke halaman index</returns>
        [HttpPost]
        public ActionResult Create(Material material)
        {
            try
            {
                var username = User.Identity.Name;

                // TODO: Add Material
                material.Created = DateTime.Now;
                material.CreatedBy = username;
                material.LastModified = DateTime.Now;
                material.LastModifiedBy = username;
                db.Materials.Add(material);
                db.SaveChanges();
                
                ViewBag.Message = "success";
                return View();
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "An error occured, please check your data input and try again";
                ViewBag.Exception = ex;
            }
            return View("Error");
        }

        /// <summary>
        /// Method Edit berfungsi untuk menampilkan halaman edit
        /// </summary>
        /// <param name="id">Parameter id merupakan id dari material</param>
        /// <param name="page">Parameter nomor halaman</param>
        /// <returns>Menampilkan halaman edit</returns>
        public ActionResult Edit(int? id, int? page)
        {
            if (id.HasValue)
            {
                var materialData = db.Materials.Find(id);
                if (materialData != null)
                {
                    ViewBag.Page = page.ToString();
                    return View(materialData);
                }
               else
                {
                    ViewBag.ErrorMessage = "Sorry we couldn't find this material";
                    return View("Error");
                }
            }
            else
            {
                return RedirectToAction("Index", "Material");
            }
        }

        /// <summary>
        /// Method Post Edit berfungsi untuk mengubah sebuah data di material
        /// </summary>
        /// <param name="id">Parameter id merupakan id dari material</param>
        /// <param name="material">Parameter material merupakan model dari material</param>
        /// <param name="collection">Parameter collection merupakan parameter yang digunakan oleh object FormCollection</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Edit(int id, Material material, FormCollection collection)
        {
            var username = User.Identity.Name;
            var materialData = db.Materials.Find(id);
            var currentPage = collection.GetValues("currentPage");
            ViewBag.CurrentPage = currentPage[0];
            try

            {
                // TODO: update material
                materialData.Name = material.Name;
                materialData.LastModified = DateTime.Now;
                materialData.LastModifiedBy = username;
                db.SaveChanges();

                ViewBag.Message = "success";
                return View(materialData);
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "An error occured, please check your data input and try again";
                ViewBag.Exception = ex;
            }
            return View("Error");
        }

        /// <summary>
        /// Method Delete berfungsi untuk menghapus sebuah data di material
        /// </summary>
        /// <param name="id">Parameter id merupakan id dari material</param>
        /// <returns>Jika data berhasil di hapus, maka web akan menavigasikan ke halaman index</returns>
        public ActionResult Delete(int id)
        {
            var materialData = db.Materials.Find(id);
            try
            {
                if (materialData.Components.Count() > 0)
                {
                    ViewBag.ErrorMessage = materialData.Name + " " + "can't be deleted, because some components are using this material";
                }
                db.Materials.Remove(materialData);
                db.SaveChanges();

                return RedirectToAction("Index");
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
