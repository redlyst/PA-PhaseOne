using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Http;
using PowerAppsCMS.Models;
using Swashbuckle.Swagger.Annotations;
using Newtonsoft.Json;
using System.Security.Cryptography;
using System.Text;

namespace PowerAppsCMS.Controllers
{
    /// <summary>
    /// Controller untuk mendapatkan user
    /// </summary>
    public class SwaggerGetUserController : ApiController
    {
        /// <summary>
        /// Mengambil daftar operator yang masih aktif
        /// </summary>
        /// <returns>Daftar operator yang masih aktif</returns>
        [Route("api/GetAllOperator")]
        [HttpGet]
        [SwaggerResponse(HttpStatusCode.OK,
            Description = "Get all Operator",
            Type = typeof(UserModel))]
        [SwaggerResponse(HttpStatusCode.InternalServerError,
                Description = "Internal Server Error",
               Type = typeof(Exception))]
        [SwaggerOperation("GetAllOperator")]
        public IHttpActionResult GetAllOperator()
        {
            using (PowerAppsCMSEntities db = new PowerAppsCMSEntities())
            {
                List<UserModel> listOfOperators = new List<UserModel>();
                try
                {
                    foreach (User itemUser in db.Users.Where(x => x.RoleID == 8 && x.IsActive == true))
                    {
                        UserModel userModel = new UserModel();
                        userModel.ID = itemUser.ID;
                        userModel.Name = itemUser.Name;
                        userModel.EmployeeNumber = itemUser.EmployeeNumber;
                        userModel.RoleID = itemUser.RoleID;
                        userModel.ProcessGroupID = itemUser.ProcessGroupID;
                        userModel.ParentUserID = itemUser.ParentUserID;
                        userModel.Email = itemUser.Email;
                        userModel.IsActive = itemUser.IsActive;
                        userModel.IsAssign = itemUser.IsAssign;
                        userModel.PersonID = itemUser.PersonID;
                        userModel.ProcessGroupName = itemUser.ProcessGroup.Name;
                        userModel.RoleName = itemUser.Role.Name;
                        userModel.PIN = itemUser.PIN;
                        userModel.UserName = itemUser.Username;
                        //userModel.GambarPegawai = operators.GambarPegawais;
                        listOfOperators.Add(userModel);
                    }
                    return Ok(listOfOperators);
                }
                catch (Exception ex)
                {
                    return InternalServerError(ex);
                }
            }
        }

        /// <summary>
        /// Mengambil daftar operator PB
        /// </summary>
        /// <returns>Daftar operator PB</returns>
        [Route("api/GetAllOperatorPB")]
        [HttpGet]
        [SwaggerResponse(HttpStatusCode.OK,
            Description = "Get all Operator PB",
            Type = typeof(UserModel))]
        [SwaggerResponse(HttpStatusCode.InternalServerError,
                Description = "Internal Server Error",
               Type = typeof(Exception))]
        [SwaggerOperation("GetAllOperatorPB")]
        public IHttpActionResult GetAllOperatorPB()
        {
            using (PowerAppsCMSEntities db = new PowerAppsCMSEntities())
            {
                List<UserModel> listOfUsers = new List<UserModel>();
                try
                {
                    foreach (User itemUser in db.Users.Where(x => x.RoleID == 12 && x.IsActive == true))
                    {
                        UserModel userModel = new UserModel();
                        userModel.ID = itemUser.ID;
                        userModel.Name = itemUser.Name;
                        userModel.EmployeeNumber = itemUser.EmployeeNumber;
                        userModel.RoleID = itemUser.RoleID;
                        userModel.ProcessGroupID = itemUser.ProcessGroupID;
                        userModel.ParentUserID = itemUser.ParentUserID;
                        userModel.Email = itemUser.Email;
                        userModel.IsActive = itemUser.IsActive;
                        userModel.PersonID = itemUser.PersonID;
                        userModel.ProcessGroupName = itemUser.ProcessGroup.Name;
                        userModel.RoleName = itemUser.Role.Name;
                        userModel.PIN = itemUser.PIN;
                        userModel.UserName = itemUser.Username;
                        //userModel.GambarPegawai = user.GambarPegawais;
                        listOfUsers.Add(userModel);
                    }
                    return Ok(listOfUsers);
                }
                catch (Exception ex)
                {
                    return InternalServerError(ex);
                }
            }
        }

