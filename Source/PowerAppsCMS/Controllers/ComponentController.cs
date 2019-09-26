using System;
using System.Linq;
using System.Web.Mvc;
using PowerAppsCMS.Models;
using PagedList;
using PowerAppsCMS.ViewModel;
using PowerAppsCMS.CustomAuthentication;
using PowerAppsCMS.Constants;

namespace PowerAppsCMS.Controllers
{
    /// <summary>
    /// ComponentController berfungsi sebagai CRUD component
    /// </summary>
    /// <remarks>
    /// selain sebagai CRUD component, ComponentController juga berfungsi sebagai CRUD material preparation
    /// </remarks>
    [CustomAuthorize(Roles = RoleNames.PE + "," + RoleNames.SuperAdmin)]
    public class ComponentController : Controller
    {
        private PowerAppsCMSEntities db = new PowerAppsCMSEntities();

        /// <summary>
        /// Method Index berfungsi untuk menampilkan list semua component
        /// </summary>
        /// <param name="searchName">Parameter searchName digunakan untuk mencari component berdasarkan nama yang di input</param>
        /// <param name="currentFilter">Parameter yang digunakan untuk mengatur filter ketika user membuka halaman saat ini atau berikutnya</param>
        /// <param name="page">Parameter nomor halaman</param>
        /// <returns>Menampilkan list semua component</returns>
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

            var componentList = from x in db.Components
                                select x;
            if (!String.IsNullOrEmpty(searchName))
            {
                componentList = componentList.Where(x => x.PartNumber.Contains(searchName));
            }

            int pageSize = 10;
            int pageNumber = (page ?? 1);

            ViewBag.PageNumber = pageNumber.ToString();
            ViewBag.ItemperPage = pageSize.ToString();
            ViewBag.Page = page.ToString();

            return View(componentList.OrderBy(x => x.PartNumber).ToPagedList(pageNumber, pageSize));
        }

        /// <summary>
        /// Method Details berfungsi untuk menampilkan detail dari sebuah component
        /// </summary>
        /// <param name="id">Parameter id merupakan id dari component</param>
        /// <param name="page">Parameter nomor halaman</param>
        /// <returns>Menampilkan detail dari sebuah component</returns>
        public ActionResult Details(int? id, int? page)
        {
            if (id.HasValue)
            {
                var componentData = db.Components.Find(id);
                if (componentData != null)
                {
                    var componentMaterialPreparationList = db.ComponentMaterialPreparationProcesses.Where(x => x.ComponentID == id).OrderBy(x => x.MaterialPreparationProcess.Name).ToList();

                    var materialPreparationProcessList = (from x in db.MaterialPreparationProcesses
                                                          where !db.ComponentMaterialPreparationProcesses.Where(c => c.ComponentID == id).Select(c => c.MaterialPreparationProcessID).Contains(x.ID)
                                                          select x).ToList();

                    var viewModel = new ComponentViewModel
                    {
                        Component = componentData,
                        MaterialPreparationProcessesCollections = materialPreparationProcessList,
                        ComponentMaterialPreparationProcesses = componentMaterialPreparationList

                    };
                    ViewBag.Page = page.ToString();
                    return View(viewModel);
                }
                else
                {
                    ViewBag.ErrorMessage = "Sorry we could not find the component";
                    return View("Error");
                }
            }
            else
            {
                return RedirectToAction("Index", "Component");
            }
            
        }

        /// <summary>
        /// Method Create berfungsi untuk menampilkan halaman create
        /// </summary>
        /// <returns>Menampilkan halaman create</returns>
        public ActionResult Create()
        {
            Component component = new Component();
            ViewBag.MaterialID = new SelectList(db.Materials.OrderBy(x => x.Name), "ID", "Name");
            component.IsInHouse = true;
            return View(component);
        }

