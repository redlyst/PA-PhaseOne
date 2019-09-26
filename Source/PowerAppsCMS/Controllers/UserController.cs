using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PowerAppsCMS.Models;
using PowerAppsCMS.ViewModel;
using PowerAppsCMS.Services;
using System.Threading.Tasks;
using System.Configuration;
using System.Security.Cryptography;
using System.Text;
using PagedList;
using PowerAppsCMS.CustomAuthentication;
using System.Web.Configuration;
using System.DirectoryServices;
using System.Collections.Generic;
using PowerAppsCMS.Constants;

namespace PowerAppsCMS.Controllers
{
    /// <summary>
    /// UserController berfungsi sebagai CRUD modul user
    /// </summary>
    /// <remarks>
    /// Di dalam UserController juga terdapat fungsi lain seperti DeleteImages, UserPreview, UploadImages, RetrainImage,
    /// EncryptPin, LoadPartialView, IsExistingInAD, GetParent, GetIsAccessPA, CheckUserToAD
    /// </remarks>
    [CustomAuthorize(Roles = RoleNames.Administrator + "," + RoleNames.SuperAdmin)]
    public class UserController : Controller
    {
        private PowerAppsCMSEntities db = new PowerAppsCMSEntities();
        ImageServices imageServices = new ImageServices();

        /// <summary>
        /// Berfungsi untuk menampilkan semua list user
        /// </summary>
        /// <param name="searchName">sebuah string yang berarti nama dari product yang akan di cari memonya</param>
        /// <param name="currentFilter">sebuah string yang berarti sedang di filter di halaman index</param>
        /// <param name="page">sebuah bilangan integer yang boleh kosong dan berfungsi sebagai angka parameter nomor halaman</param>
        /// <returns>mamanggil halaman daftar memo dengan data yang sesuai dengan filter parameter</returns>
        public ActionResult Index(string searchName, string searchEmployeeNumber, string currentFilterSearchName, string currentFilterSearchEmployeeNumber, int? page)
        {
            var username = User.Identity.Name;

            if (searchName != null || searchEmployeeNumber != null)
            {
                page = 1;
            }
            else
            {
                searchName = currentFilterSearchName;
                searchEmployeeNumber = currentFilterSearchEmployeeNumber;
            }

            ViewBag.CurrentFilterSearchName = searchName;
            ViewBag.CurrentFilterSearchEmployeeNumber = searchEmployeeNumber;

            var userList = from x in db.Users
                           where x.IsActive == true
                           select x;

            if (!String.IsNullOrEmpty(searchName) && !String.IsNullOrEmpty(searchEmployeeNumber))
            {
                userList = userList.Where(x => x.Name.Contains(searchName) && x.EmployeeNumber.Contains(searchEmployeeNumber));
            }
            else if (!String.IsNullOrEmpty(searchName))
            {
                userList = userList.Where(x => x.Name.Contains(searchName));
            }
            else if(!String.IsNullOrEmpty(searchEmployeeNumber))
            {
                userList = userList.Where(x => x.EmployeeNumber.Contains(searchEmployeeNumber));
            }

            int pageSize = 10;
            int pageNumber = (page ?? 1);

            ViewBag.PageNumber = pageNumber.ToString();
            ViewBag.ItemperPage = pageSize.ToString();
            ViewBag.Page = page.ToString();
            return View(userList.Where(x => x.RoleID > 0).OrderBy(x => x.Name).ToPagedList(pageNumber, pageSize));
        }

        public ActionResult Index2(string searchName, string currentFilter, int? page)
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

            var userList = from x in db.Users
                           where x.IsActive == true
                           select x;

            if (!String.IsNullOrEmpty(searchName))
            {
                userList = userList.Where(x => x.Name.Contains(searchName));
            }

            int pageSize = 1000;
            int pageNumber = (page ?? 1);

            ViewBag.PageNumber = pageNumber.ToString();
            ViewBag.ItemperPage = pageSize.ToString();
            ViewBag.Page = page.ToString();
            return View(userList.Where(x => x.RoleID > 0).OrderBy(x => x.Name).ToPagedList(pageNumber, pageSize));
        }


        public ActionResult Index3(string searchName, string currentFilter, int? page)
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

            List<tempImage> tempImageList = db.tempImages.ToList();

            int pageSize = 1000;
            int pageNumber = (page ?? 1);

