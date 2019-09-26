using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PowerAppsCMS.Models;
using PowerAppsCMS.ViewModel;
using PagedList;
using PowerAppsCMS.CustomAuthentication;
using PowerAppsCMS.Constants;

namespace PowerAppsCMS.Controllers
{
    /// <summary>
    /// MasterProcessController berisikan fungsi-fungsi untuk proses CRUD data Master Process
    /// </summary>
    /// <remarks>
    /// Fungsi-fungsi yang ada pada controller ini adalah index, Details, Create, Edit, Delete, UpdateMasterProcessOrder dan GetMasterProcessList
    /// </remarks>
    [CustomAuthorize(Roles = RoleNames.PE + "," + RoleNames.SuperAdmin)]
    public class MasterProcessController : Controller
    {
        private PowerAppsCMSEntities db = new PowerAppsCMSEntities();

        // GET: MasterProcess
        /// <summary>
        /// Fungsi Index berfungsi untuk mengambil dan menampilkan daftar data MasterProcess yang ada
        /// </summary>
        /// <returns>Mengembalikan View Index untuk menampilkan daftar data Master Process</returns>
        public ActionResult Index()
        {
            return View();
        }

        // GET: MasterProcess/Details/5
        /// <summary>
        /// Details berfungsi menampilkan detail data dari sebuah master process yang dipilih berdasarkan id
        /// </summary>
        /// <param name="id">Merupakan sebuah bilangan integer</param>
        /// <returns>Mengembalikan sebuah action result yang menampilkan halaman view detail data sebuah master process berdasarkan id yang dipilih</returns>
        public ActionResult Details(int id)
        {
            var masterProcessData = db.MasterProcesses.Find(id);
            ViewBag.ProductID = Convert.ToString(masterProcessData.ProductID);
            return View(masterProcessData);
        }

        // GET: MasterProcess/Create
        /// <summary>
        /// Fungsi Create ini merupakan fungsi yang digunakan untuk menampilkan form untuk menambahkan data Master Process yang baru untuk sebuah product
        /// </summary>
        /// <param name="productID">Merupakan sebuah bilangan integer</param>
        /// <returns>Mengembalikan sebuah action result yang memanggil halaman create</returns>
        public ActionResult Create(int productID)
        {
            ViewBag.ProductID = Convert.ToString(productID);
            ViewBag.ProcessGroupID = new SelectList(db.ProcessGroups, "ID", "Name");
            var masterProcessList = GetMasterProcessList(productID);
            var viewModel = new MasterProcessViewModel
            {
                MasterProcessCollections = masterProcessList
            };

            ViewBag.currentProcessOrder = 1;

            if (masterProcessList.LastOrDefault() != null)
            {
                ViewBag.currentProcessOrder = Convert.ToInt32(masterProcessList.LastOrDefault().ProcessOrder) + 1;
            }

            return View(viewModel);
        }

