using System.Web.Mvc;

namespace PowerAppsCMS.Controllers
{
    /// <summary>
    /// ErrorController berfungsi untuk menampilkan halaman error karena tidak punya akses
    /// </summary>
    public class ErrorController : Controller
    {
        /// <summary>
        /// Method Unauthorized berfungsi untuk menampilkan halaman error karena tidak punya akses
        /// </summary>
        /// <returns>Menampilkan halaman error</returns>
        public ActionResult Unauthorized()
        {
            return View();
        }

        public ActionResult AccessDenied()
        {
            return View();
        }
    }
}