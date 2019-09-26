using System;
using System.Linq;
using System.Web.Mvc;
using PowerAppsCMS.Models;
using PagedList;
using PowerAppsCMS.CustomAuthentication;
using PowerAppsCMS.Constants;

namespace PowerAppsCMS.Controllers
{
    /// <summary>
    /// HolidayController berfungsi sebagai CRUD holiday
    /// </summary>
    [CustomAuthorize(Roles = RoleNames.Administrator + "," + RoleNames.SuperAdmin)]
    public class HolidayController : Controller
    {
        private PowerAppsCMSEntities db = new PowerAppsCMSEntities();

        /// <summary>
        /// Method Index berfungsi untuk menampilkan list semua holiday
        /// </summary>
        /// <param name="searchName">Parameter searchName digunakan untuk mencari holiday berdasarkan nama yang di input</param>
        /// <param name="currentFilter">Parameter yang digunakan untuk mengatur filter ketika user membuka halaman saat ini atau berikutnya</param>
        /// <param name="page">Parameter nomor halaman</param>
        /// <returns>Menampilkan list semua holiday</returns>
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

            var holidayList = from x in db.Holidays
                              select x;

            if (!String.IsNullOrEmpty(searchName))
            {
                holidayList = holidayList.Where(x => x.Name.Contains(searchName));
            }

            int pageSize = 10;
            int pageNumber = (page ?? 1);

            ViewBag.PageNumber = pageNumber.ToString();
            ViewBag.ItemperPage = pageSize.ToString();
            ViewBag.Page = page.ToString();

            return View(holidayList.OrderBy(x => x.StartDate).ToPagedList(pageNumber, pageSize));
        }

        /// <summary>
        /// Method Create berfungsi untuk menampilkan halaman create
        /// </summary>
        /// <returns>Menampilkan halaman create</returns>
        public ActionResult Create()
        {
            return View();
        }

        /// <summary>
        /// Method Post Create berfungsi untuk menambah data baru di holiday
        /// </summary>
        /// <param name="holiday">Parameter holiday merupakan paramter dari model holiday</param>
        /// <returns>Jika data baru berhasil di input, maka web akan menavigasikan ke halaman index</returns>
        [HttpPost]
        public ActionResult Create(Holiday holiday)
        {
            try
            {
                var username = User.Identity.Name;
                DateTime startDate = holiday.StartDate;
                //Add Holiday into database
                if (holiday.EndDate < holiday.StartDate)
                {
                    ViewBag.ErrorMessage = "End date can't be less than start date";
                    return View();
                }
                else
                {
                    DateTime now = DateTime.Now;
                    holiday.Created = now;
                    holiday.CreatedBy = username;
                    holiday.LastModified = now;
                    holiday.LastModifiedBy = username;

                    holiday = SetHolidayDetailList(holiday);                    
                    db.Holidays.Add(holiday);
                    db.SaveChanges();

                    ViewBag.Message = "Success";
                    return View();
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
        /// Method Edit berfungsi untuk menampilkan halaman edit
        /// </summary>
        /// <param name="id">Parameter id merupakan id dari holiday</param>
        /// <param name="page">Parameter nomor halaman</param>
        /// <returns>Menampilkan halaman edit</returns>
        public ActionResult Edit(int? id, int? page)
        {
            if (id.HasValue)
            {
                var holidayData = db.Holidays.Find(id);
                if(holidayData != null)
                {
                    ViewBag.Page = page.ToString();
                    return View(holidayData);
                }
                else
                {
                    ViewBag.ErrorMessage = "Sorry we couldn't find this holiday";
                    return View("Error");
                }
            }
            else
            {
                return RedirectToAction("Index", "Holiday");
            }
        }

        /// <summary>
        /// Method Post Edit berfungsi untuk mengubah sebuah data di holiday
        /// </summary>
        /// <param name="id">Parameter id merupakan id dari holiday</param>
        /// <param name="holiday">Parameter holiday merupakan paramter dari model holiday</param>
        /// <param name="collection">Parameter collection merupakan parameter yang di gunakan oleh object FormCollection</param>
        /// <returns>Jika data berhasil dirubah, maka web akan menavigasikan ke halaman index</returns>
        [HttpPost]
        public ActionResult Edit(int id, Holiday holiday, FormCollection collection)
        {
            try
            {
                var username = User.Identity.Name;
                var currentPage = collection.GetValues("currentPage");
                ViewBag.CurrentPage = currentPage[0];


               if (holiday.EndDate <= holiday.StartDate && holiday.StartDate != holiday.EndDate)
                {
                    ViewBag.ErrorMessage = "End date can't be less than start date";
                    return View();
                }
                else
                {
                    var holidayData = db.Holidays.Find(id);
                    holidayData.Name = holiday.Name;
                    holidayData.StartDate = holiday.StartDate;
                    holidayData.EndDate = holiday.EndDate;
                    holidayData.LastModified = DateTime.Now;
                    holidayData.LastModifiedBy = username;
                    db.HolidayDetails.RemoveRange(holidayData.HolidayDetails);
                    if(db.SaveChanges() >0)
                    {
                        holidayData = SetHolidayDetailList(holidayData);
                        db.SaveChanges();
                    }

                    ViewBag.Message = "Success";
                    return View();
                }
            }
            catch (Exception ex)
            {
                ViewBag.Exception = ex;
            }
            return View("Error");
        }

        /// <summary>
        /// Method Delete berfungsi untuk menghapus sebuah data dari holiday
        /// </summary>
        /// <param name="id">Parameter id merupakan id dari holiday</param>
        /// <returns>Jika data berhasil di hapus, maka web akan menavigasikan ke halaman index</returns>
        public ActionResult Delete(int id)
        {
            try
            {
                var holidayData = db.Holidays.Find(id);
                db.HolidayDetails.RemoveRange(holidayData.HolidayDetails);
                db.Holidays.Remove(holidayData);
                db.SaveChanges();

                return RedirectToAction("Index", "Holiday");
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "An error occured, please check your data input and try again";
                ViewBag.Exception = ex;
            }
            return View("Error");
        }

        /// <summary>
        /// Merupakan fungsi yang untuk menambahkan Holiday Details dari sebuah data holiday
        /// </summary>
        /// <param name="holiday">sebuah object class holiday yang merupakan data holiday yang akan ditambahkan holiday details nya.</param>
        /// <returns>holiday yang sudah di tambahkan holiday detailsnya</returns>
        public Holiday SetHolidayDetailList(Holiday holiday)
        {
            DateTime startDate = holiday.StartDate;
            var username = User.Identity.Name;
            DateTime now = DateTime.Now;
            do
            {
                HolidayDetail dataDetail = new HolidayDetail();
                dataDetail.HolidayDate = startDate;
                dataDetail.Created = dataDetail.LastModified = now;
                dataDetail.CreatedBy = dataDetail.LastModifiedBy = username;
                holiday.HolidayDetails.Add(dataDetail);
                startDate = startDate.AddDays(1);

            } while (startDate.Date <= holiday.EndDate.Date);

            return holiday;
        }
    }
}
