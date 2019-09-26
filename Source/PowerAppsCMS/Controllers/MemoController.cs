using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PowerAppsCMS.Models;
using PowerAppsCMS.ViewModel;
using PagedList;
using PowerAppsCMS.Constants;
using PowerAppsCMS.CustomAuthentication;

namespace PowerAppsCMS.Controllers
{
    /// <summary>
    /// Merupakan controller yang berisikin fungsi-fungsi CRUD modul Memo
    /// </summary>
    public class MemoController : Controller
    {
        private PowerAppsCMSEntities db = new PowerAppsCMSEntities();
        // GET: Memo
        /// <summary>
        /// Fungsi Index berfungsi untuk mengambil dan menampilkan daftar data Memo yang ada
        /// </summary>
        /// <param name="searchName">sebuah string yang berarti nama dari product yang akan di cari memonya</param>
        /// <param name="currentFilter">sebuah string yang berarti nama dari product yang sedang di cari memonya</param>
        /// <param name="page">sebuah bilangan integer yang boleh kosong dan berfungsi sebagai angka parameter nomor halaman</param>
        /// <returns>mamanggil halaman daftar memo dengan data yang sesuai dengan filter parameter</returns>
        [CustomAuthorize(Roles = RoleNames.PPC + "," + RoleNames.SuperAdmin + "," + RoleNames.Supervisor+","+RoleNames.GroupLeaderPB)]
        public ActionResult Index(string searchName, string currentFilter, int? page)
        {
            Session.Remove("newMemo");
            Session.Remove("message");
            if (searchName != null)
            {
                page = 1;
            }
            else
            {
                searchName = currentFilter;
            }

            ViewBag.CurrentFilter = searchName;

            var memoList = from x in db.Memos
                           select x;
            if (!String.IsNullOrEmpty(searchName))
            {
                memoList = memoList.Where(x => x.Products.Name.Contains(searchName));
            }
            int pageSize = 10;
            int pageNumber = (page ?? 1);

            ViewBag.Page = page.ToString();

            return View(memoList.OrderByDescending(x => x.Created).ToPagedList(pageNumber, pageSize));
        }

        /// <summary>
        /// merupakan fungsi untuk menampilkan halaman form create Memo
        /// </summary>
        /// <returns>>Menampilkan halaman create memo</returns>
        [CustomAuthorize(Roles = RoleNames.PPC + "," + RoleNames.SuperAdmin)]
        public ActionResult Create()
        {
            List<Services.NewMemoServices> memo = (List<Services.NewMemoServices>)Session["newMemo"];
            List<Products> productDataList = db.Products.Where(x => x.ProductComposition.Count > 0 && x.PRO.Count > 0).ToList();

            if (Session["newMemo"] != null)
            {
                ViewBag.ProductID = new SelectList(productDataList, "ID", "PartNumberName", memo[0].Memo.ProductID);
                ViewBag.MemoTypeID = new SelectList(db.MemoTypes, "ID", "Description", memo[0].Memo.MemoTypeID);
                ViewBag.Description = memo[0].Memo.Description;
                var viewModel = new MemoViewModel
                {
                    Memo = memo[0].Memo
                };

                return View(viewModel);
            }
            else
            {
                ViewBag.ProductID = new SelectList(productDataList, "ID", "PartNumberName");
                ViewBag.MemoTypeID = new SelectList(db.MemoTypes, "ID", "Description");
                ViewBag.SessionStatus = "null";
                return View();
            }
        }

