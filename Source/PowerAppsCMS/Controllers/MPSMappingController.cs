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
    /// Merupakan controller yang digunakan untuk melakukan CRUP Mappin Memo PRO
    /// </summary>
    [CustomAuthorize(Roles = RoleNames.PPC + "," + RoleNames.SuperAdmin)]
    public class MPSMappingController : Controller
    {
        private PowerAppsCMSEntities db = new PowerAppsCMSEntities();
        
        /// <summary>
        /// Fungsi Index berfungsi untuk mengambil dan menampilkan daftar PRO yang ada beserta hasil mappingnta terhadap data SO
        /// </summary>
        /// <param name="searchSONumber">sebuah string yang berarti nomor dari SO yang dicari</param>
        /// <param name="searchPartNumber">sebuah string yang berarti nomor dari partnumber yang dicari</param>
        /// <param name="currentFilter">sebuah string yang berarti nama dari product yang sedang di cari memonya</param>
        /// <param name="page">sebuah bilangan integer yang boleh kosong dan berfungsi sebagai angka parameter nomor halaman</param>
        /// <returns>mamanggil halaman index dari MPS Mapping yang berisikan daftar PRO yang sesuai dengan filter parameter</returns>
        public ActionResult Index(string searchSONumber, string searchPartNumber, string currentFilter, int? page)
        {
            Session.Remove("message");
            if (searchSONumber != null || searchPartNumber != null)
            {
                page = 1;
            }
            else
            {
                searchSONumber = currentFilter;
                searchPartNumber = currentFilter;
            }

            ViewBag.CurrentFilter = searchSONumber;
            ViewBag.CurrentFilter = searchPartNumber;

            var salesOrderList = from x in db.SalesOrders
                                 select x;
            if (!String.IsNullOrEmpty(searchSONumber) && !String.IsNullOrEmpty(searchPartNumber))
            {
                salesOrderList = salesOrderList.Where(x => x.Number.Contains(searchSONumber) && x.PartNumber.Contains(searchPartNumber));
            }
            else if (!String.IsNullOrEmpty(searchSONumber))
            {
                salesOrderList = salesOrderList.Where(x => x.Number.Contains(searchSONumber));
            }
            else if (!String.IsNullOrEmpty(searchPartNumber))
            {
                salesOrderList = salesOrderList.Where(x => x.PartNumber.Contains(searchPartNumber));
            }

            int pageSize = 10;
            int pageNumber = (page ?? 1);

            ViewBag.PageNumber = pageNumber.ToString();
            ViewBag.ItemperPage = pageSize.ToString();
            ViewBag.Page = page.ToString();

            return View(salesOrderList.OrderByDescending(x => x.Created).ToPagedList(pageNumber, pageSize));
        }

        /// <summary>
        /// Berfungsi untuk menambahkan PRO ke dalam sales order
        /// </summary>
        /// <param name="SalesOrderID">Sebuah integer yang merupakan id dari sales order</param>
        /// <param name="mPSMappingViewModel">sebuah object dari view model MPSMappingViewModel</param>
        /// <param name="collection">Object dari FormCollection yang isinya data dari input form halaman view</param>
        /// <returns></returns>
        public ActionResult AddPRO(int SalesOrderID, MPSMappingViewModel mPSMappingViewModel, FormCollection collection)
        {
            var username = User.Identity.Name;
            var currentPage = collection.GetValues("currentPage");
            try
            {
                foreach (var proID in mPSMappingViewModel.SelectedPROID)
                {
                    var newSalesOrderPRO = new PROSalesOrder
                    {
                        PROID = proID,
                        SalesOrderID = SalesOrderID,
                        Created = DateTime.Now,
                        CreatedBy = username,
                        LastModified = DateTime.Now,
                        LastModifiedBy = username
                    };
                    db.PROSalesOrders.Add(newSalesOrderPRO);
                }
                db.SaveChanges();
                Session["message"] = "success";
                return RedirectToAction("Details", "MPSMapping", new { id = SalesOrderID, page = currentPage[0] });
            }
            catch (Exception ex)
            {
                ViewBag.Exception = ex;
            }
            return View("Error");
        }


        /// <summary>
        /// Merupakan sebuah fungsi yang menampilkan halaman detail MPSMapping
        /// </summary>
        /// <param name="id">sebuah bilangan integer yang berarti id dari SalesOrder</param>
        /// <param name="page">sebuah bilangan integer yang boleh kosong dan berfungsi sebagai angka parameter nomor halaman</param>
        /// <param name="mPSMappingViewModel"></param>
        /// <returns></returns>
        public ActionResult Details(int? id, int? page, MPSMappingViewModel mPSMappingViewModel)
        {
            if (id.HasValue)
            {
                try
                {
                    var salesOrderData = db.SalesOrders.Find(id);
                    if (salesOrderData != null)
                    {
                        var productData = db.Products.Where(x => x.PN == salesOrderData.PartNumber).FirstOrDefault();
                        var listOfPROSalesOrder = db.PROSalesOrders.Where(x => x.SalesOrderID == salesOrderData.ID).ToList();

                        if (productData != null)
                        {
                            var listOfPRO = (from x in db.Pros
                                             where x.ProductID == productData.ID && !db.PROSalesOrders.Where(y => y.SalesOrderID == id).Select(y => y.PROID).Contains(x.ID)
                                             select x).ToList();
                            var havePRO = new MPSMappingViewModel
                            {
                                SalesOrder = salesOrderData,
                                ProCollections = listOfPRO,
                                ListOfPROSalesOrder = listOfPROSalesOrder,
                                Product = productData
                            };

                            /*var currentPage = page.ToString()*/;
                            ViewBag.Page = page.ToString();
                            return View(havePRO);
                        }
                        else if (productData == null)
                        {
                            var noPRO = new MPSMappingViewModel
                            {
                                SalesOrder = salesOrderData,
                                ListOfPROSalesOrder = listOfPROSalesOrder,
                                Product = productData
                            };

                            //var currentPage = page.ToString();
                            ViewBag.Page = page.ToString();
                            Session.Remove("message");
                            return View(noPRO);
                        }
                    }
                    else
                    {                        
                        ViewBag.ErrorMessage = "Sorry we couldn't find your sales order";
                        return View("Error");
                    }
                }
                catch (Exception ex)
                {
                    ViewBag.Exception = ex;
                }
                return View("Error");
            }
            else
            {
                return RedirectToAction("Index", "MPSMapping");
            }

        }

        /// <summary>
        /// Merupakan sebuah fungsi digunakan untuk menghapus data SalesOrderPRO
        /// </summary>
        /// <param name="id">sebuah bilangan integer yang berarti id dari data SalesOrderPRO</param>
        /// <returns>Jika berhasil akan mengembalikan halaman detail dari MPSMapping, jika tidak mengembalikan halaman error</returns>
        public ActionResult DeleteSalesOrderPRO(int id)
        {
            var proSalesOrderData = db.PROSalesOrders.Find(id);
            var salesOrderID = proSalesOrderData.SalesOrderID;
            try
            {
                db.PROSalesOrders.Remove(proSalesOrderData);
                db.SaveChanges();
                Session["message"] = "success";
                return RedirectToAction("Details", "MPSMapping", new { id = salesOrderID });
            }
            catch (Exception ex)
            {
                ViewBag.Exception = ex;
            }
            return View("Error");
        }
    }
}