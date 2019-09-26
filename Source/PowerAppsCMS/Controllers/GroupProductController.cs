using System.Linq;
using System.Web.Mvc;
using PowerAppsCMS.Models;
using PagedList;
using PowerAppsCMS.ViewModel;
using System;
using PowerAppsCMS.CustomAuthentication;
using PowerAppsCMS.Constants;

namespace PowerAppsCMS.Controllers
{
    /// <summary>
    /// GroupProductController berfungsi untuk CRUD product group
    /// </summary>
    [CustomAuthorize(Roles = RoleNames.PE + "," + RoleNames.SuperAdmin)]
    public class GroupProductController : Controller
    {
        private PowerAppsCMSEntities db = new PowerAppsCMSEntities();

        /// <summary>
        /// Method Index berfungsi untuk menampilkan list semua product group
        /// </summary>
        /// <param name="searchName">Parameter searchName digunakan untuk mencari product group berdasarkan nama yang di input</param>
        /// <param name="currentFilter">Parameter yang digunakan untuk mengatur filter ketika user membuka halaman saat ini atau berikutnya</param>
        /// <param name="page">Parameter nomor halaman</param>
        /// <returns>Menampilkan list semua product group</returns>
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

            var groupProductList = from x in db.ProductGroups
                                   select x;

            if (!String.IsNullOrEmpty(searchName))
            {
                groupProductList = groupProductList.Where(x => x.Name.Contains(searchName));
            }

            int pageSize = 10;
            int pageNumber = (page ?? 1);

            ViewBag.PageNumber = pageNumber.ToString();
            ViewBag.ItemperPage = pageSize.ToString();
            ViewBag.Page = page.ToString();

            return View(groupProductList.OrderBy(x => x.Name).ToPagedList(pageNumber, pageSize));
        }

        /// <summary>
        /// Method Details berfungsi untuk menampilkan detail dari sebuah product group
        /// </summary>
        /// <param name="id">Parameter id merupakan id dari product group</param>
        /// <param name="page">Parameter nomor halaman</param>
        /// <param name="tab">Parameter tab digunakan sebagai referensi tab mana yang akan di tampilkan di halaman detail</param>
        /// <returns>Menampilkan detail dari sebuah product group</returns>
        public ActionResult Details(int? id,int? page, string tab="")
        {
            if (id.HasValue)
            {
                var groupProductData = db.ProductGroups.Find(id);
                if (groupProductData != null)
                {
                    var productSubGroupList = db.ProductSubGroups.Where(x => x.ProductGroupID == id).ToList();
                    var productGroupCapacityList = db.ProductGroupCapacities.Where(x => x.ProductGroupID == id).OrderByDescending(x => x.StartMonth).ToList();

                    var viewModel = new GroupProductViewModel
                    {
                        ProductGroup = groupProductData,
                        ProductSubGroups = productSubGroupList,
                        ProductGroupCapacities = productGroupCapacityList
                    };

                    ViewBag.ActiveTab = tab;
                    ViewBag.Page = page.ToString();
                    return View(viewModel);
                }
                else
                {
                    ViewBag.ErrorMessage = "Sorry we could not find the product group";
                    return View("Error");
                }
            }
            else
            {
                return RedirectToAction("Index", "GroupProduct");
            }
            
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
        /// Method Post Create berfungsi untuk menambah data baru di product group
        /// </summary>
        /// <param name="productGroup">Parameter model dari product group</param>
        /// <returns>Ketika data baru berhasil di tambahkan, maka web akan menavigasikan ke halaman detail</returns>
        [HttpPost]
        public ActionResult Create(ProductGroup productGroup)
        {
            var username = User.Identity.Name;
            try
            {
                var existGroupProduct = db.ProductGroups.Where(x => x.Name == productGroup.Name).SingleOrDefault();

                if (existGroupProduct != null)
                {
                    ViewBag.ExistProductGroup = "Product group already exist";
                    return View();
                }
                else
                {
                    productGroup.Created = DateTime.Now;
                    productGroup.CreatedBy = username;
                    productGroup.LastModified = DateTime.Now;
                    productGroup.LastModifiedBy = username;
                    db.ProductGroups.Add(productGroup);
                    db.SaveChanges();

                    ViewBag.Message = "Success";
                    ViewBag.ProductGroupID = productGroup.ID.ToString();
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
        /// <param name="id">Parameter id merupakan id dari product group</param>
        /// <param name="page">Parameter nomor halaman</param>
        /// <returns>Menampilkan halaman edit</returns>
        public ActionResult Edit(int? id, int? page)
        {
            if (id.HasValue)
            {
                var productGroupData = db.ProductGroups.Find(id);
                if (productGroupData != null)
                {
                    ViewBag.Page = page.ToString();
                    return View(productGroupData);
                }
                else
                {
                    ViewBag.ErrorMessage = "Sorry we couldn't find this product group";
                    return View("Error");
                }
            }
            else
            {
                return RedirectToAction("Index", "GroupProduct");
            }
            
        }

        /// <summary>
        /// Method Post Edit berfungsi untuk mengubah data dari sebuah product group
        /// </summary>
        /// <param name="id">Parameter id merupakan id dari product group</param>
        /// <param name="productGroup">Parameter model dari product group</param>
        /// <returns>Jika data berhasil dirubah, maka web akan menavigasikan ke halaman detail</returns>
        [HttpPost]
        public ActionResult Edit(int id, ProductGroup productGroup)
        {
            var username = User.Identity.Name;
            var productGroupData = db.ProductGroups.Find(id);
            try
            {
                var existProductGroup = db.ProductGroups.Where(x => x.Name == productGroup.Name && x.ID != productGroup.ID).SingleOrDefault();

                // Edit Product Group
                if (existProductGroup != null)
                {
                    ViewBag.ExistGroupProduct = "Group product already exist";
                    return View(productGroupData);
                }
                else
                {
                    productGroupData.Name = productGroup.Name;
                    productGroupData.Description = productGroup.Description;
                    productGroupData.LastModified = DateTime.Now;
                    productGroupData.LastModifiedBy = username;
                    db.SaveChanges();

                    ViewBag.Message = "Success";
                    ViewBag.ProductGroupID = productGroupData.ID.ToString();
                    return View(productGroupData);
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
        /// Method Delete berfungsi untuk menghapus sebuah data dari product group
        /// </summary>
        /// <param name="id">Parameter id merupakan id dari product group</param>
        /// <returns>Jika data berhasil di hapus, maka web akan menavigasikan ke halaman index</returns>
        public ActionResult Delete(int id)
        {
            ProductGroup productGroup = db.ProductGroups.Find(id);
            try
            {
                if (productGroup.ProductSubGroups.Count() > 0)
                {
                    ViewBag.ErrorMessage = productGroup.Name + " " + "could not be deleted because there are some product sub group on this product group";
                    return View("Error");
                }
                else if (productGroup.ProductGroupCapacities.Count > 0)
                {
                    ViewBag.ErrorMessage = productGroup.Name + " " + "could not be deleted because this product is set on the capacity setting";
                    return View("Error");
                }
                db.ProductGroups.Remove(productGroup);
                db.SaveChanges();

                return RedirectToAction("Index", "GroupProduct");
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