        /// <summary>
        /// Merupakan fungsi post yang digunakan untuk menambahkan data memo
        /// </summary>
        /// <param name="memoViewModel">sebuah object memoViewModel yang berisikan data memo yang akan dibuat</param>
        /// <returns>Ketika data baru berhasil di input, maka web akan menavigasikan ke halaman details memo</returns>
        [HttpPost]
        [CustomAuthorize(Roles = RoleNames.PPC + "," + RoleNames.SuperAdmin)]
        public ActionResult Create(MemoViewModel memoViewModel)
        {
            var productDataList = (from x in db.Products
                                   where db.ProductCompositions.Select(y => y.ProductID).Contains(x.ID)
                                   select x).ToList();

            ViewBag.ProductID = new SelectList(productDataList, "ID", "PartNumberName");
            ViewBag.MemoTypeID = new SelectList(db.MemoTypes, "ID", "Description");
            var username = User.Identity.Name;
            try
            {
                if (Session["newMemo"] == null)
                {
                    List<Services.NewMemoServices> memo = new List<Services.NewMemoServices>();
                    List<MemoPRO> listOfMemoPRO = new List<MemoPRO>();

                    memoViewModel.Memo.ProductID = memoViewModel.ProductID;
                    memoViewModel.Memo.MemoTypeID = memoViewModel.MemoTypeID;

                    memo.Add(new Services.NewMemoServices(memoViewModel.Memo, memoViewModel.ProductID, listOfMemoPRO, ""));
                    Session["newMemo"] = memo;
                    return RedirectToAction("CreateMemoPRO");
                }
                else
                {

                    List<Services.NewMemoServices> memo = (List<Services.NewMemoServices>)Session["newMemo"];
                    if (memo[0].Memo.ProductID != memoViewModel.ProductID)
                    {
                        memo[0].ListOfMemoPROs = null;
                        memo[0].Memo.ProductID = memoViewModel.ProductID;
                        memo[0].SelectedProduct = memoViewModel.ProductID;
                        memo[0].Memo.MemoTypeID = memoViewModel.MemoTypeID;
                        memo[0].Memo.Description = memoViewModel.Memo.Description;
                        return RedirectToAction("CreateMemoPRO");
                    }
                    else
                    {
                        memo[0].Memo.ProductID = memoViewModel.ProductID;
                        memo[0].SelectedProduct = memoViewModel.ProductID;
                        memo[0].Memo.MemoTypeID = memoViewModel.MemoTypeID;
                        memo[0].Memo.Description = memoViewModel.Memo.Description;
                        return RedirectToAction("CreateMemoPRO");
                    }
                }

            }
            catch (Exception ex)
            {
                ViewBag.Exception = ex;
            }
            return View("Error");
        }


        /// <summary>
        /// Merupakan fungsi yang digunakan untuk menambahkan data create memo pro
        /// </summary>
        /// <param name="memoViewModel">sebuah object MemoViewModel yang berisikan data memo yang akan di tambahkan data memo pro nya</param>
        /// <returns>menampilkan halaman details</returns>
        [CustomAuthorize(Roles = RoleNames.PPC + "," + RoleNames.SuperAdmin)]
        public ActionResult CreateMemoPRO(MemoViewModel memoViewModel)
        {
            var username = User.Identity.Name;
            List<Services.NewMemoServices> memo = (List<Services.NewMemoServices>)Session["newMemo"];
            int productID = memo[0].SelectedProduct;
            var selectedPROList = db.Pros.Where(x => x.ProductID == productID).ToList();
            var viewModel = new MemoViewModel
            {
                PROCollections = selectedPROList
            };

            try
            {
                List<MemoPRO> listOfMemoPRO = new List<MemoPRO>();

                foreach (var pro in memoViewModel.selectedProID)
                {
                    var currentPRO = db.Pros.Find(pro);
                    var newMemoPRO = new MemoPRO
                    {
                        PROID = pro,
                        Number = currentPRO.Number
                    };
                    listOfMemoPRO.Add(newMemoPRO);
                }
                memo[0].ListOfMemoPROs = listOfMemoPRO;
                Session["newMemo"] = memo;
                return View(viewModel);
            }
            catch (Exception ex)
            {
                ViewBag.Exception = ex;
            }
            return View(viewModel);
        }

        /// <summary>
        /// Merupakan fungsi yang digunakan untuk menambahkan data create memo component
        /// </summary>
        /// <param name="memoViewModel">sebuah object MemoViewModel yang berisikan data memo yang akan di tambahkan data memo pro nya</param>
        /// <returns>menampilkan halaman details</returns>
        [CustomAuthorize(Roles = RoleNames.PPC + "," + RoleNames.SuperAdmin)]
        public ActionResult CreateMemoComponent(MemoViewModel memoViewModel)
        {
            List<Services.NewMemoServices> memo = (List<Services.NewMemoServices>)Session["newMemo"];
            var currentMemo = memo[0].Memo;
            var currentProduct = db.Products.Find(currentMemo.ProductID);
            var currentMemoType = db.MemoTypes.Find(currentMemo.MemoTypeID);
            var currentListOfMemoPRO = memo[0].ListOfMemoPROs;
            var listOfProductComposition = db.ProductCompositions.Where(x => x.ProductID == currentMemo.ProductID).ToList();
            var viewModel = new MemoViewModel
            {
                Product = currentProduct,
                Memo = currentMemo,
                MemoType = currentMemoType,
                ListofMemoPRO = currentListOfMemoPRO,
                ProductCompositions = listOfProductComposition
            };

            if (memo[0].Message != null)
            {
                Session.Remove(memo[0].Message);
            }
            return View(viewModel);

        }