        // POST: MasterProcess/Create
        /// <summary>
        /// Fungsi Create ini merupakan fungsi yang digunakan untuk menambahkan data Master Process yang baru untuk sebuah product
        /// </summary>
        /// <param name="productID">merupakan sebuah bilangan integer</param>
        /// <param name="masterProcess">merupakan sebuah objek yang berisikan data master process yang baru</param>
        /// <returns>Mengembalikan sebuah action result yang memanggil halaman create</returns>
        [HttpPost]
        public ActionResult Create(int productID, MasterProcessViewModel masterProcess)
        {
            var masterProcessList = GetMasterProcessList(productID);
            ViewBag.ProcessGroupID = new SelectList(db.ProcessGroups, "ID", "Name");
            ViewBag.ProductID = Convert.ToString(productID);
            var viewModel = new MasterProcessViewModel
            {
                MasterProcessCollections = masterProcessList
            };

            int currentProcessOrder = 1;

            if (masterProcessList.LastOrDefault() != null)
            {
                currentProcessOrder = Convert.ToInt32(masterProcessList.LastOrDefault().ProcessOrder) + 1;
            }

            try
            {
                var username = User.Identity.Name;

                Products currentProduct = db.Products.Find(productID);
                // TODO: Add master process
                var newMasterProcess = new MasterProcess
                {
                    ProductID = productID,
                    ProcessGroupID = masterProcess.ProcessGroupID,
                    Name = masterProcess.Name,
                    ProcessOrder = currentProcessOrder,//masterProcess.ProcessOrder,
                    ManHour = masterProcess.ManHour,
                    ManPower = masterProcess.ManPower,
                    CycleTime = masterProcess.CycleTime,
                    Created = DateTime.Now,
                    CreatedBy = username,
                    LastModified = DateTime.Now,
                    LastModifiedBy = username
                };
                currentProduct.IsTotalDayCalculated = false;
                currentProduct.TotalDay = currentProduct.TotalDay == null ? 0 : currentProduct.TotalDay;
                currentProduct.MasterProcess.Add(newMasterProcess);
                //db.MasterProcesses.Add(newMasterProcess);
                db.SaveChanges();

                #region Add Master Process Dependency
                //this region have function to add dependency of masterProcess
                var newMasterProcessId = newMasterProcess.ID;
                foreach (int processID in masterProcess.SelectedProcess)
                {
                    var newProcessDependency = new ProcessDependency
                    {
                        MasterProcessID = newMasterProcessId,
                        PredecessorProcessID = processID,
                        Created = DateTime.Now,
                        CreatedBy = username,
                        LastModified = DateTime.Now,
                        LastModifiedBy = username
                    };

                    db.ProcessDependencies.Add(newProcessDependency);
                }
                db.SaveChanges();
                #endregion

                ViewBag.Message = "Success";
                return View(viewModel);
            }
            catch (Exception ex)
            {
                ViewBag.Exception = ex;
            }
            return View("Error");
        }

        // GET: MasterProcess/Edit/5
        /// <summary>
        /// Fungsi Edit ini merupakan fungsi yang digunakan untuk menampilkan form untuk mengubah data Master Process yang telah ditentukan
        /// </summary>
        /// <param name="id">Merupakan sebuah bilangan integer yang berarti id Master Process yang akan diubah</param>
        /// <returns>Mengembalikan sebuah action result yang memanggil halaman edit</returns>
        public ActionResult Edit(int id)
        {
            var masterProcessData = db.MasterProcesses.Find(id);
            var masterProcessList = GetMasterProcessListThatCreatedBeforeItProcess(masterProcessData.ProductID, masterProcessData.ID);
            ViewBag.MasterProcessID = id;

            var viewModel = new MasterProcessViewModel
            {
                MasterProcessCollections = masterProcessList,
                SelectedProcess = masterProcessData.ProcessDependencies.Select(x => (int)x.PredecessorProcessID).ToList(),
                ID = masterProcessData.ID,
                ProductID = masterProcessData.ProductID,
                ProcessGroupID = masterProcessData.ProcessGroupID,
                Name = masterProcessData.Name,
                ProcessOrder = masterProcessData.ProcessOrder,
                ManHour = masterProcessData.ManHour,
                ManPower = masterProcessData.ManPower,
                CycleTime = masterProcessData.CycleTime
            };

            ViewBag.ProcessGroupID = new SelectList(db.ProcessGroups, "ID", "Name", masterProcessData.ProcessGroupID);
            return View(viewModel);
        }

