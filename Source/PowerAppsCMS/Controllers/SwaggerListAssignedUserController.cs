using PowerAppsCMS.Constants;
using PowerAppsCMS.Models;
using Swashbuckle.Swagger.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Http;

namespace PowerAppsCMS.Controllers
{
    public class SwaggerListAssignedUserController : ApiController
    {
        [Route("api/GetListAssignedUser")]
        [HttpGet]
        [SwaggerResponse(HttpStatusCode.OK,
            Description = "Get list task operator",
            Type = typeof(TaskListOperator))]
        public IHttpActionResult GetListAssignedUser()
        {
            List<ListAssignedOperator> listAssignedOperator = new List<ListAssignedOperator>();
            using (PowerAppsCMSEntities db = new PowerAppsCMSEntities())
            {
                try
                {
                    foreach (User itemUser in db.Users.Where(x => x.IsAssign == true))
                    {
                        ListAssignedOperator listOperator = new ListAssignedOperator();
                        int checkProcessAssign = itemUser.ProcessAssigns.Count();                        

                        if (checkProcessAssign > 0)
                        {
                            ProcessAssign selectedProcessAssign = itemUser.ProcessAssigns.Where(x => x.Status != (int)ProcessActivityStatus.Stop).FirstOrDefault();
                            User assignBy = db.Users.Where(x => x.EmployeeNumber == selectedProcessAssign.CreatedBy).SingleOrDefault();
                            listOperator.NRP = itemUser.EmployeeNumber;
                            listOperator.UserName = itemUser.Name;
                            listOperator.StatusID = selectedProcessAssign.Status;
                            if (listOperator.StatusID == (int)ProcessAssignStatus.NotStarted)
                            {
                                listOperator.StatusName = "Not Started";
                            }
                            else if (listOperator.StatusID == (int)ProcessAssignStatus.Start)
                            {
                                listOperator.StatusName = "Start";
                            }
                            else if (listOperator.StatusID == (int)ProcessAssignStatus.Pause)
                            {
                                listOperator.StatusName = "Pause";
                            }
                            else if (listOperator.StatusID == (int)ProcessAssignStatus.Break)
                            {
                                listOperator.StatusName = "Break";
                            }
                            listOperator.PRO = selectedProcessAssign.Process.Unit.PRO.Number;
                            listOperator.SN = selectedProcessAssign.Process.Unit.SerialNumber;
                            listOperator.ProcessName = selectedProcessAssign.Process.MasterProcess.Name;
                            listOperator.ProductName = selectedProcessAssign.Process.Unit.Product.Name;
                            listOperator.AssignBy = assignBy  != null? assignBy.Name : selectedProcessAssign.CreatedBy;                            
                        }
                        else
                        {
                            listOperator.NRP = itemUser.EmployeeNumber;
                            listOperator.UserName = itemUser.Name;
                            listOperator.StatusID = (int)ProcessAssignStatus.NotAssign;
                            if (listOperator.StatusID == (int)ProcessAssignStatus.NotStarted)
                            {
                                listOperator.StatusName = "Not Started";
                            }
                            else if (listOperator.StatusID == (int)ProcessAssignStatus.Start)
                            {
                                listOperator.StatusName = "Start";
                            }
                            else if (listOperator.StatusID == (int)ProcessAssignStatus.Pause)
                            {
                                listOperator.StatusName = "Pause";
                            }
                            else if (listOperator.StatusID == (int)ProcessAssignStatus.Break)
                            {
                                listOperator.StatusName = "Break";
                            }
                            else if (listOperator.StatusID == (int)ProcessAssignStatus.NotAssign)
                            {
                                listOperator.StatusName = "-";
                            }
                            listOperator.PRO = "-";
                            listOperator.SN = "-";
                            listOperator.ProcessName = "-";
                            listOperator.ProductName = "-";
                            listOperator.AssignBy = "-";
                        }

                        listAssignedOperator.Add(listOperator);
                    }
                    return Ok(listAssignedOperator);
                }
                catch (Exception ex)
                {
                    return InternalServerError(ex);
                }
            }
        }
    }
}