        /// <summary>
        /// Berfungsi untuk menambahkan memo baru ke dalam database
        /// </summary>
        /// <param name="memoViewModel">Sebuah object dari memo view model</param>
        /// <returns>Ketika memo berhasil ditambahkan, maka web akan meredirect ke halaman detail</returns>
        public ActionResult SaveMemo(MemoViewModel memoViewModel)
        {
            List<Services.NewMemoServices> memo = (List<Services.NewMemoServices>)Session["newMemo"];
            var currentMemo = memo[0].Memo;
            var currentListOfMemoPRO = memo[0].ListOfMemoPROs;
            try
            {
                var username = User.Identity.Name;
                var newMemo = new Memo
                {
                    Description = currentMemo.Description,
                    MemoTypeID = currentMemo.MemoTypeID,
                    ProductID = currentMemo.ProductID,
                    Created = DateTime.Now,
                    CreatedBy = username,
                    LastModified = DateTime.Now,
                    LastModifiedBy = username
                };
                db.Memos.Add(newMemo);
                db.SaveChanges();
                var newMemoID = newMemo.ID;

                foreach (var memoPro in currentListOfMemoPRO)
                {
                    var newMemoPRO = new MemoPRO
                    {
                        MemoID = newMemoID,
                        PROID = memoPro.PROID,
                        Quantity = memoPro.Quantity,
                        Created = DateTime.Now,
                        CreatedBy = username,
                        LastModified = DateTime.Now,
                        LastModifiedBy = username
                    };
                    db.MemoPROes.Add(newMemoPRO);
                }
                db.SaveChanges();
                var selectedComponent = memoViewModel.SelectedComponent;

                foreach (int componentID in memoViewModel.SelectedComponent)
                {
                    var newMemoComponent = new MemoComponent
                    {
                        MemoID = newMemoID,
                        ComponentID = componentID,
                        Created = DateTime.Now,
                        CreatedBy = username,
                        LastModified = DateTime.Now,
                        LastModifiedBy = username
                    };
                    db.MemoComponents.Add(newMemoComponent);
                }
                db.SaveChanges();
                Session.Remove("newMemo");
                return RedirectToAction("Details", "Memo", new { id = newMemoID });
            }
            catch (Exception ex)
            {
                ViewBag.Exception = ex;
            }
            return View("Error");
        }

        /// <summary>
        /// Merupakan fungsi yang digunakan untuk mengubah data  memo pro
        /// </summary>
        /// <param name="collection">Object dari FormCollection yang isinya data dari input form halaman view</param>
        /// <param name="memoViewModel">Sebuah object view model dari MemoViewModel</param>
        /// <returns></returns>
        public ActionResult UpdateMemoPRO(FormCollection collection, MemoViewModel memoViewModel)
        {
            List<Services.NewMemoServices> memo = (List<Services.NewMemoServices>)Session["newMemo"];
            string[] quantity = collection.GetValues("quantityPRO");
            string[] currentPROID = collection.GetValues("proID");
            memo[0].Message = "";

            int productID = memo[0].SelectedProduct;
            var selectedPRO = db.Pros.Where(x => x.ProductID == productID).ToList();
            var viewModel = new MemoViewModel
            {
                PROCollections = selectedPRO

            };
            
            for (int i = 0; i < memo[0].ListOfMemoPROs.Count; i++)
            {
                var existPRO = db.Pros.Find(Convert.ToInt32(currentPROID[i]));
                if (quantity[i] == "" || quantity[i] == null)
                {
                    ViewBag.ErrorMessage = "Quantity value form can't be empty";
                    memo[0].Message = ViewBag.ErrorMessage;
                    return new RedirectResult("CreateMemoPRO");
                }
                else
                {
                    if (Convert.ToInt32(quantity[i]) <= existPRO.Quantity && Convert.ToInt32(quantity[i]) > 0)
                    {
                        int proID = Convert.ToInt32(currentPROID[i]);
                        var currentMemoPRO = memo[0].ListOfMemoPROs.Where(x => x.PROID == proID).FirstOrDefault();
                        currentMemoPRO.Quantity = Convert.ToInt32(quantity[i]);
                    }
                    else if (Convert.ToInt32(quantity[i]) == 0)
                    {
                        ViewBag.ErrorMessage = "Quantity value can't be 0 or form can't be empty";
                        memo[0].Message = ViewBag.ErrorMessage;
                        return new RedirectResult("CreateMemoPRO");
                    }
                    else
                    {
                        ViewBag.Message = "Quantity can't be more than PRO quantity";
                        memo[0].Message = ViewBag.Message;
                        return new RedirectResult("CreateMemoPRO");
                    }
                }
            }

            return RedirectToAction("CreateMemoComponent");
        }

