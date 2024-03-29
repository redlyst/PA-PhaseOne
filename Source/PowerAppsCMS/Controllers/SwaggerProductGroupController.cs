﻿using PowerAppsCMS.Constants;
using PowerAppsCMS.Models;
using Swashbuckle.Swagger.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Http;

namespace PowerAppsCMS.Controllers
{
    /// <summary>
    /// Controller yang digunakan untuk mengelola fungsi-fungsi yang ada pada halaman grup leader di aplikasi powerapps
    /// </summary>
    public class SwaggerProductGroupController : ApiController
    {
        /// <summary>
        /// Mengambil production list yang sesuai dengan process group Group Leader yang login
        /// </summary>
        /// <param name="processGroupID"></param>
        /// <returns>Daftar production list yang sesuai dengan process group Group Leader yang login</returns>
        [Route("api/GetProductGroup")]
        [HttpGet]
        [SwaggerResponse(HttpStatusCode.OK,
            Description = "Get all product group",
            Type = typeof(ProductGroupAPI))]
        [SwaggerResponse(HttpStatusCode.InternalServerError,
                Description = "Internal Server Error",
               Type = typeof(Exception))]
        [SwaggerOperation("GetProductGroup")]




        public IHttpActionResult GetGroupProduct()
        {
            using (PowerAppsCMSEntities db = new PowerAppsCMSEntities())
            {
                List<ProductGroupAPI> listProductGroup = new List<ProductGroupAPI>();
                //DateTime now = DateTime.Now;
                try
                {
                    var itemProcess = db.ProductGroups;

                    foreach (ProductGroup itemUnit in itemProcess)
                    {
                        ProductGroupAPI PG = new ProductGroupAPI();
                        PG.ID = itemUnit.ID;
                        PG.Name = itemUnit.Name;
                        PG.Desc = itemUnit.Description;

                        listProductGroup.Add(PG);
                    }
                    return Ok(listProductGroup);
                }
                catch (Exception ex)
                {
                    return InternalServerError(ex);
                }
            }
        }
/*        [Route("api/GetProductGroup")]
        [HttpGet]

        public IHttpActionResult DeleteAssignOperator(int processAssignID, string userID)
        {
            Guid guidUserID = Guid.Parse(userID.Replace(" ", string.Empty));
            
            try
            {
                using (PowerAppsCMSEntities db = new PowerAppsCMSEntities())
                {
                    ProcessAssign selectedProcessAssign = db.ProcessAssigns.Where(x => x.ID == processAssignID).SingleOrDefault();
                    db.ProcessAssigns.Remove(selectedProcessAssign);
                    if(db.SaveChanges() > 0)
                    {
                        User selectedUser = db.Users.Where(x => x.ID == guidUserID).SingleOrDefault();
                        selectedUser.IsAssign = false;
                        db.SaveChanges();
                    }
                    return Ok(HttpStatusCode.OK);
                }
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
         
        }*/

    }
}
