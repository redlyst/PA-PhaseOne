using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PowerAppsCMS.Models;
using PowerAppsCMS.ViewModel;
using PagedList;

namespace PowerAppsCMS.Controllers
{
    public class ProductReferenceController : Controller
    {
        private PowerAppsCMSEntities db = new PowerAppsCMSEntities();

        // GET: ProductReference
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
            var productReferencesList = from x in db.ProductReferences
                                        select x;

            if (!String.IsNullOrEmpty(searchName))
            {
                productReferencesList = productReferencesList.Where(x => x.Name.Contains(searchName));
            }

            int pageSize = 10;
            int pageNumber = (page ?? 1);

            ViewBag.PageNumber = pageNumber.ToString();
            ViewBag.ItemperPage = pageSize.ToString();
            ViewBag.Page = page.ToString();
            return View(productReferencesList.OrderBy(x => x.Name).ToPagedList(pageNumber, pageSize));
        }

        // GET: ProductReference/Details/5
        public ActionResult Details(int? id, int? page)
        {
            if (id.HasValue)
            {
                ProductReferences productReferences = db.ProductReferences.Find(id);
                List<Products> productList = db.Products.Where(x => x.ProductSubGroups.ProductGroupID == productReferences.ProductGroupID && !db.Products.Where(y => y.ProductReferenceID != null).Any(y => y.ID == x.ID)).OrderBy(x => x.Name).ToList();
                if (productReferences != null)
                {
                    ProductReferenceViewModel productReferenceViewModel = new ProductReferenceViewModel
                    {
                        ProductReferences = productReferences,
                        ProductList = productList
                    };
                    ViewBag.Page = page.ToString();
                    return View(productReferenceViewModel);
                }
                else
                {
                    ViewBag.ErrorMessage = "Sorry we could not find the product reference";
                    return View("Error");
                }
            }
            else
            {
                return RedirectToAction("Index", "ProductReference");
            }
        }

        // GET: ProductReference/Create
        public ActionResult Create()
        {
            ViewBag.ProductGroupID = new SelectList(db.ProductGroups, "ID", "Name");
            return View();
        }

        // POST: ProductReference/Create
        [HttpPost]
        public ActionResult Create(ProductReferences productReferences)
        {
            ViewBag.ProductGroupID = new SelectList(db.ProductGroups, "ID", "Name");
            try
            {
                var userIdentity = User.Identity.Name;
                DateTime now = DateTime.Now;

                productReferences.Created = now;
                productReferences.CreatedBy = userIdentity;
                productReferences.LastModified = now;
                productReferences.LastModifiedBy = userIdentity;
                db.ProductReferences.Add(productReferences);
                db.SaveChanges();
                ViewBag.Message = "success";
                ViewBag.ProductReferenceID = productReferences.ID;

                return View();
            }
            catch (Exception ex)
            {
                ViewBag.Exception = ex;
                ViewBag.ErrorMessage = "An error occured, please check your data input and try again";
                return View("Error");
            }
        }

        // GET: ProductReference/Edit/5
        public ActionResult Edit(int? id, int? page)
        {
            if (id.HasValue)
            {
                ProductReferences productReferences = db.ProductReferences.Find(id);

                if (productReferences != null)
                {
                    ViewBag.ProductGroupID = new SelectList(db.ProductGroups, "ID", "Name", productReferences.ProductGroupID);
                    ViewBag.HaveProduct = productReferences.Products.Count() > 0 ? "1" : "0";
                    ViewBag.Page = page.ToString();
                    return View(productReferences);
                }
                else
                {
                    ViewBag.ErrorMessage = "Sorry we could not find the product reference";
                    return View("Error");
                }
            }
            else
            {
                return RedirectToAction("Index", "ProductReference");
            }
        }