        /// <summary>
        /// Berfungsi untuk menambahkan pro di halaman edit memo
        /// </summary>
        /// <param name="memoViewModel">Sebuah object dari memo view model</param>
        /// <param name="collection">Object dari FormCollection yang isinya data dari input form halaman view</param>
        /// <returns>PRO berhasil ditambahkan</returns>
        public ActionResult AddPRO(MemoViewModel memoViewModel, FormCollection collection)
        {
            try
            {
                var username = User.Identity.Name;
                var memoID = collection.GetValues("Memo.ID");
                foreach (var pro in memoViewModel.selectedProID)
                {
                    var proData = db.Pros.Find(pro);
                    var newMemoPRO = new MemoPRO
                    {
                        MemoID = Convert.ToInt32(memoID[0]),
                        PROID = pro,
                        Quantity = proData.Quantity,
                        Created = DateTime.Now,
                        CreatedBy = username,
                        LastModified = DateTime.Now,
                        LastModifiedBy = username
                    };
                    db.MemoPROes.Add(newMemoPRO);
                }
                db.SaveChanges();
                Session["message"] = "pro added";
                return RedirectToAction("Edit", "Memo", new { id = memoID[0] });
            }
            catch (Exception ex)
            {
                ViewBag.Exception = ex;
            }
            return View("Error");
        }

        /// <summary>
        /// Berfungsi untuk menambahkan component di halaman edit memo
        /// </summary>
        /// <param name="memoID">Sebuah integer yang merupakan id dari memo</param>
        /// <param name="memoViewModel">Sebuah object dari memo view model</param>
        /// <returns>Component berhasil ditambahkan</returns>
        public ActionResult AddMemoComponent(int memoID, MemoViewModel memoViewModel)
        {
            try
            {
                var username = User.Identity.Name;
                foreach (var memoComponent in memoViewModel.selectedPROComponentID)
                {
                    var newMemoComponent = new MemoComponent
                    {
                        MemoID = memoID,
                        ComponentID = memoComponent,
                        Created = DateTime.Now,
                        CreatedBy = username,
                        LastModified = DateTime.Now,
                        LastModifiedBy = username
                    };
                    db.MemoComponents.Add(newMemoComponent);
                }
                db.SaveChanges();
                Session["message"] = "component added";
                return RedirectToAction("Edit", "Memo", new { id = memoID });
            }
            catch (Exception ex)
            {
                ViewBag.Exception = ex;
            }
            return View("Error");
        }

        /// <summary>
        /// Berfungsi untuk menampilkan detail dari memo
        /// </summary>
        /// <param name="id">Sebuah integer yang nilainya boleh kosong yang merupakan id dari memo</param>
        /// <param name="page">Sebuah bilangan integer yang boleh kosong dan berfungsi sebagai angka parameter nomor halaman</param>
        /// <returns></returns>
        [CustomAuthorize(Roles = RoleNames.PPC + "," + RoleNames.SuperAdmin + "," + RoleNames.Supervisor+","+RoleNames.GroupLeaderPB)]
        public ActionResult Details(int? id, int? page)
        {
            if (id.HasValue)
            {
                Session.Remove("message");
                var memoData = db.Memos.Find(id);

                if (memoData != null)
                {
                    var memoPROList = db.MemoPROes.Where(x => x.MemoID == id).ToList();
                    var memoComponentList = db.MemoComponents.Where(x => x.MemoID == id).ToList();
                    var selectedPROList = (from x in db.Pros
                                           where x.ProductID == memoData.ProductID && !db.MemoPROes.Select(y => y.PROID).Contains(x.ID)
                                           select x).ToList();
                    var selectedProComponentList = (from x in db.ProductCompositions
                                                    where x.ProductID == memoData.ProductID && !db.MemoComponents.Where(y => y.MemoID == memoData.ID).Select(y => y.ComponentID).Contains(x.ComponentID)
                                                    select x).ToList();
                    //var selectedProComponentList = db.ProductCompositions.Where(x => x.ProductID == memoData.ProductID).ToList();
                    var viewModel = new MemoViewModel
                    {
                        Memo = memoData,
                        ListofMemoPRO = memoPROList,
                        ListofMemoComponents = memoComponentList,
                        PROCollections = selectedPROList,
                        ProductCompositions = selectedProComponentList
                    };

                    ViewBag.Page = page.ToString();
                    return View(viewModel);
                }
                else
                {
                    ViewBag.ErrorMessage = "Sorry we couldn't find this memo";
                    return View("Error");
                }
            }
            else
            {
                return RedirectToAction("Index", "Memo");
            }
        }