        // POST: MasterProcess/Edit/5
        /// <summary>
        /// Fungsi Edit ini merupakan fungsi yang digunakan untuk menampilkan form untuk mengubah dan menyimpan perubahan data Master Process yang telah ditentukan
        /// </summary>
        /// <param name="id">sebuah bilangan integer yang berarti id MasterProcess yang akan diubah</param>
        /// <param name="masterProcess">sebuh object yang berisikan daftar perubahan master process</param>
        /// <returns>Mengembalikan sebuah action result yang memanggil halaman create</returns>
        [HttpPost]
        public ActionResult Edit(int id, MasterProcessViewModel masterProcess)
        {
            bool needCleanDailySchedule = false;
            var masterProcessData = db.MasterProcesses.Find(id);
            var username = User.Identity.Name;
            Products currentProduct = db.Products.Find(masterProcessData.ProductID);
            var masterProcessList = GetMasterProcessListThatCreatedBeforeItProcess(masterProcessData.ProductID, masterProcessData.ID);
            ViewBag.ProcessGroupID = new SelectList(db.ProcessGroups, "ID", "Name", masterProcessData.ProcessGroupID);
            ViewBag.ProductID = Convert.ToString(currentProduct.ID);

            var viewModel = new MasterProcessViewModel
            {
                MasterProcessCollections = masterProcessList
            };

            try
            {
                // TODO: Edit master Proses

                masterProcessData.ProcessGroupID = masterProcess.ProcessGroupID;
                masterProcessData.Name = masterProcess.Name;
                masterProcessData.ManHour = masterProcess.ManHour;
                masterProcessData.ManPower = masterProcess.ManPower;

                if (masterProcessData.CycleTime != masterProcess.CycleTime)
                {
                    currentProduct.IsTotalDayCalculated = false;
                    currentProduct.LastModified = DateTime.Now;
                    currentProduct.LastModifiedBy = username;
                    needCleanDailySchedule = true;
                }

                masterProcessData.CycleTime = masterProcess.CycleTime;
                masterProcessData.LastModified = DateTime.Now;
                masterProcessData.LastModifiedBy = username;
                db.SaveChanges();

                List<int> processPredecessorIDList = masterProcessData.ProcessDependencies.Select(x => (int)x.PredecessorProcessID).ToList();
                List<int> distinct1 = processPredecessorIDList.Except(masterProcess.SelectedProcess).ToList();
                List<int> distinct2 = masterProcess.SelectedProcess.Except(processPredecessorIDList).ToList();

                if (distinct2.Count > 0 || distinct1.Count > 0)
                {
                    #region Add Master Process Dependency
                    //this region have function to add dependency of masterProcess
                    if (distinct2.Count > 0)
                    {
                        foreach (int processID in distinct2)
                        {
                            var newProcessDependency = new ProcessDependency
                            {
                                MasterProcessID = masterProcessData.ID,
                                PredecessorProcessID = processID,
                                Created = DateTime.Now,
                                CreatedBy = username,
                                LastModified = DateTime.Now,
                                LastModifiedBy = username
                            };

                            db.ProcessDependencies.Add(newProcessDependency);
                        }
                    }

                    if (distinct1.Count > 0)
                    {
                        foreach (int processID in distinct1)
                        {
                            List<ProcessDependency> processDependenciesList = db.ProcessDependencies.Where(x => x.MasterProcessID == masterProcessData.ID && x.PredecessorProcessID == processID).ToList();

                            db.ProcessDependencies.RemoveRange(processDependenciesList);
                        }
                    }

                    currentProduct.IsTotalDayCalculated = false;
                    currentProduct.LastModified = DateTime.Now;
                    currentProduct.LastModifiedBy = username;

                    if (db.SaveChanges() > 0)
                    {
                        needCleanDailySchedule = true;
                    }

                    if (needCleanDailySchedule)
                    {
                        foreach (MasterProcess mp in currentProduct.MasterProcess)
                        {
                            db.ProcessDailySchedules.RemoveRange(mp.ProcessDailySchedules);
                        }
                        db.SaveChanges();
                    }
                    #endregion

                }

                ViewBag.Message = "Success";
                return View(viewModel);

            }
            catch (Exception ex)
            {
                ViewBag.Exception = ex;
            }
            return View(viewModel);
        }


