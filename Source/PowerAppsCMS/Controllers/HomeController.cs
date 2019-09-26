using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PowerAppsCMS.Models;
using PowerAppsCMS.CustomAuthentication;

namespace PowerAppsCMS.Controllers
{
    /// <summary>
    /// HomeController berfungsi menampilkan halaman home. Halaman yang pertama kali muncul ketika mengakses web
    /// </summary>
    public class HomeController : Controller
    {
        private PowerAppsCMSEntities db = new PowerAppsCMSEntities();
        
        /// <summary>
        /// Menampilkan halaman home
        /// </summary>
        /// <returns>Jika user sudah teregistrasi, halaman home akan muncul, jika tidak maka akan muncul pesan error</returns>
        public ActionResult Index()
        {
            //string domain = WebConfigurationManager.AppSettings["ActiveDirectoryUrl"];
            //string ldapUser = WebConfigurationManager.AppSettings["ADUsername"];
            //string ldapPassword = WebConfigurationManager.AppSettings["ADPassword"];           

            //CustomAuthentication.CustomMembership customeRole = new  CustomMembership();

            //bool isValid = customeRole.ValidateUser(ldapUser, ldapPassword);

            //using (DirectoryEntry entry = new DirectoryEntry(domain, ldapUser, ldapPassword))
            //{
            //    DirectorySearcher dSearch = new DirectorySearcher(entry);
            //    //string name = username;
            //    //dSearch.Filter = "(&(objectClass=user)(samaccountname=" + name + "))";
            //    //SearchResultCollection sResultSet = dSearch.FindAll();

            //    //if (sResultSet.Count == 0)
            //    //    isExist = false;
            //}

            //var username = User.Identity.Name;
            //var username = user.Split('\\')[1];       
            
            //if (userData != null)D:\Projects\UTE\Source\PowerAppsCMS\Views\Shared\
            if (Request.IsAuthenticated && HttpContext.User != null)
            {
                var user = HttpContext.User;
                var userData = db.Users.Where(x => x.Username == user.Identity.Name).FirstOrDefault();
                return View(userData);
            }
            else
            {
                //ViewBag.ErrorMessage = "Sorry your account not register yet in our system, please contact the administrator to register your account";
                //return View("Error");
                ModelState.AddModelError("", "You have to Login first.");
                return RedirectToAction("Login", "Account", null);
            }
        }

        public ActionResult Login()
        {
            return View();
        }
    }
}