        /// <summary>
        /// Befungsi untuk menampilkan halaman edit memo
        /// </summary>
        /// <param name="id">Sebuah integer yang nilainya boleh kosong yang merupakan id dari memo</param>
        /// <returns>Menampilkan halaman edit</returns>
        [CustomAuthorize(Roles = RoleNames.PPC + "," + RoleNames.SuperAdmin)]
        public ActionResult Edit(int? id)
        {
            if (id.HasValue)
            {
                var memoData = db.Memos.Find(id);
                if (memoData != null)
                {
                    var memoPROList = db.MemoPROes.Where(x => x.MemoID == id).ToList();
                    var memoComponentList = db.MemoComponents.Where(x => x.MemoID == id).ToList();
                    var selectedPROList = (from x in db.Pros
                                           where x.ProductID == memoData.ProductID && !db.MemoPROes.Select(y => y.PROID).Contains(x.ID)
                                           select x).ToList();
                    var selectedProComponentList = (from x in db.ProductCompositions
                                                    where x.ProductID == memoData.ProductID && !db.MemoComponents.Where(y => y.MemoID == memoData.ID).Select(y => y.ComponentID).Contains(x.ComponentID)
                                                    select x).ToList();

                    ViewBag.MemoTypeID = new SelectList(db.MemoTypes, "ID", "Description", memoData.MemoTypeID);
                    ViewBag.ProductID = new SelectList(db.Products, "ID", "Name", memoData.ProductID);
                    var viewModel = new MemoViewModel
                    {
                        Memo = memoData,
                        ListofMemoPRO = memoPROList,
                        ListofMemoComponents = memoComponentList,
                        PROCollections = selectedPROList,
                        ProductCompositions = selectedProComponentList
                    };
                    return View(viewModel);
                }
                else
                {
                    ViewBag.ErrorMessage = "Sorry we couldn't find this memo";
                    return View("Error");
                }
            }
            else
            {
                return RedirectToAction("Index", "Memo");
            }
        }

        /// <summary>
        /// Berfungsi untuk mengubah sebuah data dari memo yang dipilih oleh user
        /// </summary>
        /// <param name="id">Sebuah integer yang merupakan id dari memo</param>
        /// <param name="memoViewModel">Sebuah object dari memo view model</param>
        /// <returns>Jika data berhasil dirubah, maka web akan meredirect ke halaman detail</returns>
        [HttpPost]
        [CustomAuthorize(Roles = RoleNames.PPC + "," + RoleNames.SuperAdmin)]
        public ActionResult Edit(int id, MemoViewModel memoViewModel)
        {
            var memoData = db.Memos.Find(id);
            var memoPROList = db.MemoPROes.Where(x => x.MemoID == id).ToList();
            var memoComponentList = db.MemoComponents.Where(x => x.MemoID == id).ToList();
            var selectedPROList = (from x in db.Pros
                                   where x.ProductID == memoData.ProductID && !db.MemoPROes.Select(y => y.PROID).Contains(x.ID)
                                   select x).ToList();
            var selectedProComponentList = (from x in db.ProductCompositions
                                            where x.ProductID == memoData.ProductID && !db.MemoComponents.Where(y => y.MemoID == memoData.ID).Select(y => y.ComponentID).Contains(x.ComponentID)
                                            select x).ToList();

            ViewBag.MemoTypeID = new SelectList(db.MemoTypes, "ID", "Description", memoData.MemoTypeID);
            ViewBag.ProductID = new SelectList(db.Products, "ID", "Name", memoData.ProductID);
            var username = User.Identity.Name;

            var viewModel = new MemoViewModel
            {
                Memo = memoData,
                ListofMemoPRO = memoPROList,
                ListofMemoComponents = memoComponentList,
                PROCollections = selectedPROList,
                ProductCompositions = selectedProComponentList
            };

            try
            {
                memoData.MemoTypeID = memoViewModel.MemoTypeID;
                memoData.Description = memoViewModel.Memo.Description;
                db.SaveChanges();

                ViewBag.Success = "success";
                ViewBag.MemoID = memoData.ID;
                return View(viewModel);
            }
            catch (Exception ex)
            {
                ViewBag.Exception = ex;
            }
            return View("Error");
        }

