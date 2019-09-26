using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using PowerAppsCMS.Models;
using PowerAppsCMS.Services;

namespace PowerAppsCMS.Controllers
{
    public class RetrainUserController : Controller
    {
        private PowerAppsCMSEntities db = new PowerAppsCMSEntities();

        // GET: RetrainUser
        public ActionResult Index()
        {
            List<User> userList = db.Users.Where(x => x.PersonID == null && x.UserImages.Count > 0).ToList();
            return View(userList);
        }

        public ActionResult DisplayAllUser()
        {
            List<User> allUserList = db.Users.Where(x => x.UserImages.Count > 0).ToList();
            return View(allUserList);
        }

        public ActionResult ResetPersonID()
        {
            try
            {
                List<User> userList = db.Users.ToList();

                foreach (var user in userList)
                {
                    user.PersonID = null;
                    user.LastModified = DateTime.Now;
                    user.LastModifiedBy = "Reset Person ID";
                    db.SaveChanges();
                }

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "An error occured while doing this process";
                ViewBag.Exception = ex;
            }
            return View("Error");
        }

        public async Task<ActionResult> RetrailAllUserAsync()
        {
            try
            {
                List<User> userDataList = db.Users.Where(x => x.PersonID == null && x.UserImages.Count > 0).ToList();

                foreach (var user in userDataList)
                {
                    var faceAPIService = new FaceAPIService(ConfigurationManager.AppSettings["FaceAPIKey"], ConfigurationManager.AppSettings["FaceAPIRoot"]);
                    try
                    {
                        await faceAPIService.DeletePerson(user.ID);
                    }
                    catch
                    {
                    }
                    await RetrainImageAsync(user);
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

        public async Task RetrainImageAsync(User userData)
        {
            // Instantiasi FaceAPI Service.
            var faceAPIService = new FaceAPIService(ConfigurationManager.AppSettings["FaceAPIKey"], ConfigurationManager.AppSettings["FaceAPIRoot"]);
            try
            {
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
            catch
            {
            }
        }
    }
}