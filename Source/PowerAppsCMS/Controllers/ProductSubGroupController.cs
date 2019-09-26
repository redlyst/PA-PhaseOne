using System;
using System.Linq;
using System.Web.Mvc;
using PowerAppsCMS.Models;
using PowerAppsCMS.ViewModel;
using PagedList;
using PowerAppsCMS.CustomAuthentication;
using PowerAppsCMS.Constants;

namespace PowerAppsCMS.Controllers
{
    /// <summary>
    /// ProductSubGroupController berfungsi sebagai CRUD product sub group
    /// </summary>
    [CustomAuthorize(Roles = RoleNames.PE + "," + RoleNames.SuperAdmin)]
    public class ProductSubGroupController : Controller
    {
        private PowerAppsCMSEntities db = new PowerAppsCMSEntities();

        /// <summary>
        /// Method Index berfungsi untuk menampilkan semua list product sub group
        /// </summary>
        /// <param name="searchName">Parameter searchName digunakan untuk mencari product sub group berdasarkan nama yang di input</param>
        /// <param name="currentFilter">Parameter yang digunakan untuk mengatur filter ketika user membuka halaman saat ini atau berikutnya</param>
        /// <param name="page">Parameter nomor halaman</param>
        /// <returns>Menampilkan semua list product sub group</returns>
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

            var productSubGroupList = from x in db.ProductSubGroups
                                      select x;

            if (!String.IsNullOrEmpty(searchName))
            {
                productSubGroupList = productSubGroupList.Where(x => x.Name.Contains(searchName));
            }

            int pageSize = 10;
            int pageNumber = (page ?? 1);

            ViewBag.PageNumber = pageNumber.ToString();
            ViewBag.ItemperPage = pageSize.ToString();
            ViewBag.CurrentPage = page.ToString();

            return View(productSubGroupList.OrderBy(x => x.Name).ToPagedList(pageNumber, pageSize));
        }

        /// <summary>
        /// Method Details berfungsi untuk menampilkan detail dari sebuah product sub group
        /// </summary>
        /// <param name="id">Parameter id yang merupakan id dari product sub group</param>
        /// <param name="page">Parameter nomor halaman</param>
        /// <param name="currentPage">Parameter nomor halaman saat ini</param>
        /// <returns>Menampilkan detail dari sebuah product sub group</returns>
        public ActionResult Details(int? id, int? page, int? currentPage)
        {
            if (id.HasValue)
            {
                var productSubGroupData = db.ProductSubGroups.Find(id);
                if (productSubGroupData != null)
                {
                    var productList = db.Products.Where(x => x.ProductSubGroupID == id).ToList();
                    var pager = new Pager(productList.Count(), page);
                    ViewBag.ProductGroupID = new SelectList(db.ProductGroups, "ID", "Name");

                    var viewModel = new ProductSubGroupViewModel
                    {
                        ProductSubGroup = productSubGroupData,
                        ListOfProducts = productList.Skip((pager.CurrentPage - 1) * pager.PageSize).Take(pager.PageSize),
                        Pager = pager
                    };

                    int pageSize = 10;
                    int pageNumber = (page ?? 1);

                    ViewBag.PageNumber = pageNumber.ToString();
                    ViewBag.ItemperPage = pageSize.ToString();
                    ViewBag.CurrentPage = currentPage.ToString();

                    return View(viewModel);
                }
                else
                {
                    ViewBag.ErrorMessage = "Sorry we could not find this product sub group";
                    return View("Error");
                }
            }
            else
            {   
                return RedirectToAction("Index", "ProductSubGroup");
            }
        }

        /// <summary>
        /// Method Create berfungsi untuk menampilkan halaman create
        /// </summary>
        /// <param name="productGroupId">Parameter productGroupId merupakan id dari product group</param>
        /// <param name="groupProductPage">Parameter groupProductPage merupakan halaman saat ini yang dibuka ketika user membuka halaman group product</param>
        /// <returns>Menampilkan halaman create</returns>
        public ActionResult Create(int productGroupId, int? groupProductPage)
        {
            ViewBag.ProductGroupID = productGroupId;
            ViewBag.Page = groupProductPage.ToString();
            return View();
        }