        // POST: ProductReference/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, ProductReferences productReferences, FormCollection collection)
        {
            var userIdentity = User.Identity.Name;
            ProductReferences currentProductReference = db.ProductReferences.Find(id);
            var currentPage = collection.GetValues("currentPage");
            ViewBag.ProductGroupID = new SelectList(db.ProductGroups, "ID", "Name", currentProductReference.ProductGroupID);
            ViewBag.ProductReferenceID = currentProductReference.ID.ToString();
            ViewBag.CurrentPage = currentPage[0];
            ViewBag.HaveProduct = currentProductReference.Products.Count() > 0 ? "1" : "0";
            try
            {
                currentProductReference.Name = productReferences.Name;
                currentProductReference.ProductGroupID = currentProductReference.Products.Count == 0 ? productReferences.ProductGroupID : currentProductReference.ProductGroupID;
                currentProductReference.MHPBIH = productReferences.MHPBIH;
                currentProductReference.MHPBOH = productReferences.MHPBOH;
                currentProductReference.MHFabIH = productReferences.MHFabIH;
                currentProductReference.MHFabOH = productReferences.MHFabOH;
                currentProductReference.MHPaintingIH = productReferences.MHPaintingIH;
                currentProductReference.MHPaintingOH = productReferences.MHPaintingOH;
                currentProductReference.Factor = productReferences.Factor;
                currentProductReference.IsOperatorOr = productReferences.IsOperatorOr;
                currentProductReference.LastModified = DateTime.Now;
                currentProductReference.LastModifiedBy = userIdentity;
                db.SaveChanges();

                ViewBag.Message = "Success";
                return View(currentProductReference);
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "An error occured, please check your data input and try again";
                ViewBag.Exception = ex;
                return View("Error");
            }
        }

        // GET: ProductReference/Delete/5
        public ActionResult Delete(int id)
        {
            ProductReferences productReferences = db.ProductReferences.Find(id);
            try
            {
                db.ProductReferences.Remove(productReferences);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "An error occured, please check your data input and try again";
                ViewBag.Exception = ex;
            }
            return View("Error");
        }

        public ActionResult AddProduct(ProductReferenceViewModel productReferenceViewModel, FormCollection collection)
        {
            try
            {
                var userIdentity = User.Identity.Name;
                var productReferenceID = collection.GetValues("ProductReferences.ID");
                var currentPage = collection.GetValues("currentPage");

                foreach (var productID in productReferenceViewModel.SelectedProducts)
                {
                    Products currentProduct = db.Products.Find(productID);
                    currentProduct.ProductReferenceID = Convert.ToInt32(productReferenceID[0]);
                    currentProduct.LastModified = DateTime.Now;
                    currentProduct.LastModifiedBy = userIdentity;
                    db.SaveChanges();
                }
                return new RedirectResult(Url.Action("Details", "ProductReference", new { id = productReferenceID[0], page = currentPage[0] }));
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "An error occured, please check your data input and try again";
                ViewBag.Exception = ex;
            }
            return View("Error");
        }

        public ActionResult UpdateProduct(Products products, FormCollection collection)
        {
            try
            {
                var userIdentity = User.Identity.Name;
                //var productReferenceID = collection.GetValues("ProductReferences.ID");
                List<Products> defaultProductReference = db.Products.Where(x => x.ProductReferenceID == products.ProductReferenceID && x.IsDefaultProductReference == true).ToList();
                Products currentProduct = db.Products.Find(products.ID);

                if (products.IsDefaultProductReference == true)
                {
                    if (defaultProductReference.Count() > 0)
                    {
                        return Json(new { success = false, responseText = currentProduct.Name + " can't be updated, because " + currentProduct.ProductReferences.Name + " already has a default product" }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        currentProduct.IsDefaultProductReference = products.IsDefaultProductReference;
                        currentProduct.LastModified = DateTime.Now;
                        currentProduct.LastModifiedBy = userIdentity;
                        db.SaveChanges();

                        return Json(new { success = true, responseText = currentProduct.Name + " successfully set as default for this " + currentProduct.ProductReferences.Name + " product reference" }, JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {
                    currentProduct.IsDefaultProductReference = products.IsDefaultProductReference;
                    currentProduct.LastModified = DateTime.Now;
                    currentProduct.LastModifiedBy = userIdentity;
                    db.SaveChanges();

                    return Json(new { success = true, responseText = currentProduct.Name + " successfully set to not default for this " + currentProduct.ProductReferences.Name + " product reference" }, JsonRequestBehavior.AllowGet);
                }
                
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "An error occured, please check your data input and try again";
                ViewBag.Exception = ex;
            }
            return View("Error");
        }

        public ActionResult DeleteProduct (int id, int? page)
        {
            try
            {
                var userIdentity = User.Identity.Name;
                
                Products currentProduct = db.Products.Find(id);
                var productReferenceID = currentProduct.ProductReferenceID;
                var currentPage = page.ToString();

                currentProduct.ProductReferenceID = null;
                currentProduct.IsDefaultProductReference = false;
                currentProduct.LastModified = DateTime.Now;
                currentProduct.LastModifiedBy = userIdentity;
                db.SaveChanges();

                return RedirectToAction("Details", "ProductReference", new { id = productReferenceID, page = currentPage });
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
