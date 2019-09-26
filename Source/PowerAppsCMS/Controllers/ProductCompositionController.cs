using System;
using System.Linq;
using System.Web.Mvc;
using PowerAppsCMS.Models;
using PowerAppsCMS.ViewModel;

namespace PowerAppsCMS.Controllers
{
    /// <summary>
    /// ProductCompositionController berfungsi untuk menambahkan component di dalam product
    /// </summary>
    public class ProductCompositionController : Controller
    {
        private PowerAppsCMSEntities db = new PowerAppsCMSEntities();

        /// <summary>
        /// Method Create berfungsi untuk menambahkan component di dalam product
        /// </summary>
        /// <param name="productId">Parameter productId merupakan id dari product</param>
        /// <param name="productViewModel">Parameter productViewModel merupakan view model dari product</param>
        /// <param name="collection">Parameter collection merupakan parameter yang digunakan oleh object FormCollection</param>
        /// <returns>Component berhasil ditambahkan ke dalam product</returns>
        [HttpPost]
        public ActionResult Create(int productId, ProductViewModel productViewModel, FormCollection collection)
        {
            try
            {
                // TODO: Add product composition
                var username = User.Identity.Name;
                var currentPage = collection.GetValues("currentPage");
                //productViewModel.ProductID = string.Join(",", productViewModel.SelectedProductID);


                foreach (var component in productViewModel.SelectedComponentID)
                {
                    var existingProductComposition = db.ProductCompositions.Where(x => x.ProductID == productId && x.ComponentID == component).SingleOrDefault();

                    if (existingProductComposition != null)
                    {
                        ViewBag.ExistProductComposition = "Component that you are input already exist on this product.";
                        ViewBag.ProductID = productId;
                        return View("ErrorPage");
                    }
                    else
                    {
                        var newProductComposition = new ProductComposition
                        {
                            ProductID = productId,
                            ComponentID = component,
                            Created = DateTime.Now,
                            CreatedBy = username,
                            LastModified = DateTime.Now,
                            LastModifiedBy = username

                        };
                        db.ProductCompositions.Add(newProductComposition);
                        db.SaveChanges();
                    }
                }
                return new RedirectResult(Url.Action("Details", "Product", new { id = productId, tab = "component", page = currentPage[0] }));
            }
            catch (Exception ex)
            {
                ViewBag.Exception = ex;
                return View("ErrorPage");
            }
        }

        /// <summary>
        /// Method Edit berfungsi untuk mengubah quantity di dalam product composition
        /// </summary>
        /// <param name="productComposition">Parameter productComposition merupakan model dari product composition</param>
        /// <returns>Quantity berhasil dirubah</returns>
        public ActionResult Edit(ProductComposition productComposition)
        {
            try
            {
                var productCompositionData = db.ProductCompositions.Where(x => x.ID == productComposition.ID).SingleOrDefault();
                productCompositionData.Quantity = productComposition.Quantity;
                productCompositionData.IsDefaultComponent = productComposition.IsDefaultComponent;
                db.SaveChanges();

                return Json(new { success = true, responseText = "Quantity successfully updated"}, JsonRequestBehavior.AllowGet);
            }
            catch(Exception ex)
            {
                ViewBag.Exception = ex;
            }
            return View();
        }
        

        /// <summary>
        /// Method Delete berfungsi menghapus component dari product
        /// </summary>
        /// <param name="id">Parameter id merupakan id dari product composition</param>
        /// <param name="page">Parameter nomor halaman</param>
        /// <returns>Component berhasil dirubah di product</returns>
        public ActionResult Delete(int id, int? page)
        {
            try
            {
                var productCompositionData = db.ProductCompositions.Find(id);
                var productId = productCompositionData.ProductID;
                var currentPage = page.ToString();
                db.ProductCompositions.Remove(productCompositionData);
                db.SaveChanges();
                
                return new RedirectResult(Url.Action("Details", "Product", new { id = productId, tab = "component", page = currentPage }));
            }
            catch(Exception ex)
            {
                ViewBag.Exception = ex;
            }
            return View();
        }
    }
}
