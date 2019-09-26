using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.Mvc;
using PagedList;
using PowerAppsCMS.Constants;
using PowerAppsCMS.CustomAuthentication;
using PowerAppsCMS.Models;

namespace PowerAppsCMS.Controllers
{
    /// <summary>
    /// controller ini berfungsi untuk mensinkronisasikan PRO 
    /// </summary>
    [CustomAuthorize(Roles = RoleNames.PPC + "," + RoleNames.SuperAdmin)]
    public class PROSynchronizeController : Controller
    {
        private PowerAppsCMSEntities db = new PowerAppsCMSEntities();

        /// <summary>
        /// method ini berfungsi untuk menampilkan halaman yang berisi data-data dari tabel DataZemesFails
        /// </summary>
        /// <param name="page">merupakan parameter bertipe integer yang berisi data halaman saat ini</param>
        /// <param name="proNumber">merupakan parameter bertipe string yang berisi data PRO number</param>
        /// <param name="currentFilter">merupakan parameter bertipe string yang berisi data currentFilter</param>
        /// <returns></returns>
        // GET: PROSynchronize
        public ActionResult Index(int? page, string proNumber, string currentFilter)
        {
            if (proNumber != null)
            {
                page = 1;
            }
            else
            {
                proNumber = currentFilter;
            }

            ViewBag.CurrentFilter = currentFilter;

            var PROFails = from x in db.DataZemesFails
                           select x;

            if (!String.IsNullOrEmpty(proNumber))
            {
                PROFails = PROFails.Where(x => x.ProNo.Contains(proNumber));
            }

            int pageSize = 10;
            int pageNumber = (page ?? 1);

            ViewBag.PageNumber = pageNumber.ToString();
            ViewBag.ItemperPage = pageSize.ToString();
            ViewBag.Page = page.ToString();
            return View(PROFails.OrderBy(x => x.ID).ToPagedList(pageNumber, pageSize));
        }

        // GET: PROSynchronize/Details/5

        /// <summary>
        /// method ini berfungsi untuk menampilkan halaman yang berisi data-data dari tabel DataZemesFails 
        /// yang berhasil disinkronisasikan (data yang berhasil ditambahkan ke tabel PRO)
        /// dan kemudian data pada tabel DataZemesFails dihapus apabaila data tersebut berhasil ditambahkan
        /// </summary>
        /// <param name="page">merupakan parameter bertipe integer yang berisi data halaman saat ini</param>
        /// <returns></returns>
        public ActionResult SynchronizePRO(int? page)
        {
            string userName = User.Identity.Name;
            DateTime now = DateTime.Now;
            List<DataZemesFail> dataZemesFails = db.DataZemesFails.ToList();
            List<Unit> unitDataList = new List<Unit>();

            foreach (DataZemesFail item in dataZemesFails)
            {
                Products dataProduct = db.Products.Where(x => x.PN == item.Material).SingleOrDefault();

                if (dataProduct != null)
                {
                    PRO dataPRO = db.Pros.Where(x => x.Number == item.ProNo).SingleOrDefault();

                    if (dataPRO == null)
                    {
                        PRO pro = new PRO();
                        pro.Number = item.ProNo;
                        pro.ProductID = dataProduct.ID;
                        pro.Quantity = item.Quantity;
                        pro.DueDate = item.DeliveryDate;
                        pro.Created = now;
                        pro.CreatedBy = userName;
                        pro.LastModified = now;
                        pro.LastModifiedBy = userName;

                        Unit unitData = new Unit();
                        unitData.SerialNumber = item.SerialNumber;
                        unitData.ProductID = dataProduct.ID;
                        unitData.DueDate = item.DeliveryDate;
                        unitData.IsHold = false;
                        unitData.Created = unitData.LastModified = now;
                        unitData.CreatedBy = unitData.LastModifiedBy = userName;

                        pro.Units.Add(unitData);

                        db.Pros.Add(pro);

                        if (db.SaveChanges() > 0)
                        {
                            unitDataList.Add(unitData);
                            db.DataZemesFails.Remove(item);
                            db.SaveChanges();                          
                        }
                    }
                    else
                    {
                        Unit existUnitData = new Unit();
                        existUnitData.PROID = dataPRO.ID;
                        existUnitData.ProductID = dataProduct.ID;
                        existUnitData.SerialNumber = item.SerialNumber;
                        existUnitData.DueDate = item.DeliveryDate;
                        existUnitData.IsHold = false;
                        existUnitData.Created = existUnitData.LastModified = now;
                        existUnitData.CreatedBy = existUnitData.LastModifiedBy = userName;

                        db.Units.Add(existUnitData);

                        if (db.SaveChanges() > 0)
                        {
                            unitDataList.Add(existUnitData);
                            db.DataZemesFails.Remove(item);
                            db.SaveChanges();
                        }
                    }
                }
                
            }
            int pageSize = 10;
            int pageNumber = (page ?? 1);
            return View(unitDataList.OrderBy(x => x.PROID).ToPagedList(pageNumber, pageSize));
        }

    }
}
