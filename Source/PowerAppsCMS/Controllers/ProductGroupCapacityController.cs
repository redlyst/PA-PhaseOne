using System;
using System.Web.Mvc;
using PowerAppsCMS.Constants;
using PowerAppsCMS.CustomAuthentication;
using PowerAppsCMS.Models;

namespace PowerAppsCMS.Controllers
{
    /// <summary>
    /// ProductGroupCapacityController berfungsi sebagai CRUD product group capacity
    /// </summary>
    [CustomAuthorize(Roles = RoleNames.PE + "," + RoleNames.SuperAdmin)]
    public class ProductGroupCapacityController : Controller
    {
        private PowerAppsCMSEntities db = new PowerAppsCMSEntities();
        
        /// <summary>
        /// Method Create berfungsi untuk menampilkan halaman create
        /// </summary>
        /// <param name="productGroupId">Parameter productGroupId merupakan id dari product group</param>
        /// <param name="page">Parameter nomor halaman</param>
        /// <returns></returns>
        public ActionResult Create(int productGroupId, int? page)
        {
            ViewBag.ProductGroupID = Convert.ToString(productGroupId);
            ViewBag.Page = page.ToString();
            return View();
        }

        /// <summary>
        /// Method Post Create berfungsi untuk menambah data baru product group capacity
        /// </summary>
        /// <param name="productGroupId">Parameter productGroupId merupakan id dari product group</param>
        /// <param name="productGroupCapacity">Parameter model dari product group capacity</param>
        /// <param name="collection">Parameter yang digunakan oleh object FormCollection</param>
        /// <returns>Jika data baru berhasil di tambahkan, maka web akan menavigasikan ke halaman detail product group</returns>
        [HttpPost]
        public ActionResult Create(int productGroupId, ProductGroupCapacity productGroupCapacity, FormCollection collection)
        {
            ViewBag.ProductGroupID = Convert.ToString(productGroupId);
            var username = User.Identity.Name;
            var startMonth = productGroupCapacity.StartMonth;
            var groupProductCurrentPage = collection.GetValues("currentPage");
            ViewBag.CurrentPage = groupProductCurrentPage[0];
            try
            {
                var currentMonth = DateTime.Now.Month;
                var currentYear = DateTime.Now.Year;
                if (currentMonth == productGroupCapacity.StartMonth.Month && productGroupCapacity.StartMonth.Year == currentYear)
                {
                    ViewBag.SameMonth = "Start month can't be same as current month";
                }
                else if (productGroupCapacity.Capacity < 0)
                {
                    ViewBag.Capacity = "Capacity can't be less than 0";
                }
                else
                {
                    // TODO: Add product group capacity
                    productGroupCapacity.ProductGroupID = productGroupId;
                    productGroupCapacity.StartMonth = new DateTime(productGroupCapacity.StartMonth.Year, productGroupCapacity.StartMonth.Month, 1);
                    productGroupCapacity.Created = DateTime.Now;
                    productGroupCapacity.CreatedBy = username;
                    productGroupCapacity.LastModified = DateTime.Now;
                    productGroupCapacity.LastModifiedBy = username;
                    db.ProductGroupCapacities.Add(productGroupCapacity);
                    db.SaveChanges();

                    ViewBag.Message = "success";
                    return View();
                }
            }
            catch (Exception ex)
            {
                ViewBag.Exception = ex;
            }
            return View();
        }

        /// <summary>
        /// Method Edit befungsi untuk menampilkan halaman edit
        /// </summary>
        /// <param name="id">Parameter id merupakan id dari product group capacity</param>
        /// <returns>Menampilkan halaman edit</returns>
        public ActionResult Edit(int? id)
        {
            var productGroupCapacityData = db.ProductGroupCapacities.Find(id);
            if (id.HasValue)
            {
                if (productGroupCapacityData != null)
                {
                    return View(productGroupCapacityData);
                }
                else
                {
                    ViewBag.ErrorMessage = "Sorry we couldn't find the capacity setting for any products";
                    return View("Error");
                }
                
            }
            else
            {
                return RedirectToAction("Details", "GroupProduct", new { id = productGroupCapacityData.ProductGroupID });
            }
            
        }

        /// <summary>
        /// Method Post Edit berfungsi untuk mengubah sebuah data dari product group capacity
        /// </summary>
        /// <param name="id">Parameter id merupakan id dari product group capacity</param>
        /// <param name="productGroupCapacity">Parameter model dari product group capacity</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Edit(int id, ProductGroupCapacity productGroupCapacity)
        {
            var productGroupCapacityData = db.ProductGroupCapacities.Find(id);
            ViewBag.ProductGroupID = Convert.ToString(productGroupCapacityData.ProductGroupID);

            try
            {
                var username = User.Identity.Name;
                var currentMonth = DateTime.Now.Month;

                // TODO: Edit product group capacity

                if (currentMonth == productGroupCapacity.StartMonth.Month)
                {
                    ViewBag.SameMonth = "Start month can't be same as current month";
                    return View(productGroupCapacityData);
                }
                else if(productGroupCapacity.Capacity < 0)
                {
                    ViewBag.Capacity = "Capacity can't be less than 0";
                    return View(productGroupCapacityData);
                }
                else
                {
                    productGroupCapacityData.Capacity = productGroupCapacity.Capacity;
                    productGroupCapacityData.StartMonth = new DateTime(productGroupCapacity.StartMonth.Year, productGroupCapacity.StartMonth.Month, 1);
                    productGroupCapacityData.LastModified = DateTime.Now;
                    productGroupCapacityData.LastModifiedBy = username;
                    db.SaveChanges();

                    ViewBag.Message = "success";
                    return View(productGroupCapacityData);
                }
            }
            catch (Exception ex)
            {
                ViewBag.Exception = ex;
            }
            return View();
        }

        /// <summary>
        /// Method Delete berfungsi untuk menghapus sebuah data dari product group capacity
        /// </summary>
        /// <param name="id">Parameter id merupakan id dari product group capacity</param>
        /// <param name="page">Parameter nomor halaman</param>
        /// <returns>Jika data berhasil di hapus, maka web akan menavigasikan ke halaman detail product group</returns>
        public ActionResult Delete(int id, int? page)
        {
            var currentPage = page.ToString();
            try
            {
                var productGroupCapacityData = db.ProductGroupCapacities.Find(id);
                var productGroupId = productGroupCapacityData.ProductGroupID;
                db.ProductGroupCapacities.Remove(productGroupCapacityData);
                db.SaveChanges();

                //return new RedirectResult(Url.Action("Details", "GroupProduct", new { id = productGroupId, tab = "product-group-capacity", page = currentPage }));
                return RedirectToAction("Details", "GroupProduct", new { id = productGroupId, tab = "product-group-capacity", page = currentPage });
            }
            catch (Exception ex)
            {
                ViewBag.Exception = ex;
            }
            return View();
        }

    }
}