        /// <summary>
        /// Method Post Create berfungsi untuk menambah data baru di product sub group
        /// </summary>
        /// <param name="productGroupId">Parameter productGroupId merupakan id dari product group</param>
        /// <param name="productSubGroup">Parameter model dari product sub group</param>
        /// <param name="collection">Parameter yang digunakan oleh object FormCollection</param>
        /// <returns>Jika data baru berhasil di input, maka web akan menavigasikan ke halaman detail product group</returns>
        [HttpPost]
        public ActionResult Create(int productGroupId, ProductSubGroup productSubGroup, FormCollection collection)
        {
            var username = User.Identity.Name;
            var existProductSubGroupName = db.ProductSubGroups.Where(x => x.Name == productSubGroup.Name).SingleOrDefault();
            var existSAPCode = db.ProductSubGroups.Where(x => x.SAPCode == productSubGroup.SAPCode).SingleOrDefault();
            var groupProductCurrentPage = collection.GetValues("currentPage");
            ViewBag.ProductGroupID = productGroupId;
            ViewBag.GroupProductCurrentPage = groupProductCurrentPage[0];

            try
            {
                // TODO: Add Product Sub Group
                if (existProductSubGroupName != null)
                {
                    ViewBag.ExistProductSubGroupName = "Product sub group already exist";
                    return View();
                }
                else if (existSAPCode != null)
                {
                    ViewBag.ExistSAPCode = "SAP Code already exist";
                    return View();
                }
                else
                {
                    productSubGroup.ProductGroupID = productGroupId;
                    productSubGroup.Created = DateTime.Now;
                    productSubGroup.CreatedBy = username;
                    productSubGroup.LastModified = DateTime.Now;
                    productSubGroup.LastModifiedBy = username;
                    db.ProductSubGroups.Add(productSubGroup);
                    db.SaveChanges();

                    ViewBag.Message = "success";
                    return View("Create");
                }
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "An error occured, please check your data input and try again";
                ViewBag.Exception = ex;
            }

            return View("Error");
        }

        /// <summary>
        /// Method Edit befungsi untuk menampilkan halaman edit
        /// </summary>
        /// <param name="id">Parameter id merupakan id dari product sub group</param>
        /// <param name="currentPage">Parameter currentPage merupakan halaman saat ini yang sedang di akses user</param>
        /// <returns>Menampilkan halaman edit</returns>
        public ActionResult Edit(int id, int? currentPage)
        {
            var productSubGroupData = db.ProductSubGroups.Find(id);
            if (productSubGroupData != null)
            {
                ViewBag.CurrentPage = currentPage;
                return View(productSubGroupData);
            }
           else
            {
                ViewBag.ErrorMessage = "Sorry we couldn't find this product sub group";
                return View("Error");
            }
        }

        /// <summary>
        /// Method Post Edit berfungsi untuk mengubah data dari suatu product sub group
        /// </summary>
        /// <param name="id">Parameter id merupakan id dari product sub group</param>
        /// <param name="productSubGroup">Parameter dari model ProductSubGroup</param>
        /// <returns>Jika data berhasil dirubah, maka web akan menavigasikan ke halaman detail product group</returns>
        [HttpPost]
        public ActionResult Edit(int id, ProductSubGroup productSubGroup)
        {
            var username = User.Identity.Name;
            var productSubGroupData = db.ProductSubGroups.Find(id);
            var existProductSubGroupName = db.ProductSubGroups.Where(x => x.ID != productSubGroup.ID && x.Name == productSubGroup.Name).SingleOrDefault();
            var existSAPCode = db.ProductSubGroups.Where(x => x.ID != productSubGroup.ID && x.SAPCode == productSubGroup.SAPCode).SingleOrDefault();
            ViewBag.ProductGroupID = Convert.ToString(productSubGroupData.ProductGroupID);

            try
            {
                // TODO: Update Product Sub Group
                if (existProductSubGroupName != null)
                {
                    ViewBag.ExistProductSubGroupName = "Product sub group already exist";
                    return View(productSubGroupData);
                }
                else if (existSAPCode != null)
                {
                    ViewBag.ExistSAPCode = "SAP Code already exist";
                    return View(productSubGroupData);
                }
                else
                {
                    productSubGroupData.Name = productSubGroup.Name;
                    productSubGroupData.SAPCode = productSubGroup.SAPCode;
                    productSubGroupData.LastModified = DateTime.Now;
                    productSubGroupData.LastModifiedBy = username;
                    db.SaveChanges();

                    ViewBag.Message = "success";
                    return View(productSubGroupData);
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
        /// Method Delete berfungsi untuk menghapus sebuah data product sub group
        /// </summary>
        /// <param name="id">Parameter id merupakan id dari product sub group</param>
        /// <param name="page">Parameter nomor halaman</param>
        /// <returns>Jika berhasil di delete, maka web akan menavigasikan ke halaman index</returns>
        public ActionResult Delete(int id, int? page)
        {
            var productSubGroupData = db.ProductSubGroups.Find(id);
            var productGroupId = productSubGroupData.ProductGroupID;
            var currentPage = page.ToString();
            try
            {
                if (productSubGroupData.Products.Count() > 0)
                {
                    ViewBag.ErrorMessage = productSubGroupData.Name + " " + "can't be deleted because there are some products on this sub group";
                    return View("Error");
                }
                else
                {
                    db.ProductSubGroups.Remove(productSubGroupData);
                    
                    db.SaveChanges();
                }
                return RedirectToAction("Details", "GroupProduct", new { id = productGroupId, page = currentPage });
            }
            catch (Exception ex)
            {
                ViewBag.Exception = ex;
                ViewBag.ErrorMessage = "An error occured, please check your data input and try again";
            }
            return View("Error");
        }
    }
}
