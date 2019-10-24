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
    /// <summary>
    /// Controller yang berisikan fungsi-fungsi yang digunakan dihalaman Operator
    /// </summary>
    public class SwaggerOperatorController : ApiController
    {
        /// <summary>
        /// Mengambil daftar tugas yang telah ditgaskan pada operator
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="prevStatus"></param>
        /// <returns></returns>
        [Route("api/GetTaskListOperator")]
        [HttpGet]
        [SwaggerResponse(HttpStatusCode.OK,
            Description = "Get list task operator",
            Type = typeof(TaskListOperator))]
        [SwaggerResponse(HttpStatusCode.InternalServerError,
                Description = "Internal Server Error",
               Type = typeof(Exception))]
        [SwaggerOperation("GetTaskListOperator")]

        public IHttpActionResult GetTaskListOperator(string userID, int prevStatus)
        {
            using (PowerAppsCMSEntities db = new PowerAppsCMSEntities())
            {
                DateTime now = DateTime.Now;
                List<TaskListOperator> listTaskOperator = new List<TaskListOperator>();
                Guid guidUserID = Guid.Parse(userID.Replace(" ", string.Empty));
                try
                {
                    if (prevStatus == 0)
                    {
                        //foreach (ProcessAssign itemProcessAssign in db.ProcessAssigns.Where(x => x.UserID == guidUserID))
                          foreach (ProcessAssign itemProcessAssign in db.ProcessAssigns.Where(x => x.UserID == guidUserID && x.Status < 4))
                            {
                            TaskListOperator taskListOperator = new TaskListOperator();
                            taskListOperator.ProcessID = itemProcessAssign.ProcessID;
                            taskListOperator.ProcessAssignID = itemProcessAssign.ID;
                            taskListOperator.UserID = itemProcessAssign.UserID;
                            taskListOperator.SerialNumber = itemProcessAssign.Process.Unit.SerialNumber;
                            taskListOperator.ProcessName = itemProcessAssign.Process.MasterProcess.Name;
                            taskListOperator.StatusID = itemProcessAssign.Status;
                            taskListOperator.PRO = itemProcessAssign.Process.Unit.PRO.Number;
                            taskListOperator.ProductName = itemProcessAssign.Process.Unit.Product.Name;
                            taskListOperator.Customer = itemProcessAssign.Process.Unit.PRO.CustomerListInSODisplayText;//Customer;
                            taskListOperator.Type = itemProcessAssign.Type;
                            taskListOperator.ProcessStatus = itemProcessAssign.Process.Status;
                            if (itemProcessAssign.Process.ProcessIssues.Where(x => x.Status == false).Count() > 0)
                            {
                                taskListOperator.ProcessStatus = (int)ProcessStatus.Issue;
                            }

                            listTaskOperator.Add(taskListOperator);
                        }
                        return Ok(listTaskOperator);
                    }
                    else
                    {
                        //List<ProcessAssign> listProcessAssign = db.ProcessAssigns.Where(x => x.UserID == guidUserID).ToList();
                        List<ProcessAssign> listProcessAssign = db.ProcessAssigns.Where(x => x.UserID == guidUserID && x.Status >= 4).ToList();
                        foreach (var item in listProcessAssign.Where(x => x.LastModified.Date >= now.AddDays(-7)))
                        {
                            TaskListOperator taskListOperator = new TaskListOperator();
                            taskListOperator.ProcessID = item.ProcessID;
                            taskListOperator.ProcessAssignID = item.ID;
                            taskListOperator.UserID = item.UserID;
                            taskListOperator.SerialNumber = item.Process.Unit.SerialNumber;
                            taskListOperator.ProcessName = item.Process.MasterProcess.Name;
                            taskListOperator.StatusID = item.Status;
                            taskListOperator.PRO = item.Process.Unit.PRO.Number;
                            taskListOperator.ProductName = item.Process.Unit.Product.Name;
                            taskListOperator.Customer = item.Process.Unit.PRO.CustomerListInSODisplayText;//Customer;
                            taskListOperator.Type = item.Type;
                            taskListOperator.ProcessStatus = item.Process.Status;
                            listTaskOperator.Add(taskListOperator);
                        }
                        return Ok(listTaskOperator);
                    }
                }
                catch (Exception ex)
                {
                    return InternalServerError(ex);
                }
            }
        }

        /// <summary>
        /// Mengambil daftar log pekerjaan operator, untuk melihat aktivitas pekerjaan operator
        /// </summary>
        /// <param name="processAssignID"></param>
        /// <param name="userID"></param>
        /// <returns></returns>
        [Route("api/GetWorkingLog")]
        [HttpGet]
        [SwaggerResponse(HttpStatusCode.OK,
            Description = "Get specific working log",
            Type = typeof(WorkingLog))]
        [SwaggerResponse(HttpStatusCode.InternalServerError,
                Description = "Internal Server Error",
               Type = typeof(Exception))]
        [SwaggerOperation("GetWorkingLog")]

        public IHttpActionResult GetWorkingLog(int processAssignID, string userID)
        {
            using (PowerAppsCMSEntities db = new PowerAppsCMSEntities())
            {
                List<WorkingLog> listWorkingLog = new List<WorkingLog>();
                Guid guidUserID = Guid.Parse(userID.Replace(" ", string.Empty));
                try
                {
                    foreach (ProcessActivity itemProcessActivity in db.ProcessActivities.Where(x => x.ProcessAssignID == processAssignID && x.ProcessAssign.UserID == guidUserID))
                    {
                        WorkingLog workingLog = new WorkingLog();
                        workingLog.ActivityDateTime = itemProcessActivity.ActivityDateTime.ToString("dd/MM/yyyy HH:mm:ss");

                        if (itemProcessActivity.ReasonPauseID == null && itemProcessActivity.Status == (int)ProcessActivityStatus.Start)
                        {
                            workingLog.Remark = "Start";
                        }
                        else if (itemProcessActivity.ReasonPauseID == null && itemProcessActivity.Status == (int)ProcessActivityStatus.Break)
                        {
                            workingLog.Remark = "Break";
                        }
                        else if (itemProcessActivity.ReasonPauseID == null && itemProcessActivity.Status == (int)ProcessActivityStatus.Stop)
                        {
                            workingLog.Remark = "Stop";
                        }
                        else if (itemProcessActivity.ReasonPauseID == null && itemProcessActivity.Status == (int)ProcessActivityStatus.StopByGroupLeader)
                        {
                            workingLog.Remark = "Stop by Group Leader";
                        }
                        else
                        {
                            workingLog.Remark = itemProcessActivity.ReasonPause.Name;
                        }
                        listWorkingLog.Add(workingLog);
                    }
                    return Ok(listWorkingLog);
                }
                catch (Exception ex)
                {
                    return InternalServerError(ex);
                }
            }

        }

        /// <summary>
        /// Fungsi untuk start proses pekerjaan operator
        /// </summary>
        /// <param name="processID"></param>
        /// <param name="processAssignID"></param>
        /// <param name="userName"></param>
        /// <returns></returns>
        [Route("api/StartByOperator")]
        [HttpPost]
        public IHttpActionResult StartByOperator(int processID, int processAssignID, string userName)
        {
            using (PowerAppsCMSEntities db = new PowerAppsCMSEntities())
            {
                DateTime now = DateTime.Now;                

                ProcessActivity processActivity = new ProcessActivity();
                processActivity.ProcessAssignID = processAssignID;
                processActivity.Status = (int)ProcessActivityStatus.Start;
                processActivity.Created = processActivity.LastModified = processActivity.ActivityDateTime = now;
                processActivity.CreatedBy = processActivity.LastModifiedBy = userName;

                ProcessAssign processAssign = db.ProcessAssigns.Where(x => x.ID == processAssignID).FirstOrDefault();
                processAssign.Status = (int)ProcessAssignStatus.Start;
                processAssign.LastModified = now;
                processAssign.LastModifiedBy = userName;
                processAssign.ProcessActivities.Add(processActivity);
                if (db.SaveChanges() > 0)
                {
                    Process selectedProcess = db.Processes.Where(x => x.ID == processID).SingleOrDefault();
                    if (selectedProcess.ActualStartDate == null)
                    {
                        selectedProcess.ActualStartDate = now;
                    }

                    if (selectedProcess.Status != (int)ProcessStatus.OnProcess)
                    {
                        selectedProcess.Status = (int)ProcessStatus.OnProcess;
                        selectedProcess.LastModified = now;
                        selectedProcess.LastModifiedBy = userName;
                    }
                    else if (selectedProcess.Status == (int)ProcessStatus.OnProcess)
                    {
                        selectedProcess.LastModified = now;
                        selectedProcess.LastModifiedBy = userName;
                    }

                    if(db.SaveChanges() > 0)
                    {
                        ProcessStatusLogs processStatusLogs = new ProcessStatusLogs();
                        processStatusLogs.ProcessID = processID;
                        processStatusLogs.Description = "On Process";
                        processStatusLogs.Status = (int)ProcessStatus.OnProcess;
                        processStatusLogs.StatusName = "On Process";
                        processStatusLogs.CreatedBy = userName;
                        processStatusLogs.Created = now;
                        db.ProcessStatusLogs.Add(processStatusLogs);
                        db.SaveChanges();
                    }
                }
                return Ok(HttpStatusCode.OK);
            }
        }

        /// <summary>
        /// Fungsi untuk stop pekerjaan operator
        /// </summary>
        /// <param name="processAssignID"></param>
        /// <param name="userName"></param>
        /// <param name="userID"></param>
        /// <returns></returns>
        [Route("api/StopByOperator")]
        [HttpPost]
        public IHttpActionResult StopByOperator(int processAssignID, string userName, string userID)
        {
            using (PowerAppsCMSEntities db = new PowerAppsCMSEntities())
            {
                Guid guidUserID = Guid.Parse(userID.Replace(" ", string.Empty));
                DateTime now = DateTime.Now;
                decimal actualHours = 0;

                ProcessActivity workinLog = db.ProcessActivities.Where(x => x.ProcessAssignID == processAssignID && x.ProcessAssign.UserID == guidUserID).OrderByDescending(x => x.ID).FirstOrDefault();
                if (workinLog != null && workinLog.Status == 1)
                {
                    actualHours = Convert.ToDecimal(now.Subtract(workinLog.ActivityDateTime).TotalSeconds);

                }

                ProcessActivity processActivity = new ProcessActivity();
                processActivity.Status = (int)ProcessActivityStatus.Stop;
                processActivity.CreatedBy = processActivity.LastModifiedBy = userName;
                processActivity.Created = processActivity.LastModified = processActivity.ActivityDateTime = now;
                processActivity.ActualHours = actualHours;

                ProcessAssign processAssign = db.ProcessAssigns.Where(x => x.ID == processAssignID).FirstOrDefault();
                processAssign.Status = (int)ProcessAssignStatus.Stop;
                processAssign.LastModified = now;
                processAssign.LastModifiedBy = userName;
                processAssign.ProcessActivities.Add(processActivity);

                if (db.SaveChanges() > 0)
                {
                    Process selectedProcess = db.Processes.Where(x => x.ID == processAssign.ProcessID).SingleOrDefault();
                    if (selectedProcess.ProcessAssigns.Where(x => x.Status != (int)ProcessAssignStatus.Stop).Count() == 0)
                    {
                        selectedProcess.Status = (int)ProcessStatus.StopByOperator; 
                        selectedProcess.LastModifiedBy = userName;
                        selectedProcess.LastModified = now;
                    }

                    User user = db.Users.Find(processAssign.UserID);
                    user.IsAssign = false;

                    if (db.SaveChanges() > 0)
                    {
                        ProcessStatusLogs processStatusLogs = new ProcessStatusLogs();
                        processStatusLogs.ProcessID = processAssign.ProcessID;
                        processStatusLogs.Description = "Stop by Operator";
                        processStatusLogs.Status = (int)ProcessStatus.StopByOperator;
                        processStatusLogs.StatusName = "Stop by Operator";
                        processStatusLogs.CreatedBy = userName;
                        processStatusLogs.Created = now;
                        db.ProcessStatusLogs.Add(processStatusLogs);
                        db.SaveChanges();
                    }
                }
                return Ok(HttpStatusCode.OK);
            }
        }

        /// <summary>
        /// Fungsi untuk pause operator
        /// </summary>
        /// <param name="processAssignID"></param>
        /// <param name="userName"></param>
        /// <param name="userID"></param>
        /// <param name="reasonID"></param>
        /// <returns></returns>
        [Route("api/PauseByOperator")]
        [HttpPost]
        public IHttpActionResult PauseByOperator(int processAssignID, string userName, string userID, int reasonID)
        {
            using (PowerAppsCMSEntities db = new PowerAppsCMSEntities())
            {
                Guid guidUserID = Guid.Parse(userID.Replace(" ", string.Empty));                
                DateTime now = DateTime.Now;
                decimal actualHours = 0;

                var workinLog = db.ProcessActivities.Where(x => x.ProcessAssignID == processAssignID && x.ProcessAssign.UserID == guidUserID).OrderByDescending(x => x.ID).FirstOrDefault();
                if (workinLog != null && workinLog.Status == 1)
                {
                    actualHours = Convert.ToDecimal(now.Subtract(workinLog.ActivityDateTime).TotalSeconds);

                }

                ProcessActivity processActivity = new ProcessActivity();
                processActivity.Status = (int)ProcessActivityStatus.Pause;
                processActivity.CreatedBy = processActivity.LastModifiedBy = userName;
                processActivity.Created = processActivity.LastModified = processActivity.ActivityDateTime = now;
                processActivity.ActualHours = actualHours;
                processActivity.ReasonPauseID = reasonID;

                ProcessAssign processAssign = db.ProcessAssigns.Where(x => x.ID == processAssignID).FirstOrDefault();
                processAssign.Status = (int)ProcessAssignStatus.Pause;
                processAssign.LastModified = now;
                processAssign.LastModifiedBy = userName;
                processAssign.ProcessActivities.Add(processActivity);

                db.SaveChanges();                               

                return Ok(HttpStatusCode.OK);
            }
        }
    }
}