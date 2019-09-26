using System;
using System.Linq;
using System.Web.Mvc;
using PowerAppsCMS.Constants;
using PowerAppsCMS.CustomAuthentication;
using PowerAppsCMS.Models;

namespace PowerAppsCMS.Controllers
{
    /// <summary>
    /// CapacitySettingController berfungsi untuk mengubah kapasitas per bulan dan tanggal mulai
    /// </summary>
    [CustomAuthorize(Roles = RoleNames.PE + "," + RoleNames.SuperAdmin)]
    public class CapacitySettingController : Controller
    {
        private PowerAppsCMSEntities db = new PowerAppsCMSEntities();

        /// <summary>
        /// Method Index berfungsi untuk menampilkan semua list capacity design
        /// </summary>
        /// <returns>Menampilkan semua list capacity design</returns>
        public ActionResult Index()
        {
            var capacityDesignList = from x in db.ProductGroupCapacities
                           group x by x.ProductGroupID into g
                           select g.OrderByDescending(x => x.StartMonth).FirstOrDefault();
           
            return View(capacityDesignList.OrderBy(x => x.ProductGroup.Name));
        }

        /// <summary>
        /// Method Edit berfungsi untuk mengubah capacity permonth atau start month
        /// </summary>
        /// <param name="productGroupCapacity">Parameter productGroupCapacity merupakan parameter model dari product group capacity</param>
        /// <returns>Data berhasil dirubah</returns>
        public ActionResult Edit(ProductGroupCapacity productGroupCapacity)
        {
            try
            {
                var username = User.Identity.Name;
                var productGroupCapactityData = db.ProductGroupCapacities.Find(productGroupCapacity.ID);
                var startMonth = Convert.ToDateTime(productGroupCapacity.StartMonth);
                var curentDate = DateTime.Now;

                var existingCapacityDesign = db.ProductGroupCapacities.Where(x => x.ID == productGroupCapactityData.ID && (x.StartMonth.Month == startMonth.Month && x.StartMonth.Year == startMonth.Year)).SingleOrDefault();

                if(startMonth.Month < curentDate.Month && startMonth.Year == curentDate.Year)
                {
                    return Json(new { success = false, responseText = "Capacity can't be updated because start month less than current month" }, JsonRequestBehavior.AllowGet);
                }
                else if (startMonth.Month == curentDate.Month && startMonth.Year == curentDate.Year)
                {
                    return Json(new { success = false, responseText = "Capacity can't be updated because start month same with current month" }, JsonRequestBehavior.AllowGet);
                }
                else if (existingCapacityDesign != null)
                {
                    
                    productGroupCapactityData.Capacity = productGroupCapacity.Capacity;
                    productGroupCapactityData.StartMonth = new DateTime(productGroupCapacity.StartMonth.Year, productGroupCapacity.StartMonth.Month, 1);
                    db.SaveChanges();

                    return Json(new { success = true, responseText = "Capacity setting successfully updated" }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    var newCapacityDesign = new ProductGroupCapacity
                    {
                        ProductGroupID = productGroupCapactityData.ProductGroupID,
                        Capacity = productGroupCapacity.Capacity,
                        StartMonth = new DateTime(productGroupCapacity.StartMonth.Year, productGroupCapacity.StartMonth.Month, 1),
                        Created = DateTime.Now,
                        CreatedBy = username,
                        LastModified = DateTime.Now,
                        LastModifiedBy = username
                    };
                    db.ProductGroupCapacities.Add(newCapacityDesign);
                    db.SaveChanges();

                    return Json(new { success = true, responseText = "Capacity setting successfully added" }, JsonRequestBehavior.AllowGet);
                }
            }
            catch
            {

            }
            return View("Error");
        }

        /// <summary>
        /// Method Delete berfungsi untuk menghapus data capacity design
        /// </summary>
        /// <param name="id">parameter id merupakan id dari product group capacity</param>
        /// <returns>Data berhasil dihapus</returns>
        public ActionResult Delete(int id)
        {
            var capacitySettingData = db.ProductGroupCapacities.Find(id);
            db.ProductGroupCapacities.Remove(capacitySettingData);
            db.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}