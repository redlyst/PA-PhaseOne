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
    /// Controller untuk mapping antara sales order dan serial number
    /// </summary>
    [CustomAuthorize(Roles = RoleNames.PE + "," + RoleNames.SuperAdmin)]
    public class MappingSalesOrderController : Controller
    {
        private PowerAppsCMSEntities db = new PowerAppsCMSEntities();

        /// <summary>
        /// Menampilkan list dari sales order
        /// </summary>
        /// <param name="searchSONumber">Parameter string filter search dengan SO number</param>
        /// <param name="searchPartNumber">Para</param>
        /// <param name="currentFilterSearchSONumber"></param>
        /// <param name="currentFilterSearchPartNumber"></param>
        /// <param name="page"></param>
        /// <returns></returns>
        public ActionResult Index(string searchSONumber, string searchPartNumber, string currentFilterSearchSONumber, string currentFilterSearchPartNumber, int? page)
        {
            Session.Remove("message");
            Session.Remove("TempUnits");
            if (searchSONumber != null || searchPartNumber != null)
            {
                page = 1;
            }
            else
            {
                searchSONumber = currentFilterSearchSONumber;
                searchPartNumber = currentFilterSearchPartNumber;
            }

            ViewBag.CurrentFilterSearchSONumber = searchSONumber;
            ViewBag.CurrentFilterSearchPartNumber = searchPartNumber;

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

        public ActionResult Details(int? id, int? page)
        {
            if (id.HasValue)
            {
                try
                {
                    Session.Remove("TempUnits");
                    SalesOrder salesOrder = db.SalesOrders.Find(id);
                    Products productData = db.Products.Where(x => x.PN == salesOrder.PartNumber).FirstOrDefault();

                    if (salesOrder != null)
                    {
                        List<Unit> mappedUnitList = db.Units.Where(x => x.SalesOrderID == id).ToList();

                        MappingSalesOrderViewModel haveUnits = new MappingSalesOrderViewModel
                        {
                            SalesOrder = salesOrder,
                            Products = productData,
                            MappedUnits = mappedUnitList
                        };
                        ViewBag.Page = page.ToString();
                        return View(haveUnits);
                    }
                    else
                    {
                        ViewBag.ErrorMessage = "Sorry we couldn't find your sales order";
                        return View("Error");
                    }
                }
                catch (Exception ex)
                {
                    ViewBag.ErrorMessage = "An error occured, please check your data";
                    ViewBag.Exception = ex;
                }
                return View("Error");
            }
            else
            {
                return RedirectToAction("Index");
            }
        }

        public ActionResult Edit(int? id, int? page)
        {
            if (id.HasValue)
            {
                try
                {
                    SalesOrder salesOrder = db.SalesOrders.Find(id);
                    Products productData = db.Products.Where(x => x.PN == salesOrder.PartNumber).FirstOrDefault();

                    if (salesOrder != null)
                    {
                        if (productData != null)
                        {
                            List<Unit> listOfUnits = db.Units.Where(x => x.ProductID == productData.ID && !db.Units.Where(y => y.SalesOrderID != null).Any(y => y.ID == x.ID)).OrderBy(x => x.SerialNumber).ToList();
                            List<Unit> mappedUnitList = db.Units.Where(x => x.SalesOrderID == id).ToList();
                            //CurrentMappingSalesOrder currentMappingSalesOrdersTemp = (CurrentMappingSalesOrder)Session["TempUnits"];
                            
                            List<Unit> deletedUnit = new List<Unit>();
                            List<Unit> additionalUnit = new List<Unit>();

                            if (Session["TempUnits"] == null)
                            {

                                CurrentMappingSalesOrder currentMappingSalesOrderTemp = new CurrentMappingSalesOrder
                                {
                                    ActualTempUnits = mappedUnitList
                                };
                                Session["TempUnits"] = currentMappingSalesOrderTemp;
                            }
                           

                            //CurrentMappingSalesOrder sessionMappingSalesOrder = (CurrentMappingSalesOrder)Session["TempUnits"];

                            MappingSalesOrderViewModel haveUnits = new MappingSalesOrderViewModel
                            {
                                SalesOrder = salesOrder,
                                Units = listOfUnits,
                                Products = productData
                            };
                            ViewBag.Page = page.ToString();
                            return View(haveUnits);
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
                    ViewBag.ErrorMessage = "An error occured, please check your data";
                    ViewBag.Exception = ex;
                }
                return View("Error");
            }
            else
            {
                return RedirectToAction("Index");
            }
        }

        public ActionResult AddSerialNumber(int salesOrderID,  MappingSalesOrderViewModel mappingSalesOrderViewModel,  FormCollection collection)
        {
            CurrentMappingSalesOrder temporaryUnits = (CurrentMappingSalesOrder)Session["TempUnits"];
            try
            {
                var userIdentity = User.Identity.Name;
                var page = collection.GetValues("currentPage");
                if(string.IsNullOrWhiteSpace(page[0]))
                {
                    page[0] = "1";
                }

                int currentPage = Convert.ToInt32(page[0]);
                
                List<Unit> additionalTempUnit = new List<Unit>();
                //List<Unit> deletedTempUnit = temporaryUnits.DeletedTempUnits;
                List<Unit> actualTempUnit = temporaryUnits.ActualTempUnits != null ? temporaryUnits.ActualTempUnits : new List<Unit>();//db.Units.Where(x => x.SalesOrderID == salesOrderID).ToList();


                if (temporaryUnits.DeletedTempUnits != null)
                {
                    foreach (Unit selectedDeletedUnit in temporaryUnits.DeletedTempUnits)
                    {
                        Unit deletedUnit = temporaryUnits.ActualTempUnits.Where(x => x.ID == selectedDeletedUnit.ID).FirstOrDefault();
                        temporaryUnits.ActualTempUnits.Remove(deletedUnit);
                    }

                }

                foreach (int unitID in mappingSalesOrderViewModel.selectedUnitID)
                {
                    Unit newUnit = db.Units.Find(unitID);
                    newUnit.SalesOrderID = salesOrderID;
                    actualTempUnit.Add(newUnit);
                    if (!db.Units.Any(x => x.ID == unitID && x.SalesOrderID == salesOrderID))
                    {
                        additionalTempUnit.Add(newUnit);
                    }
                }

                temporaryUnits.ActualTempUnits = actualTempUnit;
                temporaryUnits.AdditionalTempUnits = additionalTempUnit;
                Session["TempUnits"] = temporaryUnits;

                ViewBag.Page = page.ToString();
                return RedirectToAction("Edit", "MappingSalesOrder", new { id = salesOrderID, page = currentPage });


            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "An error occured while adding serial number to this sales order";
                ViewBag.Exception = ex;
            }
            return View("Error");
        }
        
        public ActionResult SaveSerialNumber(int salesOrderID, int? page)
        {
            try
            {
                CurrentMappingSalesOrder temporaryUnits = (CurrentMappingSalesOrder)Session["TempUnits"];
                var userIdentity = User.Identity.Name;
                var currentPage = page;

                if (temporaryUnits.DeletedTempUnits != null)
                {
                    foreach (Unit selectedUnitID in temporaryUnits.DeletedTempUnits)
                    {
                        Unit selectedDeletedUnit = db.Units.Where(x => x.ID == selectedUnitID.ID).FirstOrDefault();
                        selectedDeletedUnit.SalesOrderID = null;
                        selectedDeletedUnit.LastModified = DateTime.Now;
                        selectedDeletedUnit.LastModifiedBy = userIdentity;
                    }

                   
                }
                if (temporaryUnits.AdditionalTempUnits != null)
                {
                    foreach (Unit selectedAdditionalUnit in temporaryUnits.AdditionalTempUnits)
                    {
                        Unit selectedUnit = db.Units.Where(x => x.ID == selectedAdditionalUnit.ID).FirstOrDefault();
                        selectedUnit.SalesOrderID = salesOrderID;
                        selectedUnit.LastModified = DateTime.Now;
                        selectedUnit.LastModifiedBy = userIdentity;
                    }
                    
                }
                db.SaveChanges();
                return RedirectToAction("Details", "MappingSalesOrder", new { id = salesOrderID, page = currentPage });
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "An error occured while saving the changes in serial number";
                ViewBag.Exception = ex;
            }
            return View("Error");
        }

        public ActionResult DeleteSerialNumber(int salesOrderID, int unitID, int? page)
        {
            try
            {
                CurrentMappingSalesOrder temporaryUnits = (CurrentMappingSalesOrder)Session["TempUnits"];
                
                Unit deletedTempUnit = temporaryUnits.ActualTempUnits.Where(x => x.ID == unitID).FirstOrDefault();
                var currentPage = page;
                List<Unit> deletedUnit = temporaryUnits.DeletedTempUnits != null ? temporaryUnits.DeletedTempUnits : new List<Unit>();
                List<Unit> actualTempUnit = temporaryUnits.ActualTempUnits;
                actualTempUnit.Remove(deletedTempUnit);
                if (db.Units.Any(x => x.ID == unitID && x.SalesOrderID == salesOrderID))
                {
                    deletedUnit.Add(deletedTempUnit);
                }
                temporaryUnits.DeletedTempUnits = deletedUnit;
                temporaryUnits.ActualTempUnits = actualTempUnit;
                
                Session["TempUnits"] = temporaryUnits;

                ViewBag.Page = page.ToString();
                return RedirectToAction("Edit", "MappingSalesOrder", new { id = salesOrderID , page = currentPage });

            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "An error occured while deleting serial number from this sales order";
                ViewBag.Exception = ex;
            }
            return View("Error");
        }

    }
}