        /// <summary>
        /// Mengambil spesifik operator berdasarkan personID
        /// </summary>
        /// <param name="PersonID"></param>
        /// <returns>Data operator</returns>
        [Route("api/GetOperator")]
        [HttpGet]
        [SwaggerResponse(HttpStatusCode.OK,
            Description = "Get specific Operator",
            Type = typeof(UserModel))]
        [SwaggerResponse(HttpStatusCode.InternalServerError,
                Description = "Internal Server Error",
               Type = typeof(Exception))]
        [SwaggerOperation("GetOperator")]
        public IHttpActionResult GetOperator(string PersonID)
        {
            PowerAppsCMSEntities db = new PowerAppsCMSEntities();

            try
            {
                User findUser = db.Users.Where(x => x.PersonID == PersonID).SingleOrDefault();

                if (findUser != null)
                {
                    var user = new UserModel
                    {
                        ID = findUser.ID,
                        Name = findUser.Name,
                        EmployeeNumber = findUser.EmployeeNumber,
                        RoleID = findUser.RoleID,
                        ProcessGroupID = findUser.ProcessGroupID,
                        ParentUserID = findUser.ParentUserID,
                        Email = findUser.Email,
                        IsActive = findUser.IsActive,
                        PersonID = findUser.PersonID,
                        ProcessGroupName = findUser.ProcessGroup.Name,
                        RoleName = findUser.Role.Name,
                        PIN = findUser.PIN,
                        UserName = findUser.Username
                    };
                    return Ok(user);
                }
                return null;
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        /// <summary>
        /// Login dengan menggunakan username(NRP) dan PIN
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="PIN"></param>
        /// <remarks>mengubah pin yang dimasukkan user dalam bentuk encrypt</remarks>
        /// <returns>Data operator</returns>
        [Route("api/Login")]
        [HttpGet]
        [SwaggerResponse(HttpStatusCode.OK,
            Description = "Login using PIN",
            Type = typeof(UserModel))]
        [SwaggerResponse(HttpStatusCode.InternalServerError,
                Description = "Internal Server Error",
               Type = typeof(Exception))]
        [SwaggerOperation("Login")]
        public IHttpActionResult Login(string userName, string PIN)
        {
            PowerAppsCMSEntities db = new PowerAppsCMSEntities();
            try
            {
                var hashedPin = EncryptPin(PIN);
                var findUser = db.Users.Where(x => x.PIN == hashedPin && x.EmployeeNumber == userName && x.IsActive == true).SingleOrDefault();
                if (findUser != null)
                {
                    var user = new UserModel
                    {
                        LoginStatus = 1,
                        ID = findUser.ID,
                        Name = findUser.Name,
                        EmployeeNumber = findUser.EmployeeNumber,
                        RoleID = findUser.RoleID,
                        ProcessGroupID = findUser.ProcessGroupID == null?0:findUser.ProcessGroupID,
                        ParentUserID = findUser.ParentUserID,
                        Email = findUser.Email,
                        IsActive = findUser.IsActive,
                        PersonID = findUser.PersonID,
                        ProcessGroupName = findUser.ProcessGroupID == null ? "NONAME":findUser.ProcessGroup.Name,
                        RoleName = findUser.Role.Name,
                        PIN = findUser.PIN,
                        UserName = findUser.Username
                    };
                    return Ok(user);
                }

                else
                {
                    var user = new UserModel
                    {
                        LoginStatus = 0
                    };
                    return Ok(user);
                }

                //return null;
            }
            catch (Exception ex)
            {
                return InternalServerError( ex);
            }
        }

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
    }
}