            ViewBag.PageNumber = pageNumber.ToString();
            ViewBag.ItemperPage = pageSize.ToString();
            ViewBag.Page = page.ToString();
            return View(tempImageList.ToPagedList(pageNumber, pageSize));
        }

        /// <summary>
        /// Berfungsi untuk menampilkan detail dari sebuah user
        /// </summary>
        /// <param name="id">Sebuah guid yang merupakan id dari user</param>
        /// <param name="page">sebuah bilangan integer yang boleh kosong dan berfungsi sebagai angka parameter nomor halaman</param>
        /// <returns>Menampilkan detail user</returns>
        public ActionResult Details(Guid? id, int? page)
        {
            if (id.HasValue)
            {
                var userData = db.Users.Find(id);
                if (userData != null)
                {
                    var userImageList = db.UserImages.Where(x => x.UserID == id).ToList();
                    var processGroupData = db.ProcessGroups.Where(x => x.ID == userData.ProcessGroupID).SingleOrDefault();
                    var parentUser = db.Users.Where(x => x.ID == userData.ParentUserID).SingleOrDefault();
                    List<UserProcessGroup> userProcessGroupList = db.UserProcessGroups.Where(x => x.UserID == id).ToList();

                    if (parentUser == null)
                    {
                        ViewBag.ParentUser = "No higher employee for this user";
                    }
                    else
                    {
                        ViewBag.ParentUser = Convert.ToString(parentUser.Name);
                        ViewBag.ParentUserRole = Convert.ToString(parentUser.Role.Name);
                    }

                    var viewModel = new UserViewModel
                    {
                        User = userData,
                        UserImages = userImageList,
                        ProcessGroup = processGroupData,
                        UserProcessGroupList = userProcessGroupList
                    };

                    ViewBag.Page = page.ToString();
                    return View(viewModel);
                }
                else
                {
                    ViewBag.ErrorMessage = "Sorry we couldn't find this user";
                    return View("Error");
                }

            }
            else
            {
                return RedirectToAction("Index", "User");
            }
        }

        /// <summary>
        /// Berfungsi untuk menampilkan halaman create new user
        /// </summary>
        /// <param name="RoleId">Sebuah bilangan integer yang merupakan id dari role</param>
        /// <param name="ProcessGroupId">Sebuah bilangan integer yang merupakan id dari process group</param>
        /// <returns>Menampilkan halaman create new user dengan bebrapa data seperti kumpulan role, user, dan process group</returns>
        public ActionResult Create(int? RoleId, int? ProcessGroupId)
        {
            ViewBag.RoleID = new SelectList(db.Roles, "ID", "Name");
            ViewBag.ParentUserID = new SelectList(db.Users.Where(x => x.IsActive == true).OrderBy(x => x.Name), "ID", "Name");
            ViewBag.ProcessGroupID = new SelectList(db.ProcessGroups.OrderBy(x => x.Name), "ID", "Name");
            return View();
        }

        /// <summary>
        /// Berfungsi untuk menambahkan data baru di user
        /// </summary>
        /// <param name="user">Sebuah object dari model user</param>
        /// <returns>Jika data baru berhasil di input, makan web akan meredirect ke halaman yang sesuai dengan role nya</returns>
        [HttpPost]
        public ActionResult Create(User user, FormCollection collection)
        {
            ViewBag.ParentUserID = new SelectList(db.Users.Where(x => x.IsActive == true).OrderBy(x => x.Name), "ID", "Name");
            ViewBag.ProcessGroupID = new SelectList(db.ProcessGroups.OrderBy(x => x.Name), "ID", "Name");

            var username = !string.IsNullOrEmpty(user.Username) ? user.Username : "-";// !string.IsNullOrEmpty(user.Email) ? user.Email.Split('@')[0] : "-"; // User.Identity.Name;
            var existUsername = !username.Equals("-") ? db.Users.Where(x => x.Username == username).SingleOrDefault() : null; // db.Users.Where(x => x.Username == user.Username).SingleOrDefault();
            var existEmployeeNumber = user.EmployeeNumber != null ? db.Users.Where(x => x.EmployeeNumber == user.EmployeeNumber).SingleOrDefault() : null;
            var encryptedPin = string.IsNullOrEmpty(user.PIN) ? user.PIN : EncryptPin(user.PIN);
            var userIdentity = User.Identity.Name;

            //Check user to AD
            bool isLDAP = Convert.ToBoolean(WebConfigurationManager.AppSettings["IsLDAP"]);
            bool isExist = true;
            if (isLDAP)
                isExist = IsExistingInAD(username);
            else
            {
                if (!string.IsNullOrEmpty(user.Email) && !string.IsNullOrEmpty(encryptedPin))
                {
                    string domain = WebConfigurationManager.AppSettings["ActiveDirectoryUrl"];
                    string ldapUser = WebConfigurationManager.AppSettings["ADUsername"];
                    string ldapPassword = WebConfigurationManager.AppSettings["ADPassword"];
                    using (DirectoryEntry entry = new DirectoryEntry(domain, ldapUser, ldapPassword))
                    {
                        DirectorySearcher dSearch = new DirectorySearcher(entry);
                        string name = username;
                        dSearch.Filter = "(&(objectClass=user)(samaccountname=" + name + "))";
                        SearchResultCollection sResultSet = dSearch.FindAll();

                        if (sResultSet.Count == 0)
                            isExist = false;
                    }
                }
            }

            try
            {
                if (!isExist)
                {
                    ViewBag.ErrorMessage = "AD User not existing";
                    return View();
                }
                else if (existEmployeeNumber != null)
                {
                    ViewBag.ErrorMessage = "Employee number already exist";
                    return View();
                }
                else if (existUsername != null && !username.Equals("-"))
                {
                    ViewBag.ErrorMessage = "Username already exist";
                    return View();
                }
                else if (user.RoleID == 0)
                {
                    ViewBag.ErrorMessage = "Role must be selected";
                }
                else
                {

                    var identity = new CustomPrincipal(username); // new CustomPrincipal(WebConfigurationManager.AppSettings["ADUsername"]);
                    var employeeNumber = collection.GetValues("employeeNumber");
                    user.ID = Guid.NewGuid();
                    user.Username = username;// !string.IsNullOrEmpty(user.Email) ? user.Email.Split('@')[0] : "-";
                    user.Name = user.Name;// !string.IsNullOrEmpty(identity.Name) ? identity.Name : !string.IsNullOrEmpty(user.Email) ? user.Email.Split('@')[0] : user.Name;
                    user.EmployeeNumber = user.EmployeeNumber != null ? user.EmployeeNumber : employeeNumber[0];
                    user.PIN = string.IsNullOrEmpty(encryptedPin) ? "0000000" : encryptedPin;
                    user.RoleID = user.RoleID;
                    user.ProcessGroupID = user.ProcessGroupID;// 1;// FK Tidak bisa null.
                    user.IsActive = true;
                    user.Created = DateTime.Now;
                    user.CreatedBy = userIdentity;
                    user.LastModified = DateTime.Now;
                    user.LastModifiedBy = userIdentity;
                    db.Users.Add(user);
                    db.SaveChanges();

                    var powerappsAccessRole = db.Roles.Where(x => x.ID == user.RoleID).FirstOrDefault();
                    if (powerappsAccessRole.HaveAccessPowerApps == true)
                    {
                        return RedirectToAction("UserPreview", "User", new { id = user.ID });
                    }
                    else
                    {
                        return RedirectToAction("Details", "User", new { id = user.ID });
                    }
                }
            }
            catch (Exception ex)
            {
                ViewBag.Exception = ex;
                ViewBag.ErrorMessage = "An error occured, please check your data input and try again";
            }
            return View("Error");

        }

        public ActionResult CreateUser(int? RoleId, int? ProcessGroupId)
        {
            ViewBag.RoleID = new SelectList(db.Roles.Where(x => x.ID > 0), "ID", "Name");
            ViewBag.ParentUserID = new SelectList(db.Users.Where(x => x.IsActive == true).OrderBy(x => x.Name), "ID", "Name");
            ViewBag.ProcessGroupID = new SelectList(db.ProcessGroups.OrderBy(x => x.Name), "ID", "Name");

            List<ProcessGroup> listOfProcessGroup = db.ProcessGroups.ToList();

            var viewModel = new UserViewModel
            {
                ProcessGroupList = listOfProcessGroup
            };
            return View(viewModel);
        }

        [HttpPost]
        public ActionResult CreateUser(UserViewModel userViewModel)
        {
            ViewBag.ParentUserID = new SelectList(db.Users.Where(x => x.IsActive == true).OrderBy(x => x.Name), "ID", "Name");
            ViewBag.ProcessGroupID = new SelectList(db.ProcessGroups.OrderBy(x => x.Name), "ID", "Name");
            List<ProcessGroup> listOfProcessGroup = db.ProcessGroups.ToList();

            var viewModel = new UserViewModel
            {
                ProcessGroupList = listOfProcessGroup
            };

            var username = !string.IsNullOrEmpty(userViewModel.User.Username) ? userViewModel.User.Username : "-";// !string.IsNullOrEmpty(user.Email) ? user.Email.Split('@')[0] : "-"; // User.Identity.Name;
            var existUsername = !username.Equals("-") ? db.Users.Where(x => x.Username == username).SingleOrDefault() : null; // db.Users.Where(x => x.Username == user.Username).SingleOrDefault();
            var existEmployeeNumber = userViewModel.User.EmployeeNumber != null ? db.Users.Where(x => x.EmployeeNumber == userViewModel.User.EmployeeNumber).SingleOrDefault() : null;
            var encryptedPin = string.IsNullOrEmpty(userViewModel.User.PIN) ? userViewModel.User.PIN : EncryptPin(userViewModel.User.PIN);
            var userIdentity = User.Identity.Name;

            //Check user to AD
            bool isLDAP = Convert.ToBoolean(WebConfigurationManager.AppSettings["IsLDAP"]);
            bool isExist = true;
            if (isLDAP)
                isExist = IsExistingInAD(username);
            else
            {
                if (!string.IsNullOrEmpty(userViewModel.User.Email) && !string.IsNullOrEmpty(encryptedPin))
                {
                    string domain = WebConfigurationManager.AppSettings["ActiveDirectoryUrl"];
                    string ldapUser = WebConfigurationManager.AppSettings["ADUsername"];
                    string ldapPassword = WebConfigurationManager.AppSettings["ADPassword"];
                    using (DirectoryEntry entry = new DirectoryEntry(domain, ldapUser, ldapPassword))
                    {
                        DirectorySearcher dSearch = new DirectorySearcher(entry);
                        string name = username;
                        dSearch.Filter = "(&(objectClass=user)(samaccountname=" + name + "))";
                        SearchResultCollection sResultSet = dSearch.FindAll();

                        if (sResultSet.Count == 0)
                            isExist = false;
                    }
                }
            }

            try
            {
                if (!isExist)
                {
                    ViewBag.ErrorMessage = "AD User not existing";
                    return View(viewModel);
                }
                else if (existEmployeeNumber != null)
                {
                    ViewBag.ErrorMessage = "Employee number already exist";
                    return View(viewModel);
                }
                else if (existUsername != null && !username.Equals("-"))
                {
                    ViewBag.ErrorMessage = "Username already exist";
                    return View(viewModel);
                }
                else if (userViewModel.RoleID == 0)
                {
                    ViewBag.ErrorMessage = "Role must be selected";
                }
                else
                {
                    var newUser = new User
                    {
                        ID = Guid.NewGuid(),
                        Username = userViewModel.User.Username,
                        Name = userViewModel.User.Name,
                        EmployeeNumber = userViewModel.User.EmployeeNumber,
                        Email = userViewModel.User.Email,
                        PIN = string.IsNullOrEmpty(encryptedPin) ? "0000000" : encryptedPin,
                        RoleID = userViewModel.RoleID,
                        ProcessGroupID = userViewModel.ProcessGroupID,
                        ParentUserID = userViewModel.ParentUserID,
                        IsAssign = false,
                        IsActive = true,
                        Created = DateTime.Now,
                        CreatedBy = userIdentity,
                        LastModified = DateTime.Now,
                        LastModifiedBy = userIdentity
                    };
                    db.Users.Add(newUser);
                    db.SaveChanges();

                    if (userViewModel.SelectedProcessGroup != null)
                    {
                        foreach(int processGroupID in userViewModel.SelectedProcessGroup)
                        {
                            UserProcessGroup newUserProcessGroup = new UserProcessGroup
                            {
                                UserID = newUser.ID,
                                ProcessGroupID = processGroupID,
                                Created = DateTime.Now,
                                CreatedBy = userIdentity,
                                LastModified = DateTime.Now,
                                LastModifiedBy = userIdentity
                            };
                            db.UserProcessGroups.Add(newUserProcessGroup);
                        }
                        db.SaveChanges();
                    }
                    else if (userViewModel.ProcessGroupID != null)
                    {
                        var userID = newUser.ID;
                        var processGroup = (int)userViewModel.ProcessGroupID;
                        UserProcessGroup newUserProcessGroup = new UserProcessGroup
                        {
                            UserID = newUser.ID,
                            ProcessGroupID = (int)userViewModel.ProcessGroupID,
                            Created = DateTime.Now,
                            CreatedBy = userIdentity,
                            LastModified = DateTime.Now,
                            LastModifiedBy = userIdentity
                        };
                        db.UserProcessGroups.Add(newUserProcessGroup);
                        db.SaveChanges();
                    }

                    var powerappsAccessRole = db.Roles.Where(x => x.ID == newUser.RoleID).FirstOrDefault();
                    if (powerappsAccessRole.HaveAccessPowerApps == true)
                    {
                        return RedirectToAction("UserPreview", "User", new { id = newUser.ID });
                    }
                    else
                    {
                        return RedirectToAction("Details", "User", new { id = newUser.ID });
                    }
                }
            }
            catch (Exception ex)
            {
                ViewBag.Exception = ex;
                ViewBag.ErrorMessage = "An error occured, please check your data input and try again";
            }
            return View("Error");
        }

        /// <summary>
        /// Berfungsi untuk menampilkan halaman edit user
        /// </summary>
        /// <param name="id">Sebuah guid yang merupakan id dari user</param>
        /// <param name="page">sebuah bilangan integer yang boleh kosong dan berfungsi sebagai angka parameter nomor halaman</param>
        /// <returns>Menampilkan halaman edit user</returns>
        public ActionResult Edit(Guid? id, int? page)
        {
            if (id.HasValue)
            {
                var userData = db.Users.Find(id);
                if (userData != null)
                {
                    var processGroupData = db.ProcessGroups.Where(x => x.ID == userData.ProcessGroupID).SingleOrDefault();
                    //ViewBag.RoleID = new SelectList(db.Roles.Where(a => a.HaveAccessPowerApps == true), "ID", "Name");
                    //  ViewBag.RoleID = new SelectList(db.Roles.Where(a => a.HaveAccessPowerApps == userData.Role.HaveAccessPowerApps && a.ID > 0).OrderBy(x => x.Name), "ID", "Name", Convert.ToString(userData.RoleID));
                    var roleData = db.Roles.Where(x => x.ID == userData.RoleID).FirstOrDefault();
                    if (roleData.HaveAccessPowerApps == true)
                    {
                        ViewBag.RoleID = new SelectList(db.Roles.Where(x => x.ID > 0 && x.HaveAccessPowerApps == true).OrderBy(x => x.Name), "ID", "Name", userData.RoleID);
                    }
                    else
                    {
                        ViewBag.RoleID = new SelectList(db.Roles.Where(x => x.ID > 0).OrderBy(x => x.Name), "ID", "Name", userData.RoleID);
                    }

                    List<SelectListItem> itemParentList = new List<SelectListItem>();
                    foreach (var parent in db.Users.Where(x => x.IsActive == true && x.RoleID == roleData.ParentID))
                    {
                        SelectListItem item = new SelectListItem();
                        item.Value = parent.ID.ToString();
                        item.Text = parent.Name;
                        itemParentList.Add(item);
                    }
                    itemParentList.Insert(0, new SelectListItem { Text = "Select Parent User", Value = null });
                    ViewBag.ParentUserID = new SelectList(itemParentList, "Value", "Text", Convert.ToString(userData.ParentUserID));

                    List<SelectListItem> itemGroupList = new List<SelectListItem>();
                    foreach (var group in db.ProcessGroups)
                    {
                        SelectListItem item = new SelectListItem();
                        item.Value = group.ID.ToString();
                        item.Text = group.Name;
                        itemGroupList.Add(item);
                    }
                    itemGroupList.Insert(0, new SelectListItem { Text = "Select Process Group", Value = null });
                    ViewBag.ProcessGroupID = new SelectList(itemGroupList, "Value", "Text", userData.ProcessGroupID);

                    ViewBag.Page = page.ToString();
                    return View(userData);
                }
                else
                {
                    ViewBag.ErrorMessage = "Sorry we couldn't find this user";
                    return View("Error");
                }

            }
            else
            {
                return RedirectToAction("Index", "Home");
            }

        }

        /// <summary>
        /// Berfungsi untuk mengubah data user yang dipilih
        /// </summary>
        /// <param name="collection">Object dari FormCollection yang isinya data dari input form halaman view</param>
        /// <param name="user">Object dari model user</param>
        /// <returns>Jika data berhasil dirubah, makan web akan meredirect ke halaman detail</returns>
        [HttpPost]
        public ActionResult Edit(FormCollection collection, User user)
        {
            string[] id = collection.GetValues("ID");
            string[] page = collection.GetValues("currentPage");
            Guid userId = new Guid(id[0]);
            var username = User.Identity.Name;
            var userData = db.Users.Find(userId);
            var currentPage = page[0];

            ViewBag.RoleID = new SelectList(db.Roles.OrderBy(x => x.Name), "ID", "Name", userData.RoleID);
            ViewBag.ParentUserID = new SelectList(db.Users.Where(x => x.IsActive == true).OrderBy(x => x.Name), "ID", "Name", Convert.ToString(userData.ParentUserID));
            ViewBag.ProcessGroupID = new SelectList(db.ProcessGroups.OrderBy(x => x.Name), "ID", "Name", Convert.ToString(userData.ProcessGroupID));

            try
            {
                var encryptedPin = string.IsNullOrEmpty(user.PIN) ? userData.PIN : EncryptPin(user.PIN);

                userData.Name = user.Name;
                userData.RoleID = user.RoleID;
                userData.ProcessGroupID = user.ProcessGroupID;
                userData.ParentUserID = user.ParentUserID;
                userData.PIN = encryptedPin;
                userData.LastModified = DateTime.Now;
                userData.LastModifiedBy = username;

                db.SaveChanges();
                return RedirectToAction("Details", "User", new { id = userId, page = currentPage });

            }
            catch (Exception ex)
            {
                ViewBag.Exception = ex;
                ViewBag.ErrorMessage = "An error occured, please check your data input and try again";
            }
            return View("Error");
        }

        public ActionResult EditUser(Guid? id, int? page)
        {
            if (id.HasValue)
            {
                User  userData = db.Users.Find(id);
                if (userData != null)
                {
                    ProcessGroup processGroupData = db.ProcessGroups.Where(x => x.ID == userData.ProcessGroupID).SingleOrDefault();
                    Role roleData = db.Roles.Where(x => x.ID == userData.RoleID).FirstOrDefault();
                    List<ProcessGroup> listOfProcessGroup = db.ProcessGroups.ToList();
                    List<UserImage> listOfUserImage = db.UserImages.Where(x => x.UserID == id).ToList();
                    List<UserProcessGroup> listOfUserProcessGroup = db.UserProcessGroups.Where(x => x.UserID == id).ToList();

                    var viewModel = new UserViewModel
                    {
                        User = userData,
                        ProcessGroupList = listOfProcessGroup,
                        UserImages = listOfUserImage,
                        UserProcessGroupList = listOfUserProcessGroup
                    };

                    if (roleData.HaveAccessPowerApps == true)
                    {
                        ViewBag.RoleID = new SelectList(db.Roles.Where(x => x.ID > 0 && x.HaveAccessPowerApps == true).OrderBy(x => x.Name), "ID", "Name", userData.RoleID);
                    }
                    else
                    {
                        ViewBag.RoleID = new SelectList(db.Roles.Where(x => x.ID > 0).OrderBy(x => x.Name), "ID", "Name", userData.RoleID);
                    }

                    List<SelectListItem> itemParentList = new List<SelectListItem>();
                    foreach (var parent in db.Users.Where(x => x.IsActive == true && x.RoleID == roleData.ParentID))
                    {
                        SelectListItem item = new SelectListItem();
                        item.Value = parent.ID.ToString();
                        item.Text = parent.Name;
                        itemParentList.Add(item);
                    }
                    itemParentList.Insert(0, new SelectListItem { Text = "Select Parent User", Value = null });
                    ViewBag.ParentUserID = new SelectList(itemParentList, "Value", "Text", Convert.ToString(userData.ParentUserID));

                    List<SelectListItem> itemGroupList = new List<SelectListItem>();
                    foreach (var group in db.ProcessGroups)
                    {
                        SelectListItem item = new SelectListItem();
                        item.Value = group.ID.ToString();
                        item.Text = group.Name;
                        itemGroupList.Add(item);
                    }
                    itemGroupList.Insert(0, new SelectListItem { Text = "Select Process Group", Value = null });
                    List<UserProcessGroup> userProcessGroupList = db.UserProcessGroups.Where(x => x.UserID == userData.ID).ToList();
                    if (userProcessGroupList.Count() > 1)
                    {
                        ViewBag.ProcessGroupID = new SelectList(itemGroupList, "Value", "Text");
                    }
                    else if (userProcessGroupList.Count() > 0)
                    {
                        UserProcessGroup userProcessGroupData = db.UserProcessGroups.Where(x => x.UserID == userData.ID).First();
                        ViewBag.ProcessGroupID = new SelectList(itemGroupList, "Value", "Text", userProcessGroupData.ProcessGroupID);
                    }
                    else
                    {
                        ViewBag.ProcessGroupID = new SelectList(itemGroupList, "Value", "Text");
                    }
                    

                    ViewBag.Page = page.ToString();
                    return View(viewModel);
                }
                else
                {
                    ViewBag.ErrorMessage = "Sorry we couldn't find this user";
                    return View("Error");
                }

            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        [HttpPost]
        public ActionResult EditUser(UserViewModel userViewModel, FormCollection collection)
        {
            string[] id = collection.GetValues("User.ID");
            string[] page = collection.GetValues("currentPage");
            Guid userId = new Guid(id[0]);
            var userIdentity = User.Identity.Name;
            User userData = db.Users.Find(userId);
            var currentPage = page[0];

            ViewBag.RoleID = new SelectList(db.Roles.OrderBy(x => x.Name), "ID", "Name", userData.RoleID);
            ViewBag.ParentUserID = new SelectList(db.Users.Where(x => x.IsActive == true).OrderBy(x => x.Name), "ID", "Name", Convert.ToString(userData.ParentUserID));            

            List<UserProcessGroup> userProcessGroupList = db.UserProcessGroups.Where(x => x.UserID == userData.ID).ToList();
            if (userProcessGroupList.Count() > 1)
            {
                ViewBag.ProcessGroupID = new SelectList(db.ProcessGroups.OrderBy(x => x.Name), "ID", "Name");
            }
            else if (userProcessGroupList.Count() > 0)
            {
                UserProcessGroup userProcessGroupData = db.UserProcessGroups.Where(x => x.UserID == userData.ID).First();
                ViewBag.ProcessGroupID = new SelectList(db.ProcessGroups.OrderBy(x => x.Name), "ID", "Name", Convert.ToString(userProcessGroupData.ProcessGroupID));
            }
            else
            {
                ViewBag.ProcessGroupID = new SelectList(db.ProcessGroups.OrderBy(x => x.Name), "ID", "Name");
            }

            try
            {
                var encryptedPin = string.IsNullOrEmpty(userViewModel.User.PIN) ? userData.PIN : EncryptPin(userViewModel.User.PIN);
                List<UserProcessGroup> userProcessGroupsList = db.UserProcessGroups.Where(x => x.UserID == userId).ToList();
                db.UserProcessGroups.RemoveRange(userProcessGroupsList);

                userData.Name = userViewModel.User.Name;
                userData.RoleID = userViewModel.RoleID;
                userData.ProcessGroupID = null;
                userData.ParentUserID = userViewModel.ParentUserID;
                userData.PIN = encryptedPin;
                userData.LastModified = DateTime.Now;
                userData.LastModifiedBy = userIdentity;

                if (db.SaveChanges() > 0)
                {
                    if ((userViewModel.SelectedProcessGroup != null && userViewModel.ProcessGroupID == null))
                    {
                        userData.ParentUserID = null;
                        foreach (int processGroupID in userViewModel.SelectedProcessGroup)
                        {
                            UserProcessGroup newUserProcessGroup = new UserProcessGroup
                            {
                                UserID = userData.ID,
                                ProcessGroupID = processGroupID,
                                Created = DateTime.Now,
                                CreatedBy = userIdentity,
                                LastModified = DateTime.Now,
                                LastModifiedBy = userIdentity
                            };
                            db.UserProcessGroups.Add(newUserProcessGroup);
                        }
                        db.SaveChanges();
                    }
                    else if (userViewModel.ProcessGroupID != null)
                    {
                        var userID = userData.ID;
                        var processGroup = (int)userViewModel.ProcessGroupID;
                        UserProcessGroup newUserProcessGroup = new UserProcessGroup
                        {
                            UserID = userData.ID,
                            ProcessGroupID = (int)userViewModel.ProcessGroupID,
                            Created = DateTime.Now,
                            CreatedBy = userIdentity,
                            LastModified = DateTime.Now,
                            LastModifiedBy = userIdentity
                        };
                        db.UserProcessGroups.Add(newUserProcessGroup);
                        db.SaveChanges();
                    }
                }
                
                return RedirectToAction("Details", "User", new { id = userData.ID });
            }
            catch (Exception ex)
            {
                ViewBag.Exception = ex;
                ViewBag.ErrorMessage = "An error occured, please check your data input and try again";
            }
            return View("Error");
        }

        /// <summary>
        /// Berfungsi untuk menghapus data user yang dipilih
        /// </summary>
        /// <param name="id">Sebuah guid yang merupakan id dari user</param>
        /// <returns>Jika berhasil di delete, makan akan meredirect ke halaman index</returns>
        public async Task<ActionResult> Delete(Guid id)
        {
            try
            {
                User userData = db.Users.Find(id);
                List<User> parentUserList = db.Users.Where(x => x.ParentUserID == id).ToList();
                if (userData.ProcessAssigns.Where(x => x.Status != 0 && x.Status != 4 && x.Status != 5).Count() > 0)
                {
                    ViewBag.ErrorMessage = userData.Name + " " + "can't be deleted, because this user still assigned on ongoing process";
                    return View("Error");
                }
                else if (parentUserList.Count() > 0)
                {
                    ViewBag.ErrorMessage = userData.Name + " " + "can't be deleted, because this user is parent user for another user";
                    return View("Error");
                }
                else if (userData.ProcessAssigns.Where(x => x.Status == 0 || x.Status == 4 || x.Status == 5).Count() > 0)
                {
                    if (userData.PersonID != null)
                    {
                        //delete person data from face api identity
                        var faceAPIService = new FaceAPIService(ConfigurationManager.AppSettings["FaceAPIKey"], ConfigurationManager.AppSettings["FaceAPIRoot"]);
                        await faceAPIService.DeletePerson(id);
                        userData.IsActive = false;
                        userData.IsAssign = false;
                        userData.PersonID = null;
                        userData.ParentUserID = null;

                    }
                    else
                    {
                        userData.IsActive = false;
                        userData.IsAssign = false;
                        userData.ParentUserID = null;
                    }
                    db.SaveChanges();
                    return RedirectToAction("Index", "User");
                }
                else
                {
                    if (userData.PersonID != null)
                    {
                        var faceAPIService = new FaceAPIService(ConfigurationManager.AppSettings["FaceAPIKey"], ConfigurationManager.AppSettings["FaceAPIRoot"]);
                        await faceAPIService.DeletePerson(id);
                        db.Users.Remove(userData);
                    }
                    else
                    {
                        db.Users.Remove(userData);
                    }
                    db.SaveChanges();
                    return RedirectToAction("Index", "User");
                }
            }
            catch (Exception ex)
            {
                ViewBag.Exception = ex;
                ViewBag.ErrorMessage = "An error occured, please check your data input and try again";
            }
            return View("Error");
        }

        public async Task<ActionResult> DeleteUser(Guid id)
        {
            try
            {
                User userData = db.Users.Find(id);
                List<User> listOfParentUser = db.Users.Where(x => x.ParentUserID == id).ToList();
                List<UserProcessGroup> listOfUserProcessGroup = db.UserProcessGroups.Where(x => x.UserID == id).ToList();

                if (userData.ProcessAssigns.Where(x => x.Status != 0 && x.Status != 4 && x.Status != 5).Count() > 0)
                {
                    ViewBag.ErrorMessage = userData.Name + " " + "can't be deleted, because this user still assigned on ongoing process";
                    return View("Error");
                }
                else if (listOfParentUser.Count() > 0)
                {
                    ViewBag.ErrorMessage = userData.Name + " " + "can't be deleted, because this user is parent user for another user";
                    return View("Error");
                }
                else if (userData.ProcessAssigns.Where(x => x.Status == 0 || x.Status == 4 || x.Status == 5).Count() > 0)
                {
                    if (userData.PersonID != null)
                    {
                        //delete person data from face api identity
                        var faceAPIService = new FaceAPIService(ConfigurationManager.AppSettings["FaceAPIKey"], ConfigurationManager.AppSettings["FaceAPIRoot"]);
                        try
                        {
                            await faceAPIService.DeletePerson(id);
                        }
                        catch 
                        {
                        }
                        userData.IsActive = false;
                        userData.IsAssign = false;
                        userData.PersonID = null;
                        userData.ParentUserID = null;

                    }
                    else
                    {
                        userData.IsActive = false;
                        userData.IsAssign = false;
                        userData.ParentUserID = null;
                    }
                    db.SaveChanges();
                    return RedirectToAction("Index", "User");
                }
                else
                {
                    if (userData.PersonID != null)
                    {
                        var faceAPIService = new FaceAPIService(ConfigurationManager.AppSettings["FaceAPIKey"], ConfigurationManager.AppSettings["FaceAPIRoot"]);
                        try
                        {
                            await faceAPIService.DeletePerson(id);
                        }
                        catch 
                        {
                        }
                        db.UserImages.RemoveRange(userData.UserImages);
                        db.UserProcessGroups.RemoveRange(listOfUserProcessGroup);
                        db.Users.Remove(userData);
                    }
                    else
                    {
                        db.UserProcessGroups.RemoveRange(listOfUserProcessGroup);
                        db.Users.Remove(userData);
                    }
                    db.SaveChanges();
                    return RedirectToAction("Index", "User");
                }


            }
            catch (Exception ex)
            {
                ViewBag.Exception = ex;
                ViewBag.ErrorMessage = "An error occured, please check your data input and try again";
            }
            return View("Error");
        }

        /// <summary>
        /// Berfungsi untuk menghapus gambar di halaman edit
        /// </summary>
        /// <param name="id">Sebuah integer yang merupakan id dari user images</param>
        /// <param name="page">Sebuah bilangan integer yang boleh kosong dan berfungsi sebagai angka parameter nomor halaman</param>
        /// <returns>Gambar berhasil di delete dari database dan blob storage</returns>
        public ActionResult DeleteImage(int id, int? page)
        {
            var userImageData = db.UserImages.Find(id);
            var userID = userImageData.UserID;
            try
            {
                DeleteImages(userImageData);
                var currentPage = page.ToString();
                return RedirectToAction("Edit", "User", new { id = userID, page = currentPage });
            }
            catch (Exception ex)
            {
                ViewBag.Exception = ex;
                ViewBag.ErrorMessage = "An error occured, please check your data input and try again";
            }
            return View("Error");
        }

        /// <summary>
        /// Berfungsi untuk menghapus gambar di halaman user preview
        /// </summary>
        /// <param name="id">Sebuah integer yang merupakan id dari user image</param>
        /// <returns>Gambar berhasil di delete dari database dan blob storage</returns>
        public ActionResult DeletePreviewImage(int id)
        {
            var userImageData = db.UserImages.Find(id);
            var userID = userImageData.UserID;
            try
            {
                DeleteImages(userImageData);
                return RedirectToAction("UserPreview", "User", new { id = userID });
            }
            catch (Exception ex)
            {
                ViewBag.Exception = ex;
                ViewBag.ErrorMessage = "An error occured, please check your data input and try again";
            }
            return View("Error");
        }

        /// <summary>
        /// Berfungsi untuk menampilkan halaman user preview yang berfungsi untuk menambahkan gambar user dan 
        /// training gambar tersebut sehingga bisa login menggunakan face api
        /// </summary>
        /// <param name="id">Sebuah guid yang merupakan id dari user</param>
        /// <returns>Jika gambar berhasil ditambahkan dan berhasil di train, makan web akan meredirect ke halaman index</returns>
        public ActionResult UserPreview(Guid? id)
        {
            if (id.HasValue)
            {
                var userData = db.Users.Find(id);

                if (userData != null)
                {
                    try
                    {
                        var userImageList = db.UserImages.Where(x => x.UserID == id).ToList();
                        var processGroupData = db.ProcessGroups.Where(x => x.ID == userData.ProcessGroupID).SingleOrDefault();
                        var parentUser = db.Users.Where(x => x.ID == userData.ParentUserID).SingleOrDefault();
                        ViewBag.UserID = id;
                        if (parentUser == null)
                        {
                            ViewBag.ParentUser = "No higher employee for this user";
                        }
                        else
                        {
                            ViewBag.ParentUser = Convert.ToString(parentUser.Name);
                            ViewBag.ParentUserRole = Convert.ToString(parentUser.Role.Name);
                        }

                        if (userImageList.Count >= 3)
                        {
                            ViewBag.UserImages = "Images enough";
                        }

                        var viewModel = new UserViewModel
                        {
                            User = userData,
                            UserImages = userImageList,
                            ProcessGroup = processGroupData

                        };

                        return View(viewModel);
                    }
                    catch (Exception ex)
                    {
                        ViewBag.Exception = ex;
                        ViewBag.ErrorMessage = "An error occured, please check your data input and try again";
                    }
                    return View("UserPreviewError");
                }
                else
                {
                    ViewBag.ErrorMessage = "Sorry we couldn't find this user";
                    return View("UserPreviewError");
                }
            }
            else
            {
                return RedirectToAction("Index", "User");
            }
        }

        /// <summary>
        /// Berfungsi untuk mengupload gambar di halaman user preview
        /// </summary>
        /// <param name="collection">Object dari FormCollection yang isinya data dari input form halaman view</param>
        /// <param name="photos">Sebuah array yang berisi kumpulan beberapa gambar yang di upload</param>
        /// <returns>Gambar berhasil ditambahkan ke blob storage dan database</returns>
        [HttpPost]
        public async Task<ActionResult> UploadPhotos(FormCollection collection, HttpPostedFileBase[] photos)
        {
            try
            {
                string[] pegawaiId = collection.GetValues("pegawaiID");
                await UploadImages(pegawaiId[0], photos);
                return RedirectToAction("UserPreview", "User", new { id = pegawaiId[0] });
            }
            catch (Exception ex)
            {
                ViewBag.Exception = ex;
                ViewBag.ErrorMessage = "An error occured, please check your data input and try again";
            }
            return View("Error");
        }

        public async Task<ActionResult> DeleteAllImage()
        {
            try
            {
                List<tempImage> tempImageList = db.tempImages.ToList();

                foreach (tempImage item in tempImageList)
                {
                    imageServices.DeleteImage(item.ImageDataUrl);
                }
                return RedirectToAction("Index3", "User");
            }
            catch (Exception ex)
            {
                ViewBag.Exception = ex;
                ViewBag.ErrorMessage = "An error occured, please check your data input and try again";
                return View("Error");
            }
        }
        /// <summary>
        /// Berfungsi untuk mengupload gambar di halaman edit
        /// </summary>
        /// <param name="collection">Object dari FormCollection yang isinya data dari input form halaman view</param>
        /// <param name="photos">Sebuah array yang berisi kumpulan beberapa gambar yang di upload</param>
        /// <returns>Gambar berhasil ditambahkan ke blob storage dan database</returns>
        public async Task<ActionResult> UpdatePhoto(FormCollection collection, HttpPostedFileBase[] photos)
        {
            try
            {
                string[] pegawaiId = collection.GetValues("pegawaiID");
                string[] page = collection.GetValues("currentPage");
                var currentPage = page[0];
                await UploadImages(pegawaiId[0], photos);
                return RedirectToAction("Edit", "User", new { id = pegawaiId[0], page = currentPage });
            }
            catch (Exception ex)
            {
                ViewBag.Exception = ex;
                ViewBag.ErrorMessage = "An error occured, please check your data input and try again";
            }
            return View("Error");
        }

        /// <summary>
        /// Berfungsi untuk train ulang gambar user
        /// </summary>
        /// <param name="id">Sebuah guid yang merupakan id dari user</param>
        /// <param name="page">Sebuah bilangan integer yang boleh kosong dan berfungsi sebagai angka parameter nomor halaman</param>
        /// <returns>Person id yang lama akan di hapus terlebih dahulu, dan fungsi akan meredirect ke controller FaceAPI untuk 
        /// di train ulang dan mendapatkan person id yang baru</returns>
        public async Task<ActionResult> RetrainImage(Guid id, int? page)
        {
            try
            {
                var currentPage = page;
                var user = db.Users.Find(id);
                if (user.PersonID != null)
                {
                    var faceAPIService = new FaceAPIService(ConfigurationManager.AppSettings["FaceAPIKey"], ConfigurationManager.AppSettings["FaceAPIRoot"]);
                    await faceAPIService.DeletePerson(id);

                    return RedirectToAction("Index", "FaceAPI", new { id = user.ID, page = currentPage });
                }
                else
                {
                    return RedirectToAction("Index", "FaceAPI", new { id = user.ID, page = currentPage });
                }

            }
            catch (Exception ex)
            {
                ViewBag.Exception = ex;
                ViewBag.ErrorMessage = "An error occured, please check your data input and try again";
            }
            return View("Error");
        }

        /// <summary>
        /// Berfungsi untuk mengenkripsi pin
        /// </summary>
        /// <param name="pin">Sebuah string yang isinya merupakan pin user</param>
        /// <returns></returns>
        public string EncryptPin(string pin)
        {
            MD5 md5 = MD5.Create();
            byte[] hashPin = md5.ComputeHash(Encoding.Default.GetBytes(pin));
            StringBuilder returnHashedPin = new StringBuilder();

            for (int i = 0; i < hashPin.Length; i++)
            {
                returnHashedPin.Append(hashPin[i].ToString());
            }

            return returnHashedPin.ToString();
        }

        /// <summary>
        /// Berfungsi untuk menampilkan partial view berdasarkan tipe user, apakah user powerapps atau user ad di halaman create new user
        /// </summary>
        /// <param name="target">Sebuah string bersisikan referensi halaman mana yang akan ditampilkan</param>
        /// <returns>Menampilkan halaman sesuai dengan yang dipilih user</returns>
        public ActionResult LoadPartialView(string target)
        {
            ViewBag.RoleID = new SelectList(db.Roles.OrderBy(x => x.Name), "ID", "Name");
            ViewBag.ParentUserID = new SelectList(db.Users.Where(x => x.IsActive == true).OrderBy(x => x.Name), "ID", "Name");
            ViewBag.ProcessGroupID = new SelectList(db.ProcessGroups.OrderBy(x => x.Name), "ID", "Name");


            switch (target)
            {
                case "new-powerapps-user-section":
                    //ViewBag.Select = "PowerApps";
                    ViewBag.RoleID = new SelectList(db.Roles.Where(a => a.HaveAccessPowerApps == true && a.ID > 0).OrderBy(x => x.Name), "ID", "Name");
                    return PartialView("_Create2");
                case "powerapps-user-from-ad-section":
                    ViewBag.RoleID = new SelectList(db.Roles.Where(a => a.ParentID == null && a.ID > 0).OrderBy(x => x.Name), "ID", "Name");
                    //ViewBag.Select = "UserAD";
                    return PartialView("_Create3");
                case "user-ad-section":
                    ViewBag.RoleID = new SelectList(db.Roles.Where(a => a.ParentID == null && a.ID > 0).OrderBy(x => x.Name), "ID", "Name");
                    ViewBag.ProcessGroupList = db.ProcessGroups.ToList();
                    return PartialView("_CreateUserAD");
                case "powerapps-user-section":
                    ViewBag.RoleID = new SelectList(db.Roles.Where(a => a.HaveAccessPowerApps == true && a.ID > 0).OrderBy(x => x.Name), "ID", "Name");
                    ViewBag.ProcessGroupList = db.ProcessGroups.ToList();
                    return PartialView("_CreateUserPowerApps");
                default:
                    return PartialView("_Create3");
            }
        }

        /// <summary>
        /// Berfungsi untuk mengecek apakah username yang di input, ada di ad atau tiak
        /// </summary>
        /// <param name="name">Sebuah string yang merupakan username yang di input oleh user</param>
        /// <returns>Jika ada makan return nya true, jika tidak ada maka return nya false</returns>
        public bool IsExistingInAD(string name)
        {
            bool isExist = true;
            try
            {
                //List<User> lstADUsers = new List<User>();
                string DomainPath = WebConfigurationManager.AppSettings["ActiveDirectoryUrl"];
                DirectoryEntry searchRoot = new DirectoryEntry(DomainPath);
                DirectorySearcher search = new DirectorySearcher(searchRoot);
                search.Filter = "(&(objectClass=user)(samaccountname=" + name + "))";
                SearchResultCollection resultCol = search.FindAll();
                if (resultCol == null)
                    isExist = false;
            }
            catch (Exception ex)
            {
                ViewBag.Exception = ex;
            }
            return isExist;
        }

        /// <summary>
        /// Berfungsi untuk mengambil data parent user
        /// </summary>
        /// <param name="role">Sebuah integer yang merupakan id dari role</param>
        /// <param name="processGroup">Sebuah integer yang merupakan id dari process group</param>
        /// <returns></returns>
        public JsonResult GetParent(int? role, int? processGroup)
        {
            if (role != null || processGroup != null)
            {
                //var repo = new RegionsRepository();
                var parentId = db.Roles.Where(x => x.ID == role).SingleOrDefault().ParentID;
                IEnumerable<SelectListItem> parents = new SelectList(db.Users.Where(x => x.IsActive == true && x.RoleID == parentId && x.ProcessGroupID == processGroup), "ID", "UserDetail");
                return Json(parents, JsonRequestBehavior.AllowGet);
            }
            return null;
        }

        /// <summary>
        /// Berfungsi untuk mengecek apakah role yang dipilih mempunyai akses powerapps atau tidak
        /// </summary>
        /// <param name="role">Sebuah integer yang merupakan id dari role</param>
        /// <returns>Jika mempunyai akses powerapps makan return nya true</returns>
        public ActionResult GetIsAccessPA(int? role)
        {
            if (role != null)
            {
                bool isHave = db.Roles.Where(x => x.ID == role).SingleOrDefault().HaveAccessPowerApps;

                return Json(isHave, JsonRequestBehavior.AllowGet);
            }
            return null;
        }

        /// <summary>
        /// berfungsi untuk mengambil data user dari ad
        /// </summary>
        /// <param name="username">Sebuah string yang berisikan username user yang di input oleh user</param>
        /// <returns>Menampilkan data user dari ad</returns>
        public ActionResult CheckUserToAD(string username)
        {
            if (!string.IsNullOrEmpty(username))
            {
                var existUsername = db.Users.Where(x => x.Username == username).SingleOrDefault();
                if (existUsername != null)
                {
                    string errorMessage = "Username already exist";
                    return Json(new { success = false, message = errorMessage }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    SearchResultCollection sResultSet;

                    string domain = WebConfigurationManager.AppSettings["ActiveDirectoryUrl"];
                    string ldapUser = WebConfigurationManager.AppSettings["ADUsername"];
                    string ldapPassword = WebConfigurationManager.AppSettings["ADPassword"];
                    using (DirectoryEntry entry = new DirectoryEntry(domain, ldapUser, ldapPassword))
                    {
                        DirectorySearcher dSearch = new DirectorySearcher(entry);
                        dSearch.Filter = "(&(objectClass=user)(samaccountname=" + username + "))";
                        sResultSet = dSearch.FindAll();
                    }
                    return Json(new { success = true, sResultSet }, JsonRequestBehavior.AllowGet);
                }

            }
            return null;
        }

        /// <summary>
        /// Berfungsi untuk sign out
        /// </summary>
        /// <returns>Muncul pop up untuk login</returns>
        public ActionResult SignOut()
        {
            //if(Session["Username"] == "Signout")
            //{
            //    Session.Abandon();
            //    Response.ClearContent();
            //    Response.StatusCode = 401;
            //    Response.End();
            //}
            //else
            //    Session["Username"] = "Signout";
            Session.Abandon();
            Response.ClearContent();
            Response.StatusCode = 401;
            Response.End();

            //Session["Username"] = "Signout";
            return View();

            //Session.Abandon();
            //Response.Redirect("http://192.168.11.219/utpe");
            //return View();
        }

        /// <summary>
        /// Berfungsi untuk mengupload gambar
        /// </summary>
        /// <param name="pegawiID">Sebuag string yang berisikan id dari user</param>
        /// <param name="photos">Sebuah array yang berisi kumpulan beberapa gambar yang di upload</param>
        /// <returns>Gambar berhasil di upload ke blob storage dan database</returns>
        public async Task UploadImages(string pegawiID, HttpPostedFileBase[] photos)
        {
            var username = User.Identity.Name;
            try
            {
                foreach (var photo in photos)
                {
                    if (photo != null)
                    {
                        var imageUrl = await imageServices.UploadImage(photo);

                        var addUserImage = new UserImage
                        {
                            UserID = new Guid(pegawiID),
                            BlobImage = Convert.ToString(imageUrl),
                            Created = DateTime.Now,
                            CreatedBy = username,
                            LastModified = DateTime.Now,
                            LastModifiedBy = username
                        };
                        db.UserImages.Add(addUserImage);
                        db.SaveChanges();

                    }
                }
            }
            catch (Exception ex)
            {
                ViewBag.Exception = ex;
                ViewBag.ErrorMessage = "An error occured, please check your data input and try again";
            }
        }

        /// <summary>
        /// Berfungsi untuk menghapus gambar
        /// </summary>
        /// <param name="userImage">Sebuah object yang merupakan model dari user image</param>
        public void DeleteImages(UserImage userImage)
        {
            var userImageData = userImage;
            imageServices.DeleteImage(userImageData.BlobImage);
            db.UserImages.Remove(userImageData);
            db.SaveChanges();
        }
    }
}
