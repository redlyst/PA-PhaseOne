using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using PagedList;
using PowerAppsCMS.Constants;
using PowerAppsCMS.CustomAuthentication;
using PowerAppsCMS.Models;
using PowerAppsCMS.ViewModel;

namespace PowerAppsCMS.Controllers
{
    /// <summary>
    /// ProductController berfungsi sebagai CRUD product
    /// </summary>
    /// <remarks>
    /// Di dalam ProductController terdapat beberapa method lain selain CRUD, seperti
    /// CalculateTotalDay, dan juga GetProcessDay
    /// </remarks>
    [CustomAuthorize(Roles = RoleNames.PE + "," + RoleNames.SuperAdmin)]
    public class ProductController : Controller
    {
        private PowerAppsCMSEntities db = new PowerAppsCMSEntities();

        /// <summary>
        /// Merupakan sebuah class berisikan sebuah bilangan integer dan bilangan decimal
        /// </summary>
        private class ProcessDay
        {
            /// <summary>
            /// sebuah bilangan integer yang menandakan angka hari kapan sebuah process itu dimulai
            /// </summary>
            public int StartDay { get; set; }

            /// <summary>
            /// sebuah bilangan decimal yang menandakan sebarapa banyak hari tersebut di gunakan
            /// </summary>
            /// <remarks>
            /// bernilai 0-1, dimana 1 menandakan bahwa hari yang digunakan satu hari penuh
            /// </remarks>
            public decimal UsedDay { get; set; }
        }

        /// <summary>
        /// Method Index berfungsi untuk menampilkan list semua product
        /// </summary>
        /// <param name="searchName">Parameter searchName digunakan untuk mencari product berdasarkan nama yang di input</param>
        /// <param name="currentFilter">Parameter yang digunakan untuk mengatur filter ketika user membuka halaman saat ini atau berikutnya</param>
        /// <param name="page">Parameter nomor halaman</param>
        /// <returns>List semua product</returns>
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

            var productList = from x in db.Products
                              select x;
            if (!String.IsNullOrEmpty(searchName))
            {
                productList = productList.Where(x => x.Name.Contains(searchName));
            }

            int pageSize = 10;
            int pageNumber = (page ?? 1);

            ViewBag.PageNumber = pageNumber.ToString();
            ViewBag.ItemperPage = pageSize.ToString();
            ViewBag.Page = page.ToString();
            return View(productList.OrderBy(x => x.Name).ToPagedList(pageNumber, pageSize));
        }

        /// <summary>
        /// Method Details berfungsi untuk menampilkan detail dari sebuah product
        /// </summary>
        /// <param name="id">Parameter id merupakan id dari product</param>
        /// <param name="page">Parameter nomor halaman</param>
        /// <param name="tab">Parameter tab digunakan sebagai referensi tab mana yang akan di tampilkan di halaman detail</param>
        /// <returns>Menampilkan detail dari sebuah product</returns>
        public ActionResult Details(int? id, int? page, string tab = "")
        {
            if (id.HasValue)
            {
                Products productData = db.Products.Find(id);
                if (productData != null)
                {
                    List<MasterProcess> masterProcessData = db.MasterProcesses.Where(x => x.ProductID == id).OrderBy(x => x.ProcessOrder).ToList();
                    List<Component> componentList = (from x in db.Components
                                         where x.ComponentMaterialPreparationProcesses.Count > 0 && !db.ProductCompositions.Where(y => y.ProductID == productData.ID).Select(y => y.ComponentID).Contains(x.ID)
                                         select x).ToList();
                    List<ProductComposition> productCompositionList = db.ProductCompositions.Where(x => x.ProductID == id).ToList();
                    List<ProcessDependency> processDependencyData = db.ProcessDependencies.ToList();
                    List<ComponentMaterialPreparationProcess> componentMaterialPreparationProcess = db.ComponentMaterialPreparationProcesses.ToList();

                    var viewModel = new ProductViewModel
                    {
                        Product = productData,
                        MasterProcesses = masterProcessData,
                        ProductCompositions = productCompositionList,
                        ComponentCollections = componentList,
                        ProcessDependencies = processDependencyData,
                        ComponentMaterialPreparationProcesses = componentMaterialPreparationProcess
                    };
                    ViewBag.ActiveTab = tab;
                    ViewBag.Page = page.ToString();
                    return View(viewModel);
                }
                else
                {
                    ViewBag.ErrorMessage = "Sorry we couldn't find this product";
                    return View("Error");
                }
            }
            else
            {
                return RedirectToAction("Index", "Product");
            }

        }

