using PagedList;
using PowerAppsCMS.Constants;
using PowerAppsCMS.CustomAuthentication;
using PowerAppsCMS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PowerAppsCMS.Controllers
{
    [CustomAuthorize(Roles = RoleNames.Administrator + "," + RoleNames.SuperAdmin)]
    public class ComponentSynchronizerController : Controller
    {
        private PowerAppsCMSEntities db = new PowerAppsCMSEntities();
        // GET: ComponentSynchronizer
        public ActionResult Index(int? page)
        {
            var listOfComponentTemp = from x in db.ComponentsTemps
                                      select x;

            int pageSize = 10;
            int pageNumber = (page ?? 1);

            ViewBag.PageNumber = pageNumber.ToString();
            ViewBag.ItemperPage = pageSize.ToString();
            ViewBag.Page = page.ToString();
            return View(listOfComponentTemp.OrderBy(x => x.ProductPartNumber).ToPagedList(pageNumber, pageSize));
        }

        public ActionResult SynchronizeComponent()
        {
            List<ComponentsTemp> listOfComponentTemp = db.ComponentsTemps.ToList();
            var username = User.Identity.Name;
            DateTime now = DateTime.Now;

            try
            {
                foreach (ComponentsTemp item in listOfComponentTemp)
                {
                    var existComponent = db.Components.Where(x => x.PartNumber == item.ComponentPartNumber).FirstOrDefault();
                    if (existComponent != null)
                    {
                        if (item.BS1 > 0)
                        {
                            UpdateMaterialPreparation(item.BS1, 1, existComponent.ID);
                        }

                        if (item.SGC2 > 0)
                        {
                            UpdateMaterialPreparation(item.SGC2, 2, existComponent.ID);
                        }

                        if (item.ET3 > 0)
                        {
                            UpdateMaterialPreparation(item.ET3, 3, existComponent.ID);
                        }

                        if (item.PLM4 > 0)
                        {
                            UpdateMaterialPreparation(item.PLM4, 4, existComponent.ID);
                        }

                        if (item.HGC5 > 0)
                        {
                            UpdateMaterialPreparation(item.HGC5, 5, existComponent.ID);
                        }

                        if (item.AGC6 > 0)
                        {
                            UpdateMaterialPreparation(item.AGC6, 6, existComponent.ID);
                        }

                        if (item.BPB7 > 0)
                        {
                            UpdateMaterialPreparation(item.BPB7, 7, existComponent.ID);
                        }

                        if (item.RB8 > 0)
                        {
                            UpdateMaterialPreparation(item.RB8, 8, existComponent.ID);
                        }

                        if (item.RD9 > 0)
                        {
                            UpdateMaterialPreparation(item.RD9, 9, existComponent.ID);
                        }

                        if (item.TD10 > 0)
                        {
                            UpdateMaterialPreparation(item.TD10, 10, existComponent.ID);
                        }

                        if (item.HB11 > 0)
                        {
                            UpdateMaterialPreparation(item.HB11, 11, existComponent.ID);
                        }

                        if (item.GL12 > 0)
                        {
                            UpdateMaterialPreparation(item.GL12, 12, existComponent.ID);
                        }

                        if (item.GR13 > 0)
                        {
                            UpdateMaterialPreparation(item.GR13, 13, existComponent.ID);
                        }

                        if (item.BVL14 > 0)
                        {
                            UpdateMaterialPreparation(item.BVL14, 14, existComponent.ID);
                        }

                        if (item.LB15 > 0)
                        {
                            UpdateMaterialPreparation(item.LB15, 15, existComponent.ID);
                        }

                        if (item.UML16 > 0)
                        {
                            UpdateMaterialPreparation(item.UML16, 16, existComponent.ID);
                        }

                        ComponentsTemp ct = db.ComponentsTemps.Where(x => x.ID == item.ID).SingleOrDefault();
                        ct.IsInsertToCMPP = true;
                        ct.LastModified = DateTime.Now;
                        ct.LastModifiedBy = username;
                        db.SaveChanges();
                    }
                    else
                    {
                        item.IsComponentExist = false;
                        item.LastModified = now;
                        item.LastModifiedBy = username;
                        db.SaveChanges();
                    }
                }

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ViewBag.Exception = ex;
                ViewBag.ErrorMessage = "An error occured while doing this process";
                return View("Error");
            }
        }

        public void UpdateMaterialPreparation(decimal value, int materialPreparationProcessID, int componentID)
        {
            //ComponentMaterialPreparationProcess currentCMPP = db.ComponentMaterialPreparationProcesses.Where(x => x.MaterialPreparationProcessID == materialPreparationProcessID).SingleOrDefault();

            //if (value > currentCMPP.Value)
            //{
            //    ComponentMaterialPreparationProcess newCMPP = db.ComponentMaterialPreparationProcesses.Find(currentCMPP.ID);
            //    newCMPP.Value = value;
            //    newCMPP.LastModified = DateTime.Now;
            //    newCMPP.LastModifiedBy = User.Identity.Name;
            //    db.SaveChanges();
            //}

            ComponentMaterialPreparationProcess currentCMPP = db.ComponentMaterialPreparationProcesses.Where(x => x.MaterialPreparationProcessID == materialPreparationProcessID && x.ComponentID == componentID).SingleOrDefault();

            if (currentCMPP != null && value > currentCMPP.Value)
            {
                currentCMPP.Value = value;
                currentCMPP.LastModified = DateTime.Now;
                currentCMPP.LastModifiedBy = User.Identity.Name;
                db.SaveChanges();
            }
            else if(currentCMPP == null)
            {
                ComponentMaterialPreparationProcess newCMPP = new ComponentMaterialPreparationProcess();
                newCMPP.ComponentID = componentID;
                newCMPP.MaterialPreparationProcessID = materialPreparationProcessID;
                newCMPP.Value = value;
                newCMPP.LastModified = newCMPP.Created = DateTime.Now;
                newCMPP.LastModifiedBy = newCMPP.CreatedBy= User.Identity.Name;
                db.ComponentMaterialPreparationProcesses.Add(newCMPP);
                db.SaveChanges();
            }

        }

        public ActionResult SynchronizeProductComposition()
        {
            List<ComponentsTemp> listOfComponentTemp = db.ComponentsTemps.ToList();
            var username = User.Identity.Name;
            DateTime now = DateTime.Now;

            try
            {
                foreach (ComponentsTemp item in listOfComponentTemp)
                {
                    Component existComponent = db.Components.Where(x => x.PartNumber == item.ComponentPartNumber).FirstOrDefault();
                    Products existProduct = db.Products.Where(x => x.PN == item.ProductPartNumber).FirstOrDefault();

                    if (existComponent != null && existProduct !=null)
                    {
                        ProductComposition currentPC = db.ProductCompositions.Where(x => x.ComponentID == existComponent.ID && x.ProductID == existProduct.ID).FirstOrDefault();

                        if(currentPC != null && currentPC.Quantity < item.Qty)
                        {
                            currentPC.Quantity = item.Qty;
                            currentPC.LastModified = DateTime.Now;
                            currentPC.LastModifiedBy = username;
                        }
                        else if(currentPC == null)
                        {
                            ProductComposition newPC = new ProductComposition();
                            newPC.ProductID = existProduct.ID;
                            newPC.ComponentID = existComponent.ID;
                            newPC.Quantity = item.Qty;
                            newPC.LastModified = newPC.Created = DateTime.Now;
                            newPC.LastModifiedBy = newPC.CreatedBy = username;
                            db.ProductCompositions.Add(newPC);
                        }
                        if (db.SaveChanges() > 0)
                        {
                            ComponentsTemp ct = db.ComponentsTemps.Where(x => x.ID == item.ID).SingleOrDefault();
                            ct.IsInsertToPCom = true;
                            ct.LastModified = DateTime.Now;
                            ct.LastModifiedBy = username;

                            db.SaveChanges(); 
                        }
                    }
                    else
                    {
                        item.IsComponentExist = existComponent == null?false:true;
                        item.IsProductExist = existProduct == null ? false : true;
                        item.LastModified = now;
                        item.LastModifiedBy = username;
                        db.SaveChanges();
                    }
                }

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ViewBag.Exception = ex;
                ViewBag.ErrorMessage = "An error occured while doing this process";
                return View("Error");
            }
        }
    }
}