        /// <summary>
        /// Fungsi UpdateMasterProcessOrder merupakan sebuah fungsi yang digunakan untuk mengupdate nilai Process Order pada sebuah product
        /// </summary>
        /// <param name="masterProcessList">merupakan list of object dari masterprocess yang merupakan daftar master process yang akan mengalami perubahan process order</param>
        /// <returns>mengembalikan json yang berisikin status dan pesan perubahan apakan berhasil atau tidak</returns>
        public ActionResult UpdateMasterProcessOrder(List<MasterProcess> masterProcessList)
        {
            try
            {
                var username = User.Identity.Name;
                DateTime now = DateTime.Now;
                foreach (MasterProcess item in masterProcessList)
                {
                    MasterProcess masterProcess = db.MasterProcesses.Find(item.ID);
                    masterProcess.ProcessOrder = item.ProcessOrder;
                    masterProcess.LastModified = now;
                    masterProcess.LastModifiedBy = username;
                    db.SaveChanges();
                }
                return Json(new { success = true, responseText = "Value successfully updated" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ViewBag.Exception(ex);
            }
            return View("Error");
        }

        // GET: MasterProcess/Delete/5
        /// <summary>
        /// Fungsi Delete merupakan sebuah fungsi yang digunakan untuk mengahapus data master process
        /// </summary>
        /// <param name="id">sebuah bilangan integer yang berarti id dari master process yang akan dihapus</param>
        /// <returns>jika berhasil akan diredirect kehalaman daftar master process namu jika tidak berhasil akan menampilkan halaman error </returns>
        public ActionResult Delete(int id)
        {
            MasterProcess masterProcessData = db.MasterProcesses.Find(id);
            Products productData = masterProcessData.Products;
            var processDependencyList = db.ProcessDependencies.Where(x => x.PredecessorProcessID == masterProcessData.ID).ToList();
            var currentProcessOrder = masterProcessData.ProcessOrder;
            ViewBag.ProductID = masterProcessData.ProductID.ToString();
            try
            {
                if (masterProcessData.Processes.Count() > 0)
                {
                    ViewBag.ErrorMessage = "Can't delete" + " " + masterProcessData.Name + " " + "because the process has been assigned ";
                    ViewBag.ProductID = masterProcessData.ProductID.ToString();
                    return View("Error");
                }
                //else if (masterProcessData.ProcessDailySchedules.Count() > 0)
                //{
                //    ViewBag.ErrorMessage = "Can't delete" + " " + masterProcessData.Name + " " + "because the process has been assigned into process daily schedule ";
                //    ViewBag.ProductID = masterProcessData.ProductID.ToString();
                //    return View("Error");
                //}
                else if (processDependencyList.Count > 0)
                {
                    ViewBag.ErrorMessage = "Can't delete" + " " + masterProcessData.Name + " " + "because the process is being used as dependecy to another process";
                    ViewBag.ProductID = masterProcessData.ProductID.ToString();
                    return View("Error");
                }
                else
                {
                    var nextProcessOrderList = db.MasterProcesses.Where(x => x.ProcessOrder > currentProcessOrder).ToList();
                    foreach (var nextProcess in nextProcessOrderList)
                    {
                        nextProcess.ProcessOrder = nextProcess.ProcessOrder - 1;
                    }

                    db.ProcessDailySchedules.RemoveRange(masterProcessData.ProcessDailySchedules);
                    db.ProcessDependencies.RemoveRange(masterProcessData.ProcessDependencies);
                    db.ProcessDependencies.RemoveRange(masterProcessData.PredecessorProcessDependencies);
                    db.MasterProcesses.Remove(masterProcessData);

                    if (db.SaveChanges() > 0)
                    {
                        if (productData.MasterProcess.Count() == 0)
                        {
                            productData.TotalDay = null;
                        }
                        else
                        {
                            productData.TotalDay = 0;
                        }
                        productData.IsTotalDayCalculated = false;
                        productData.LastModified = DateTime.Now;
                        productData.LastModifiedBy = User.Identity.Name;

                        db.SaveChanges();
                    }
                }

                return RedirectToAction("Details", "Product", new { id = productData.ID });
            }
            catch (Exception ex)
            {
                ViewBag.Exception = ex;
            }
            return View("Error");
        }
        
        /// <summary>
        /// Fungsi GetMasterProcessList merupakan fungsi untuk mendapatkan daftar master process dari sebuah product
        /// </summary>
        /// <param name="productID">sebuah bilangan integer yang berarti id dari sebuah product yang akan diambil data master processnya</param>
        /// <returns>List of Master Process yang berisikan daftar Master Process</returns>
        public List<MasterProcess> GetMasterProcessList(int productID)
        {
            return db.MasterProcesses.Where(x => x.ProductID == productID).OrderBy(x => x.ProcessOrder).ToList();
        }


        /// <summary>
        /// Merupakan sebuah fungsi untuk mendapatkan daftar master process yang dibuat sebelum master process itu sendiri (master process yang memiliki id lebih kecil dari id master process yang diinginkan)
        /// </summary>
        /// <param name="productID">sebuah bilangan integer yang berarti ID sebuah product</param>
        /// <param name="masterProcessID">sebuah bilangan integer yang berarti ID sebuah master process</param>
        /// <returns></returns>
        public List<MasterProcess> GetMasterProcessListThatCreatedBeforeItProcess(int productID, int masterProcessID)
        {
            return db.MasterProcesses.Where(x => x.ProductID == productID && x.ID < masterProcessID).OrderBy(x => x.ProcessOrder).ToList();
        }
    }
}