        /// <summary>
        /// Berfungsi untuk mengubah quantity dari pro pada saat pro baru di input oleh user ketika user ingin create memo baru
        /// </summary>
        /// <param name="memoPRO">Sebuah object dari memo pro</param>
        /// <returns>Quantity berhasil ditambahkan</returns>
        public ActionResult EditMemoPRO(MemoPRO memoPRO)
        {
            try
            {
                var memoPROData = db.MemoPROes.Find(memoPRO.ID);
                if (memoPRO.Quantity > memoPROData.PRO.Quantity)
                {
                    return Json(new { success = false, responseText = "Update quantity failed. Quantity that inserted is higher than PRO quantity" }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    memoPROData.Quantity = memoPRO.Quantity;
                    db.SaveChanges();
                    return Json(new { success = true, responseText = "Quantity successfully updated" }, JsonRequestBehavior.AllowGet);
                }

            }
            catch (Exception ex)
            {
                ViewBag.Exception = ex;
            }
            return View("Error");
        }

        /// <summary>
        /// Merupakan sebuah fungsi yang digunakan untuk menghapus data memo
        /// </summary>
        /// <param name="id">sebuah bilangan integer yang berarti id dari memo yang akan dihapus</param>
        /// <returns>Jika berhasil akan memanggil halaman daftar memo, namun jika tidak berhasil akan menampilkan halaman error</returns>
        [CustomAuthorize(Roles = RoleNames.PPC + "," + RoleNames.SuperAdmin)]
        public ActionResult Delete(int id)
        {
            try
            {
                var memoPROList = db.MemoPROes.Where(x => x.MemoID == id).ToList();
                db.MemoPROes.RemoveRange(memoPROList);
                db.SaveChanges();

                var memoComponentList = db.MemoComponents.Where(x => x.MemoID == id).ToList();
                db.MemoComponents.RemoveRange(memoComponentList);
                db.SaveChanges();

                var memoData = db.Memos.Find(id);
                db.Memos.Remove(memoData);
                db.SaveChanges();

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ViewBag.Exception = ex;
            }
            return View("Error");

        }

        /// <summary>
        /// Merupakan sebuah fungsi yang digunakan untuk menghapus data memo pro
        /// </summary>
        /// <param name="id">sebuah bilangan integer yang berarti id dari memo pro yang akan dihapus</param>
        /// <returns>Jika berhasil akan memanggil halaman edit Memo, namun jika tidak berhasil akan menampilkan halaman error</returns>
        public ActionResult DeleteMemoPRO(int id)
        {
            var memoPROData = db.MemoPROes.Find(id);
            var memoID = memoPROData.MemoID;
            try
            {
                db.MemoPROes.Remove(memoPROData);
                db.SaveChanges();
                Session["message"] = "pro deleted";
                return RedirectToAction("Edit", "Memo", new { id = memoID });
            }
            catch (Exception ex)
            {
                ViewBag.Exception = ex;
            }
            return View("Error");
        }

        /// <summary>
        /// Merupakan sebuah fungsi yang digunakan untuk menghapus data memo component
        /// </summary>
        /// <param name="id">sebuah bilangan integer yang berarti id dari memo component yang akan dihapus</param>
        /// <returns>Jika berhasil akan memanggil halaman edit Memo, namun jika tidak berhasil akan menampilkan halaman error</returns>
        public ActionResult DeleteMemoPROComponent(int id)
        {
            var memoComponentData = db.MemoComponents.Find(id);
            var memoID = memoComponentData.MemoID;
            try
            {
                db.MemoComponents.Remove(memoComponentData);
                db.SaveChanges();
                Session["message"] = "component deleted";
                return RedirectToAction("Edit", "Memo", new { id = memoID });
            }
            catch (Exception ex)
            {
                ViewBag.Exception = ex;
            }
            return View("Error");
        }
    }
}