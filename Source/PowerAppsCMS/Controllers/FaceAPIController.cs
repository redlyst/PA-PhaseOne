using PowerAppsCMS.Services;
using System;
using System.Configuration;
using System.Threading.Tasks;
using System.Web.Mvc;
using PowerAppsCMS.Models;
using System.Linq;
using System.Collections.Generic;
using PowerAppsCMS.CustomAuthentication;
using PowerAppsCMS.Constants;

namespace PowerAppsCMS.Controllers
{
    /// <summary>
    /// FaceAPIController berfungsi untuk meretrain gambar dari user yang sudah di upload ke blob storage
    /// </summary>

    public class FaceAPIController : Controller
    {
        private PowerAppsCMSEntities db = new PowerAppsCMSEntities();

        /// <summary>
        /// Method Index berfungsi untuk meretrain gambar dari user
        /// </summary>
        /// <param name="id">Parameter id merupakan id dari user</param>
        /// <param name="page">Parameter nomor halaman</param>
        /// <returns>Gambar yang berhasil di train di face api, akan mengembalikan person id user</returns>
        public async Task<ActionResult> Index(Guid id, int? page)
        {
            User userData = db.Users.Find(id);
            var userImageList = db.UserImages.Where(x => x.UserID == id).ToList();
            try
            {
                if (userImageList.Count > 0)
                {
                    await RetrainImageAsync(userData);

                    var currentPage = page;
                    ViewBag.Message = "Success";
                    return RedirectToAction("Details", "User", new { id = userData.ID, page = currentPage });
                }
                else
                {
                    ViewBag.ErrorMessage = "Image could not be train because you don't have any photos, please back to user preview";
                    ViewBag.UserId = Convert.ToString(id);
                    return View("Error");
                }
            }
            catch (Exception ex)
            {
                ViewBag.Exception = ex;
                ViewBag.ErrorMessage = "An error occured, please check your data input and try again";
            }
            return View("Error");


            // Melakukan autentikasi dengan foto wajah yg berbeda dengan yg ditrain model
            //var result = await faceAPIService.AuthenticateFaceAsync("https://scontent.fcgk13-1.fna.fbcdn.net/v/t1.0-9/1483001_10202824543804432_1973921140_n.jpg?_nc_cat=0&oh=82e0e7493517deffaf1f9b67e5c1b37c&oe=5BDA631E", new Guid("b103f537-be7a-4d82-b82c-7cec36319fc7"));

            //// Menghapus foto lama.
            //await faceAPIService.AddPersonPicture("https://scontent.fcgk13-1.fna.fbcdn.net/v/t1.0-9/422714_3392965472691_1257056959_n.jpg?_nc_cat=0&oh=0758f62aa565a955c65e2b72015fa2db&oe=5BE28F44", new Guid("a6aa7fe6-303b-427a-ad7a-09d7f0724ac7"));

        }

        /// <summary>
        /// Berfungsi untuk train gambar semua user
        /// </summary>
        /// <returns>Gambar user di train dan user mendapatkan personid</returns>
        [CustomAuthorize(Roles = RoleNames.Administrator + "," + RoleNames.SuperAdmin)]
        public async Task<ActionResult> RetrailAllUserAsync()
        {
            try
            {
                List<User> userDataList = db.Users.Where(x => x.PersonID == null && x.UserImages.Count > 0).ToList();

                foreach (var user in userDataList)
                {
                    await RetrainImageAsync(user);
                }
                return RedirectToAction("Index", "User");
            }
            catch(Exception ex)
            {
                ViewBag.Exception = ex;
                ViewBag.ErrorMessage = "An error occured, please check your data input and try again";
                return View("Error");
            }
        }

        /// <summary>
        /// Method yang di gunakan untuk train gambar user
        /// </summary>
        /// <param name="userData">Sebuah object dari model user</param>
        /// <returns>user mendapat personid yang baru</returns>
        public async Task RetrainImageAsync(User userData)
        {
            // Instantiasi FaceAPI Service.
            var faceAPIService = new FaceAPIService(ConfigurationManager.AppSettings["FaceAPIKey"], ConfigurationManager.AppSettings["FaceAPIRoot"]);

            // Membuat person baru.
            var personID = await faceAPIService.CreatePersonAsync(Convert.ToString(userData.ID), userData.Name); // Person 01_Sangadji Prabowo sudah ditambahkan sebelumnya untuk keperluan debugging dan PersonIDnya adalah b103f537-be7a-4d82-b82c-7cec36319fc7
            userData.PersonID = Convert.ToString(personID);
            userData.LastModified = DateTime.Now;
            userData.LastModifiedBy = User.Identity.Name;
            db.SaveChanges();

            // Menambahkan foto baru dan melatih model.
            List<string> images = new List<string>();
            images = (from x in db.UserImages
                      where x.UserID == userData.ID
                      select x.BlobImage).ToList();

            await faceAPIService.AddPersonPicture(images, new Guid(userData.PersonID));
        }
    }
}