        /// <summary>
        /// Method Post Create berfungsi untuk menambah data baru di component
        /// </summary>
        /// <param name="component">Parameter model dari component</param>
        /// <returns>Jika data berhasil di input, maka web akan menavigasikan ke halaman detail</returns>
        [HttpPost]
        public ActionResult Create(Component component)
        {
            ViewBag.MaterialID = new SelectList(db.Materials.OrderBy(x => x.Name), "ID", "Name");
            try
            {
                var username = User.Identity.Name;
                var existComponent = db.Components.Where(x => x.PartNumber == component.PartNumber).SingleOrDefault();

                // TODO: Add component
                if (existComponent != null)
                {
                    ViewBag.ExistComponent = "Component already exist";
                    return View();
                }
                else
                {
                    component.Created = DateTime.Now;
                    component.CreatedBy = username;
                    component.LastModified = DateTime.Now;
                    component.LastModifiedBy = username;
                    db.Components.Add(component);
                    db.SaveChanges();
                    return RedirectToAction("Details", "Component", new { id = component.ID });
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
        /// <param name="id">Parameter id merupakan id dari component</param>
        /// <param name="page">Parameter nomor halaman</param>
        /// <returns></returns>
        public ActionResult Edit(int? id, int? page)
        {
            if (id.HasValue)
            {
                var componentData = db.Components.Find(id);
                if (componentData != null)
                {
                    ViewBag.MaterialID = new SelectList(db.Materials.OrderBy(x => x.Name), "ID", "Name", componentData.MaterialID);
                    ViewBag.Page = page.ToString();
                    return View(componentData);
                }
                else
                {
                    ViewBag.ErrorMessage = "Sorry we could not find the component";
                    return View("Error");
                }
            }
            else
            {
                return RedirectToAction("Index", "Component");
            }
        }

        /// <summary>
        /// Method Post Edit berfungsi untuk merubah sebuah data dari component
        /// </summary>
        /// <param name="id">Parameter id merupakan id dari component</param>
        /// <param name="component">Parameter model dari component</param>
        /// <param name="collection">Parameter yang digunakan oleh object FormCollection</param>
        /// <returns>Jika data berhasil dirubah, maka web akan menavigasikan ke halaman detail</returns>
        [HttpPost]
        public ActionResult Edit(int id, Component component, FormCollection collection)
        {
            var username = User.Identity.Name;
            var componentData = db.Components.Find(id);
            var currentPage = collection.GetValues("currentPage");
            ViewBag.MaterialID = new SelectList(db.Materials.OrderBy(x => x.Name), "ID", "Name", componentData.MaterialID);
            ViewBag.ComponentID = componentData.ID.ToString();
            ViewBag.CurrentPage = currentPage[0];
            try
            {
                var existComponent = db.Components.Where(x => x.PartNumber == component.PartNumber && x.ID != component.ID).SingleOrDefault();

                // TODO: Add update logic here
                if (existComponent != null)
                {
                    ViewBag.ExistComponent = "Component already exist";
                    return View(componentData);
                }
                else
                {
                    componentData.PartNumber = component.PartNumber;
                    componentData.PartName = component.PartName;
                    componentData.MaterialID = component.MaterialID;
                    componentData.Long = component.Long;
                    componentData.Width = component.Width;
                    componentData.Thickness = component.Thickness;
                    componentData.OuterDiameter = component.OuterDiameter;
                    componentData.InnerDiameter = component.InnerDiameter;
                    componentData.IsInHouse = component.IsInHouse;
                    componentData.LastModified = DateTime.Now;
                    componentData.LastModifiedBy = username;
                    db.SaveChanges();

                    ViewBag.Message = "Success";
                    return View(componentData);
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
        /// Method Delete berfungsi untuk menghapus sebuah data dari component
        /// </summary>
        /// <param name="id">Parameter id merupakan id dari component</param>
        /// <returns>Jika data berhasil di hapus, maka web akan menavigasikan ke halaman index</returns>
        public ActionResult Delete(int id)
        {
            var componentData = db.Components.Find(id);
            try
            {
                if (componentData.ComponentMaterialPreparationProcesses.Count() > 0)
                {
                    ViewBag.ErrorMessage = "Component with part number" + " " + componentData.PartNumber + " " + "could not be deleted because there are some material process on this component";
                    return View("Error");
                }
                else if (componentData.MemoComponents.Count() > 0)
                {
                    ViewBag.ErrorMessage = "Component with part number" + " " + componentData.PartNumber + " " + "could not be deleted because this component already assigned on memo";
                    return View("Error");
                }
                else if (componentData.ProductCompositions.Count() > 0)
                {
                    ViewBag.ErrorMessage = "Component with part number" + " " + componentData.PartNumber + " " + "could not be deleted because some products are using this component";
                    return View("Error");
                }
                else
                {
                    db.Components.Remove(componentData);
                    db.SaveChanges();

                    return RedirectToAction("Index");
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
        /// Method AddMaterialPreparation berfungsi untuk menambah material preparation
        /// </summary>
        /// <param name="componentID">Parameter componentID merupakan id dari component</param>
        /// <param name="componentViewModel">Parameter componentViewModel merupakan view model dari component</param>
        /// <param name="collection">Parameter collection merupakan parameter yang digunakan oleh object FormCollection</param>
        /// <returns>Jika data berhasil ditambahkan, maka web akan menavigasikan ke halaman detail component</returns>
        public ActionResult AddMaterialPreparation(int componentID, ComponentViewModel componentViewModel, FormCollection collection)
        {
            try
            {
                var username = User.Identity.Name;
                var currentPage = collection.GetValues("currentPage");

                foreach (var materialPreparation in componentViewModel.SelectedMaterialPreparationProcess)
                {
                    var existingComponentMaterialPreparation = db.ComponentMaterialPreparationProcesses.Where(x => x.ComponentID == componentID && x.MaterialPreparationProcessID == materialPreparation).SingleOrDefault();

                    if (existingComponentMaterialPreparation != null)
                    {

                        ViewBag.ComponentID = componentID.ToString();
                        ViewBag.ErrorMessage = "Can't add material preparation process, because" + "" + existingComponentMaterialPreparation.MaterialPreparationProcess.Name + "" + "already exist on this component";
                        return View("Error");
                    }
                    else
                    {
                        var newMaterialPreparationProcess = new ComponentMaterialPreparationProcess
                        {
                            ComponentID = componentID,
                            MaterialPreparationProcessID = materialPreparation,
                            Created = DateTime.Now,
                            CreatedBy = username,
                            LastModified = DateTime.Now,
                            LastModifiedBy = username
                        };
                        db.ComponentMaterialPreparationProcesses.Add(newMaterialPreparationProcess);
                        db.SaveChanges();
                    }
                }
                return RedirectToAction("Details", "Component", new { id = componentID, page = currentPage[0] });
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "An error occured, please check your data input and try again";
                ViewBag.Exception = ex;
            }
            return View("Error");
        }

        /// <summary>
        /// MethodEditMaterialPreparation befungsi untuk mengubah sebuah data di material preparation
        /// </summary>
        /// <param name="componentMaterialPreparationProcess">Parameter componentMaterialPreparationProcess merupakan model dari component material preparation process</param>
        /// <returns>Data berhasil dirubah</returns>
        public ActionResult EditMaterialPreparation(ComponentMaterialPreparationProcess componentMaterialPreparationProcess)
        {
            try
            {
                var componentMaterialPreparationData = db.ComponentMaterialPreparationProcesses.Find(componentMaterialPreparationProcess.ID);
                componentMaterialPreparationData.Value = componentMaterialPreparationProcess.Value;
                db.SaveChanges();

                return Json(new { success = true, responseText = "Value successfully updated" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "An error occured, please check your data input and try again";
                ViewBag.Exception(ex);
            }
            return View("Error");
        }

        /// <summary>
        /// Method DeleteMaterialPreparation berfungsi untuk menghapus sebuah data di material preparation
        /// </summary>
        /// <param name="id">Parameter id merupakan id dari component material preparation process</param>
        /// <param name="page">Parameter nomor halaman</param>
        /// <returns>Jika data berhasil di hapus, maka web akan menavigasikan ke halaman detail component</returns>
        public ActionResult DeleteMaterialPreparation(int id,  int? page)
        {
            try
            {
                var componentMaterialPreparationData = db.ComponentMaterialPreparationProcesses.Find(id);
                var componentID = componentMaterialPreparationData.ComponentID;
                var currentPage = page.ToString();
                db.ComponentMaterialPreparationProcesses.Remove(componentMaterialPreparationData);
                db.SaveChanges();

                return RedirectToAction("Details", "Component", new { id = componentID , page = currentPage});
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