        /// <summary>
        /// Merupakan fungsi untuk menghitung Total Day lamanya sebuah produk dikerjakan
        /// </summary>
        /// <param name="productID">sebuah bilangan integer yang berarti id dari product yang akan dihitung total day prosesnya</param>
        /// <returns>nilai dari total day dan ditampilkan halaman details product</returns>
        [HttpPost]
        public ActionResult CalculateTotalDay(int productID)
        {
            Products productData = db.Products.Find(productID);
            var username = User.Identity.Name;
            try
            {
                foreach (MasterProcess item in productData.MasterProcess)
                {
                    if (item.ProcessDailySchedules.Count() > 0)
                    {
                        db.ProcessDailySchedules.RemoveRange(item.ProcessDailySchedules);
                        db.SaveChanges();
                    }
                }

                ProcessDay processDayData = new ProcessDay() { StartDay = 0, UsedDay = 0 };

                List<ProcessDailySchedule> currentProductProcessDailyScheduleList = new List<ProcessDailySchedule>();

                List<MasterProcess> orderedMasterProcessesList = productData.MasterProcess.OrderBy(o => o.ID).ThenBy(o => o.ProcessDependencies.Count()).ToList();
                foreach (MasterProcess item in orderedMasterProcessesList)
                {
                    if (item.ProcessDependencies.Count() == 0)//set process as first processes
                    {
                        processDayData.StartDay = 1;
                        processDayData.UsedDay = 0;
                    }
                    else if (item.ProcessDependencies.Count() == 1)//Set Start Day for Process that have dependency only 1
                    {
                        ProcessDependency processDependency = new ProcessDependency();
                        processDependency = item.ProcessDependencies.FirstOrDefault();

                        processDayData = GetProcessDay(processDependency, currentProductProcessDailyScheduleList);
                    }
                    else//Set Start Day for Process that have dependency more than 1
                    {
                        ProcessDay maxprocessDay = new ProcessDay() { StartDay = 0, UsedDay = 0 };
                        processDayData.StartDay = 0;
                        processDayData.UsedDay = 0;
                        foreach (ProcessDependency pdItem in item.ProcessDependencies)
                        {
                            maxprocessDay = GetProcessDay(pdItem, currentProductProcessDailyScheduleList);
                            if (processDayData.StartDay < maxprocessDay.StartDay || (processDayData.StartDay == maxprocessDay.StartDay && processDayData.UsedDay < maxprocessDay.UsedDay))
                            {
                                processDayData.StartDay = maxprocessDay.StartDay;
                                processDayData.UsedDay = maxprocessDay.UsedDay;
                            }
                        }
                    }

                    decimal currentProcessCycleTime = item.CycleTime;
                    int startDay = processDayData.StartDay;
                    decimal usedDay = 0;
                    decimal currentProcessUsedDay = 1 - processDayData.UsedDay;//find the capacity of currentDay for currentProcess

                    if (currentProcessUsedDay <= currentProcessCycleTime)//if currentCapacity smaller then processCycletime, so the capacity of the da is fully used
                    {
                        usedDay = 1;
                    }
                    else //if no, current usedDay is sum of processCycleTime and usedDay
                    {
                        usedDay = processDayData.UsedDay + currentProcessCycleTime;
                        currentProcessUsedDay = usedDay;
                    }

                    do
                    {

                        DateTime now = DateTime.Now;
                        currentProductProcessDailyScheduleList.Add(new ProcessDailySchedule()
                        {
                            MasterProcessID = item.ID,
                            Day = startDay,
                            UsedDay = usedDay,
                            Created = now,
                            LastModified = now,
                            LastModifiedBy = username,
                            CreatedBy = username
                        });

                        currentProcessCycleTime -= currentProcessUsedDay;
                        startDay++;

                        if (currentProcessCycleTime > 1)
                        {
                            currentProcessUsedDay = usedDay = 1;
                        }
                        else
                        {
                            currentProcessUsedDay = usedDay = currentProcessCycleTime;
                        }

                    } while (currentProcessCycleTime > 0);
                }

                int totalDay = currentProductProcessDailyScheduleList.Max(x => x.Day);
                //ProcessDailySchedule lastProcessDailySchedule = currentProductProcessDailyScheduleList.Where(x => x.Day == totalDay).OrderByDescending(x => x.UsedDay).FirstOrDefault();

                //if()
                db.ProcessDailySchedules.AddRange(currentProductProcessDailyScheduleList);
                if (db.SaveChanges() > 0)
                {
                    productData.TotalDay = totalDay;
                    productData.IsTotalDayCalculated = true;
                    productData.LastModifiedBy = username;
                    productData.LastModified = DateTime.Now;
                    db.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                ViewBag.Exception = ex;
            }
            return RedirectToAction("Details", "Product", new { id = productID });
        }

        /// <summary>
        /// Merupakan fungsi yang menghitung nilai hari yang berlaku pada sebuah process
        /// hari yang berlaku untuk sebuah process dilihat dari nilai hari maksimun yang dimiliki olah proses yang menjadi dependency proses tersebut
        /// </summary>
        /// <param name="processDependency">sebuah class processDependency yang berarti dependency yang dimiliki ssebuah proses</param>
        /// <param name="currentProductProcessDailyScheduleList">sebuah list dari processDailySchedule yang dimiliki oleh process yang akad dihitung nilai harinya</param>
        /// <returns>sebuah class processday yang berisikan angka hari kapan process dimulai dan seberapa lama hari tersebut digunakan </returns>
        private ProcessDay GetProcessDay(ProcessDependency processDependency, List<ProcessDailySchedule> currentProductProcessDailyScheduleList)
        {
            ProcessDay processDayData = new ProcessDay() { StartDay = 1, UsedDay = 0 };

            int maxDay = currentProductProcessDailyScheduleList.Where(x => x.MasterProcessID == processDependency.PredecessorProcessID).Max(m => m.Day);//processDependency.MasterProcessPredecessor.ProcessDailySchedules.Max(x => x.Day);
            ProcessDailySchedule processDailyScheduleWithMaximumDay = currentProductProcessDailyScheduleList.Where(x => x.Day == maxDay && x.MasterProcessID == processDependency.PredecessorProcessID).FirstOrDefault();// processDependency.MasterProcessPredecessor.ProcessDailySchedules.Where(x => x.Day == maxDay).Sum(x => x.UsedDay);
            processDayData.UsedDay = processDailyScheduleWithMaximumDay.UsedDay;

            if (processDayData.UsedDay < 1)
            {
                processDayData.StartDay = maxDay;
            }
            else
            {
                processDayData.StartDay = maxDay + 1;
                processDayData.UsedDay = 0;
            }
            return processDayData;
        }


        /// <summary>
        /// Method Create berfungsi untuk menampilkan halaman create
        /// </summary>
        /// <returns>Menampilkan halaman create product</returns>
        public ActionResult Create()
        {
            ViewBag.ProductSubGroupID = new SelectList(db.ProductSubGroups.OrderBy(x => x.Name), "ID", "Name");
            return View();
        }

        /// <summary>
        /// Method Post Create digunakan untuk menambahkan data baru di product
        /// </summary>
        /// <param name="product">Parameter model dari product</param>
        /// <returns>Ketika data baru berhasil di input, maka web akan menavigasikan ke halaman details</returns>
        [HttpPost]
        public ActionResult Create(Products product)
        {
            var username = User.Identity.Name;
            var existPartNumber = db.Products.Where(x => x.PN == product.PN).SingleOrDefault();
            ViewBag.ProductSubGroupID = new SelectList(db.ProductSubGroups.OrderBy(x => x.Name), "ID", "Name");
            try
            {

                // TODO: Add Product
                if (existPartNumber != null)
                {
                    ViewBag.PartNumber = "Part number already exist";
                    return View();
                }
                else
                {
                    product.IsTotalDayCalculated = false;
                    product.Created = DateTime.Now;
                    product.CreatedBy = username;
                    product.LastModified = DateTime.Now;
                    product.LastModifiedBy = username;
                    db.Products.Add(product);
                    db.SaveChanges();

                    ViewBag.Message = "Success";
                    ViewBag.ProductID = product.ID;
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
        /// <param name="id">Parameter id yang merupakan id dari product</param>
        /// <param name="page">Parameter nomor halaman</param>
        /// <returns>Menampilkan halaman edit</returns>
        public ActionResult Edit(int? id, int? page)
        {
            if (id.HasValue)
            {
                var productData = db.Products.Find(id);
                if (productData != null)
                {
                    ViewBag.Page = page.ToString();
                    ViewBag.ProductSubGroupID = new SelectList(db.ProductSubGroups.OrderBy(x => x.Name), "ID", "Name", productData.ProductSubGroupID);
                    return View(productData);
                }
                else
                {
                    ViewBag.ErrorMessage = "Sorry we couldn't find this user";
                    return View("Error");
                }
            }
            else
            {
                return RedirectToAction("Index", "Product");
            }
        }

        /// <summary>
        /// Method Post Edit berfungsi untuk mengubah data dari sebuah product
        /// </summary>
        /// <param name="id">Parameter id yang merupakan id dari product</param>
        /// <param name="product">Parameter model dari product</param>
        /// <param name="collection">Parameter dari object FormCollection</param>
        /// <returns>Jika data berhasil dirubah, maka web akan menavigasikan ke halaman details</returns>
        [HttpPost]
        public ActionResult Edit(int id,  Products product, FormCollection collection)
        {
            var username = User.Identity.Name;
            var productData = db.Products.Find(id);
            var existPartNumber = db.Products.Where(x => x.PN == product.PN && x.ID != product.ID).SingleOrDefault();
            var currentPage = collection.GetValues("currentPage");

            ViewBag.ProductSubGroupID = new SelectList(db.ProductSubGroups.OrderBy(x => x.Name), "ID", "Name", productData.ProductSubGroupID);

            try
            {
                if (existPartNumber != null)
                {
                    ViewBag.PartNumber = "Part number already exist";
                    return View(productData);
                }
                else
                {
                    // TODO: Edit product
                    productData.Name = product.Name;
                    productData.ProductSubGroupID = product.ProductSubGroupID;
                    productData.PN = product.PN;
                    db.SaveChanges();

                    ViewBag.Message = "Success";
                    return RedirectToAction("Details", "Product", new { id = productData.ID, page = currentPage[0] });
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
        /// Method Delete berfungsi untuk menghapus sebuah data dari product
        /// </summary>
        /// <param name="id">Parameter id yang merupakan id dari product</param>
        /// <returns>Data berhasil di hapus, dan web akan menavigasikan ke halaman index</returns>
        public ActionResult Delete(int id)
        {
            try
            {
                var productData = db.Products.Find(id);
                var masterProcessData = db.MasterProcesses.Where(x => x.ProductID == id).ToList();

                if (masterProcessData.Count > 0)
                {
                    ViewBag.ErrorMessage = productData.Name + " " + "could not be deleted, because there are some process on this product";
                    return View("Error");
                }
                else if (productData.ProductComposition.Count() > 0)
                {
                    ViewBag.ErrorMessage = productData.Name + " " + "could not be deleted, because there are some components are within in this product";
                }
                else
                {
                    db.Products.Remove(productData);
                    db.SaveChanges();

                    return RedirectToAction("Index", "Product");
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
