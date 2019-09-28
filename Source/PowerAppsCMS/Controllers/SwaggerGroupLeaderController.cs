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
    /// Controller yang digunakan untuk mengelola fungsi-fungsi yang ada pada halaman grup leader di aplikasi powerapps
    /// </summary>
    public class SwaggerGroupLeaderController : ApiController
    {
        /// <summary>
        /// Mengambil production list yang sesuai dengan process group Group Leader yang login
        /// </summary>
        /// <param name="processGroupID"></param>
        /// <returns>Daftar production list yang sesuai dengan process group Group Leader yang login</returns>
        [Route("api/GetGroupLeaderProductionPlanning")]
        [HttpGet]
        [SwaggerResponse(HttpStatusCode.OK,
            Description = "Get specific production planning",
            Type = typeof(ProductionPlanning))]
        [SwaggerResponse(HttpStatusCode.InternalServerError,
                Description = "Internal Server Error",
               Type = typeof(Exception))]
        [SwaggerOperation("GetGroupLeaderProductionPlanning")]

        public IHttpActionResult GetGroupLeaderProductionPlanning(int processGroupID, int ProductGroupID)
        {
            using (PowerAppsCMSEntities db = new PowerAppsCMSEntities())
            {
                List<ProductionPlanning> listProductionPlanning = new List<ProductionPlanning>();
                List<Process> selectedProcesslist = null;
                DateTime now = DateTime.Now;
                try
                {
                    var itemProcess = db.Processes.Where(x => x.MasterProcess.ProcessGroupID == processGroupID && x.Unit.IsHold == false && x.Unit.SFSDueDate <= x.Unit.MPSDueDate && x.Unit.Product.ProductSubGroups.ProductGroup.ID == ProductGroupID).Select(x => x.Unit).Distinct();

                    foreach (Unit itemUnit in itemProcess)
                    {
                        ProductionPlanning productionPlanning = new ProductionPlanning();
                        productionPlanning.UnitID = itemUnit.ID;
                        //productionPlanning.MasterProcessID = productionPlannings.MasterProcessID;
                        //productionPlanning.p = item.Processes;
                        productionPlanning.SerialNumber = itemUnit.SerialNumber;
                        productionPlanning.PRO = itemUnit.PRO.Number;
                        productionPlanning.Product = itemUnit.Product.Name;
                        productionPlanning.Customer = itemUnit.PRO.CustomerListInSODisplayText;// Customer;
                        productionPlanning.ProductID = itemUnit.ProductID;
                        //productionPlanning.PGID = itemUnit.Product.ProductSubGroups.ProductGroup.ID;
                        //productionPlanning.PGName = itemUnit.Product.ProductSubGroups.ProductGroup.Name;
                        var prs = db.Processes.Where(y => y.MasterProcess.ProductID == itemUnit.ProductID && y.Unit.ID == itemUnit.ID);
                        foreach (Process item in prs )
                        {
                            if(item.Status == 7)
                            {
                                productionPlanning.ProcessProgres = "Finish";
                            }
                            else
                            {
                                productionPlanning.ProcessProgres = "Not Finish";
                            }
                            
                        }
                        selectedProcesslist = db.Processes.Where(x => x.MasterProcess.ProductID == productionPlanning.ProductID && x.UnitID == productionPlanning.UnitID && x.MasterProcess.ProcessGroupID == processGroupID).ToList();
                        int countSelectedProcessList = selectedProcesslist.Count();
                        if (selectedProcesslist.Where(x => x.PlanStartDate > now && x.Status == (int)ProcessStatus.NotStarted).Count() == countSelectedProcessList)
                        {
                            productionPlanning.StatusID = (int)UnitStatus.NotStarted;
                            productionPlanning.StatusName = "Not Started";
                        }
                        else if (selectedProcesslist.Where(x => (x.Status != (int)ProcessStatus.QCPassed || x.Status != (int)ProcessStatus.Finish) && (x.ProcessIssues.Count(y => y.Status == false) > 0)).Count() > 0)
                        {
                            productionPlanning.StatusID = (int)UnitStatus.Issue;
                            productionPlanning.StatusName = "Issue";
                        }
                        else if (selectedProcesslist.Where(x => now > x.PlanEndDate && (x.Status != (int)ProcessStatus.QCPassed || x.Status != (int)ProcessStatus.Finish)).Count() >= 1)
                        {
                            productionPlanning.StatusID = (int)UnitStatus.Issue;
                            productionPlanning.StatusName = "Process Late";
                        }
                        else if (selectedProcesslist.Where(x => x.Status == (int)ProcessStatus.QCNotGood).Count() >= 1)
                        {
                            productionPlanning.StatusID = (int)UnitStatus.QCNotGood;
                            productionPlanning.StatusName = "QC Not Good";
                        }
                        else
                        {
                            productionPlanning.StatusID = (int)UnitStatus.OnProcess;
                        }
                        listProductionPlanning.Add(productionPlanning);
                    }
                    return Ok(listProductionPlanning);
                }
                catch (Exception ex)
                {
                    return InternalServerError(ex);
                }
            }
        }

        [Route("api/GetUnitsByGroupLeaderUserID")]
        [HttpGet]
        public IHttpActionResult GetUnitsByGroupLeaderUserID(string userID)
        {
            using (PowerAppsCMSEntities db = new PowerAppsCMSEntities())
            {
                Guid guidUserID = Guid.Parse(userID.Replace(" ", string.Empty));
                List<ProductionPlanning> listProductionPlanning = new List<ProductionPlanning>();
                List<Process> selectedProcesslist = null;
                DateTime now = DateTime.Now;
                try
                {
                    List<UserProcessGroup> listUserProcessGroup = db.UserProcessGroups.Where(x => x.UserID == guidUserID).ToList();
                    List<int> listProcessGroupID = new List<int>();

                    if (listUserProcessGroup != null)
                    {
                        listProcessGroupID = listUserProcessGroup.Select(x => x.ProcessGroupID).ToList();
                    }

                    if (listProcessGroupID != null)
                    {
                        var itemProcess = db.Processes.Where(x => listProcessGroupID.Contains(x.MasterProcess.ProcessGroupID) && x.Unit.IsHold == false && x.Unit.SFSDueDate <= x.Unit.MPSDueDate).Select(x => x.Unit).Distinct();

                        foreach (Unit itemUnit in itemProcess)
                        {
                            ProductionPlanning productionPlanning = new ProductionPlanning();
                            productionPlanning.UnitID = itemUnit.ID;
                            //productionPlanning.MasterProcessID = productionPlannings.MasterProcessID;
                            //productionPlanning.p = item.Processes;
                            productionPlanning.SerialNumber = itemUnit.SerialNumber;
                            productionPlanning.PRO = itemUnit.PRO.Number;
                            productionPlanning.Product = itemUnit.Product.Name;
                            productionPlanning.Customer = itemUnit.PRO.CustomerListInSODisplayText;// Customer;
                            productionPlanning.ProductID = itemUnit.ProductID;

                            selectedProcesslist = db.Processes.Where(x => x.MasterProcess.ProductID == productionPlanning.ProductID && x.UnitID == productionPlanning.UnitID && listProcessGroupID.Contains(x.MasterProcess.ProcessGroupID)).ToList();
                            if (selectedProcesslist.Where(x => x.PlanStartDate > now && x.Status != (int)ProcessStatus.NotStarted).Count() == 0)
                            {
                                productionPlanning.StatusID = (int)UnitStatus.NotStarted;
                                productionPlanning.StatusName = "Not Started";
                            }
                            else if (selectedProcesslist.Where(x => (x.Status != (int)ProcessStatus.QCPassed || x.Status != (int)ProcessStatus.Finish) && (x.ProcessIssues.Count(y => y.Status == false) > 0)).Count() > 0)
                            {
                                productionPlanning.StatusID = (int)UnitStatus.Issue;
                                productionPlanning.StatusName = "Issue";
                            }
                            else if (selectedProcesslist.Where(x => now > x.PlanEndDate && (x.Status != (int)ProcessStatus.QCPassed || x.Status != (int)ProcessStatus.Finish)).Count() >= 1)
                            {
                                productionPlanning.StatusID = (int)UnitStatus.Issue;
                                productionPlanning.StatusName = "Process Late";
                            }
                            else if (selectedProcesslist.Where(x => x.Status == 6).Count() >= 1)
                            {
                                productionPlanning.StatusID = (int)UnitStatus.QCNotGood;
                                productionPlanning.StatusName = "QC Not Good";
                            }
                            else
                            {
                                productionPlanning.StatusID = (int)UnitStatus.OnProcess;
                            }

                            listProductionPlanning.Add(productionPlanning);
                        }
                    }
                    return Ok(listProductionPlanning);
                }
                catch (Exception ex)
                {
                    return InternalServerError(ex);
                }
            }
        }

        /// <summary>
        /// Mengambil semua daftar process yang ada pada unit yang dipilih
        /// </summary>
        /// <param name="productID"></param>
        /// <param name="unitID"></param>
        /// <param name="userID"></param>
        /// <returns>Daftar process dari unit yang dipilih</returns>
        [Route("api/GetProductionProcessDetail")]
        [HttpGet]
        [SwaggerResponse(HttpStatusCode.OK,
            Description = "Get production process detail",
            Type = typeof(ProductionPlanningUnitDetail))]
        [SwaggerResponse(HttpStatusCode.InternalServerError,
                Description = "Internal Server Error",
               Type = typeof(Exception))]
        [SwaggerOperation("GetProductionProcessDetail")]

        public IHttpActionResult GetProductionProcessDetail(int productID, int unitID)
        {
            using (PowerAppsCMSEntities db = new PowerAppsCMSEntities())
            {
                List<ProductionPlanningUnitDetail> listOfProductionPlanningUnitDetail = new List<ProductionPlanningUnitDetail>();
                try
                {
                    foreach (Process processUnitList in db.Processes.Where(x => x.MasterProcess.ProductID == productID && x.UnitID == unitID).ToList())
                    {
                        ProductionPlanningUnitDetail productionPlanningUnitDetail = new ProductionPlanningUnitDetail();
                        productionPlanningUnitDetail.ProcessID = processUnitList.ID;
                        productionPlanningUnitDetail.ProcessGroupID = processUnitList.MasterProcess.ProcessGroupID;
                        productionPlanningUnitDetail.StatusID = processUnitList.Status;

                        if (productionPlanningUnitDetail.StatusID == (int)ProcessStatus.NotStarted)
                        {
                            productionPlanningUnitDetail.StatusName = "Not Started";
                        }
                        else if (productionPlanningUnitDetail.StatusID == (int)ProcessStatus.OnProcess)
                        {
                            productionPlanningUnitDetail.StatusName = "On Process";
                        }
                        else if (productionPlanningUnitDetail.StatusID == (int)ProcessStatus.OnProcess && processUnitList.PlanEndDate < DateTime.Now)
                        {
                            productionPlanningUnitDetail.StatusName = "On Process (Late)";
                        }
                        else if (productionPlanningUnitDetail.StatusID == (int)ProcessStatus.StopByOperator)
                        {
                            productionPlanningUnitDetail.StatusName = "Stopped By Operator";
                        }
                        else if (productionPlanningUnitDetail.StatusID == (int)ProcessStatus.WaitForQC)
                        {
                            productionPlanningUnitDetail.StatusName = "Wait For QC";
                        }
                        else if (productionPlanningUnitDetail.StatusID == (int)ProcessStatus.QCPassed)
                        {
                            productionPlanningUnitDetail.StatusName = "QC Passed";
                        }
                        else if (productionPlanningUnitDetail.StatusID == (int)ProcessStatus.QCNotGood)
                        {
                            productionPlanningUnitDetail.StatusName = "QC Not Good";
                        }
                        else if (productionPlanningUnitDetail.StatusID == (int)ProcessStatus.Finish)
                        {
                            productionPlanningUnitDetail.StatusName = "Finish";
                        }

                        productionPlanningUnitDetail.ProcessName = processUnitList.MasterProcess.Name;
                        productionPlanningUnitDetail.ManHour = processUnitList.MasterProcess.ManHour;
                        productionPlanningUnitDetail.ManPower = processUnitList.MasterProcess.ManPower;
                        decimal? totalMHActual = processUnitList.ProcessAssigns.Sum(x => x.ProcessActivities.Sum(y => y.ActualHours));// db.ProcessActivities.Where(x => x.ProcessAssign.ProcessID == processUnitList.ID).Sum(x => x.ActualHours);

                        productionPlanningUnitDetail.ManHourActual = totalMHActual;
                        productionPlanningUnitDetail.MasterProcessID = processUnitList.MasterProcessID;

                        listOfProductionPlanningUnitDetail.Add(productionPlanningUnitDetail);
                    }
                    return Ok(listOfProductionPlanningUnitDetail);
                }
                catch (Exception ex)
                {
                    return InternalServerError(ex);
                }
            }
        }

        [Route("api/GetProductionProcessDetail1")]
        [HttpGet]
        [SwaggerResponse(HttpStatusCode.OK,
            Description = "Get production process detail",
            Type = typeof(ProductionPlanningUnitDetail))]
        [SwaggerResponse(HttpStatusCode.InternalServerError,
                Description = "Internal Server Error",
               Type = typeof(Exception))]
        [SwaggerOperation("GetProductionProcessDetail1")]

        public IHttpActionResult GetProductionProcessDetail1(int productID, int unitID, string userID)
        {
            using (PowerAppsCMSEntities db = new PowerAppsCMSEntities())
            {
                Guid guidUserID = Guid.Parse(userID.Replace(" ", string.Empty));
                List<ProductionPlanningUnitDetail> listOfProductionPlanningUnitDetail = new List<ProductionPlanningUnitDetail>();
                try
                {
                    List<UserProcessGroup> listProcessGroup = db.UserProcessGroups.Where(x => x.UserID == guidUserID).ToList();
                    foreach (Process processUnitList in db.Processes.Where(x => x.MasterProcess.ProductID == productID && x.UnitID == unitID).ToList())
                    {
                        ProductionPlanningUnitDetail productionPlanningUnitDetail = new ProductionPlanningUnitDetail();
                        productionPlanningUnitDetail.ProcessID = processUnitList.ID;
                        productionPlanningUnitDetail.ProcessGroupID = processUnitList.MasterProcess.ProcessGroupID;

                        if (listProcessGroup.Where(x => x.ProcessGroupID == productionPlanningUnitDetail.ProcessGroupID).FirstOrDefault() != null)
                        {
                            productionPlanningUnitDetail.IsCanAccess = true;
                        }
                        else
                        {
                            productionPlanningUnitDetail.IsCanAccess = false;
                        }

                        productionPlanningUnitDetail.StatusID = processUnitList.Status;

                        if (productionPlanningUnitDetail.StatusID == (int)ProcessStatus.NotStarted)
                        {
                            productionPlanningUnitDetail.StatusName = "Not Started";
                        }
                        else if (productionPlanningUnitDetail.StatusID == (int)ProcessStatus.OnProcess)
                        {
                            productionPlanningUnitDetail.StatusName = "On Process";
                        }
                        else if (productionPlanningUnitDetail.StatusID == (int)ProcessStatus.OnProcess && processUnitList.PlanEndDate < DateTime.Now)
                        {
                            productionPlanningUnitDetail.StatusName = "On Process (Late)";
                        }
                        else if (productionPlanningUnitDetail.StatusID == (int)ProcessStatus.StopByOperator)
                        {
                            productionPlanningUnitDetail.StatusName = "Stopped By Operator";
                        }
                        else if (productionPlanningUnitDetail.StatusID == (int)ProcessStatus.WaitForQC)
                        {
                            productionPlanningUnitDetail.StatusName = "Wait For QC";
                        }
                        else if (productionPlanningUnitDetail.StatusID == (int)ProcessStatus.QCPassed)
                        {
                            productionPlanningUnitDetail.StatusName = "QC Passed";
                        }
                        else if (productionPlanningUnitDetail.StatusID == (int)ProcessStatus.QCNotGood)
                        {
                            productionPlanningUnitDetail.StatusName = "QC Not Good";
                        }
                        else if (productionPlanningUnitDetail.StatusID == (int)ProcessStatus.Finish)
                        {
                            productionPlanningUnitDetail.StatusName = "Finish";
                        }

                        productionPlanningUnitDetail.ProcessName = processUnitList.MasterProcess.Name;
                        productionPlanningUnitDetail.ManHour = processUnitList.MasterProcess.ManHour;
                        productionPlanningUnitDetail.ManPower = processUnitList.MasterProcess.ManPower;

                        decimal? totalMHActual = processUnitList.ProcessAssigns.Sum(x => x.ProcessActivities.Sum(y => y.ActualHours));// db.ProcessActivities.Where(x => x.ProcessAssign.ProcessID == processUnitList.ID).Sum(x => x.ActualHours);

                        productionPlanningUnitDetail.ManHourActual = totalMHActual;
                        productionPlanningUnitDetail.MasterProcessID = processUnitList.MasterProcessID;

                        listOfProductionPlanningUnitDetail.Add(productionPlanningUnitDetail);
                    }
                    return Ok(listOfProductionPlanningUnitDetail);
                }
                catch (Exception ex)
                {
                    return InternalServerError(ex);
                }
            }
        }


        /// <summary>
        /// Mengambil semua daftar operator yang ditugaskan pada suatu process
        /// </summary>
        /// <param name="processID"></param>
        /// <returns>Daftar operator yang ditugaskan pada suatu process</returns>
        [Route("api/GetListAddedOperator")]
        [HttpGet]
        [SwaggerResponse(HttpStatusCode.OK,
            Description = "Get specific production planning",
            Type = typeof(ListAddedOperator))]
        [SwaggerResponse(HttpStatusCode.InternalServerError,
                Description = "Internal Server Error",
               Type = typeof(Exception))]
        [SwaggerOperation("GetListAddedOperator")]

        public IHttpActionResult GetListAddedOperator(int processID)
        {
            using (PowerAppsCMSEntities db = new PowerAppsCMSEntities())
            {
                List<ListAddedOperator> listAddedOperator = new List<ListAddedOperator>();

                try
                {
                    foreach (ProcessAssign itemProcessAssign in db.ProcessAssigns.Where(x => x.ProcessID == processID))
                    {
                        ListAddedOperator addedOperator = new ListAddedOperator();
                        addedOperator.ID = itemProcessAssign.ID;
                        addedOperator.UserID = itemProcessAssign.User.ID;
                        addedOperator.Name = itemProcessAssign.User.Name;
                        addedOperator.Type = itemProcessAssign.Type;
                        addedOperator.StatusID = itemProcessAssign.Status;
                        var statusID = addedOperator.StatusID;
                        if (statusID == 0)
                        {
                            addedOperator.StatusName = "Not Started";
                        }
                        else if (statusID == 1)
                        {
                            addedOperator.StatusName = "Started";
                        }
                        else if (statusID == 2)
                        {
                            addedOperator.StatusName = "Pause";
                        }
                        else if (statusID == 3)
                        {
                            addedOperator.StatusName = "Break";
                        }
                        else if (statusID == 4)
                        {
                            addedOperator.StatusName = "Stop";
                        }
                        else if (statusID == 5)
                        {
                            addedOperator.StatusName = "Stop by Group Leader";
                        }

                        listAddedOperator.Add(addedOperator);
                    }
                    return Ok(listAddedOperator);
                }
                catch (Exception ex)
                {
                    return InternalServerError(ex);
                }
            }
        }

        /// <summary>
        /// Mengambil semua daftar list issue yang ada pada process yang dipilih
        /// </summary>
        /// <param name="processID"></param>
        /// <returns>Daftar issue yang ada pada process yang dipilih</returns>
        [Route("api/GetProcessIssue")]
        [HttpGet]
        [SwaggerResponse(HttpStatusCode.OK,
            Description = "Get process issue",
            Type = typeof(ListProcessIssue))]
        [SwaggerResponse(HttpStatusCode.InternalServerError,
                Description = "Internal Server Error",
               Type = typeof(Exception))]
        [SwaggerOperation("GetProcessIssue")]

        public IHttpActionResult GetProcessIssue(int processID)
        {
            using (PowerAppsCMSEntities db = new PowerAppsCMSEntities())
            {
                List<ListProcessIssue> listProcessIssue = new List<ListProcessIssue>();
                try
                {
                    foreach (ProcessIssue itemProcessIssue in db.ProcessIssues.Where(x => x.ProcessID == processID))
                    {
                        ListProcessIssue processIssue = new ListProcessIssue();
                        processIssue.ProcessIssueID = itemProcessIssue.ID;
                        processIssue.ReasonIssueID = itemProcessIssue.ReasonIssueID;
                        processIssue.IssueName = itemProcessIssue.ReasonIssue.Name;
                        processIssue.Status = itemProcessIssue.Status;
                        var status = processIssue.Status;
                        if (status)
                        {
                            processIssue.FinishDate = itemProcessIssue.LastModified.ToString("dd/MM/yyyy");
                        }
                        else
                        {
                            processIssue.FinishDate = null;
                        }

                        processIssue.StartDate = itemProcessIssue.Created.ToString("dd/MM/yyyy");
                        listProcessIssue.Add(processIssue);
                    }
                    return Ok(listProcessIssue);
                }
                catch (Exception ex)
                {
                    return InternalServerError(ex);
                }
            }
        }

        /// <summary>
        /// Menambahkan operator yang akan ditugaskan pada process yang dipilih 
        /// </summary>
        /// <param name="processID"></param>
        /// <param name="userID"></param>
        /// <param name="type"></param>
        /// <param name="userName"></param>
        /// <returns></returns>
        [Route("api/PostAddedOperator")]
        [HttpPost]
        public IHttpActionResult PostAddedOperator(int processID, string userID, int type, string userName)
        {
            try
            {
                using (PowerAppsCMSEntities db = new PowerAppsCMSEntities())
                {
                    Guid guidUserID = Guid.Parse(userID.Replace(" ", string.Empty));
                    DateTime now = DateTime.Now;

                    ProcessAssign processAssign = new ProcessAssign();
                    processAssign.ProcessID = processID;
                    processAssign.UserID = guidUserID;
                    processAssign.Status = (int)ProcessAssignStatus.NotStarted;
                    processAssign.Type = type;
                    processAssign.Created = processAssign.LastModified = now;
                    processAssign.CreatedBy = processAssign.LastModifiedBy = userName;
                    db.ProcessAssigns.Add(processAssign);

                    if (db.SaveChanges() > 0)
                    {
                        User user = db.Users.Find(guidUserID);
                        user.IsAssign = true;
                        db.SaveChanges();
                    }
                    return Ok(HttpStatusCode.OK);
                }
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        /// <summary>
        /// Fungsi untuk group leader, untuk menghentikan process kerja operator. Digunakan apabila 
        /// operator mengalami kecelakaan atau ada urgensi lain
        /// </summary>
        /// <param name="processAssignID"></param>
        /// <param name="userName"></param>
        /// <param name="userID"></param>
        /// <returns></returns>
        [Route("api/ForceStop")]
        [HttpPost]
        public IHttpActionResult ForceStop(int processAssignID, string userName, string userID)
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
                        processStatusLogs.Description = "Force Stop";
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

        [Route("api/DeleteProcessAssign")]
        [HttpPost]
        public IHttpActionResult DeleteProcessAssign(int processAssignID, string userID)
        {
            try
            {
                using (PowerAppsCMSEntities db = new PowerAppsCMSEntities())
                {
                    Guid guidUserID = Guid.Parse(userID.Replace(" ", string.Empty));
                    DateTime now = DateTime.Now;

                    ProcessAssign selectedProcessAssign = db.ProcessAssigns.Where(x => x.ID == processAssignID).SingleOrDefault();
                    db.ProcessAssigns.Remove(selectedProcessAssign);

                    if (db.SaveChanges() > 0)
                    {
                        User selectedUser = db.Users.Where(x => x.ID == guidUserID).SingleOrDefault();
                        selectedUser.IsAssign = false;
                        db.SaveChanges();
                    }
                    return Ok(HttpStatusCode.OK);
                }
            }
            catch(Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [Route("api/WaitForQC")]
        [HttpPost]
        public IHttpActionResult WaitForQC(int processID, string userName)
        {
            try
            {
                using (PowerAppsCMSEntities db = new PowerAppsCMSEntities())
                {
                    DateTime now = DateTime.Now;

                    Process selectedProcess = db.Processes.Where(x => x.ID == processID).SingleOrDefault();
                    selectedProcess.Status = (int)ProcessStatus.WaitForQC;
                    selectedProcess.LastModified = now;
                    selectedProcess.LastModifiedBy = userName;

                    if(db.SaveChanges() > 0)
                    {
                        ProcessStatusLogs processStatusLogs = new ProcessStatusLogs();
                        processStatusLogs.ProcessID = processID;
                        processStatusLogs.Description = "Wait for QC";
                        processStatusLogs.Status = (int)ProcessStatus.WaitForQC;
                        processStatusLogs.StatusName = "Wait for QC";
                        processStatusLogs.CreatedBy = userName;
                        processStatusLogs.Created = now;
                        db.ProcessStatusLogs.Add(processStatusLogs);
                        db.SaveChanges();
                    }
                    return Ok(HttpStatusCode.OK);
                }
            }
            catch(Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [Route("api/DeleteAssignOperator")]
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
         
        }
    }
}
