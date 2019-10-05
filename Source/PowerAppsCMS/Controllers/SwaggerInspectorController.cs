using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Http;
using PowerAppsCMS.Models;
using PowerAppsCMS.Constants;
using Newtonsoft.Json;
using Swashbuckle.Swagger.Annotations;

namespace PowerAppsCMS.Controllers
{
    /// <summary>
    /// Controller yang berisikan fungsi-fungsi yang digunakan dihalaman Inspektor
    /// </summary>
    public class SwaggerInspectorController : ApiController
    {
        /// <summary>
        /// Menampilkan unit yang memiliki status process siap untuk dilakukan pengecekan QC
        /// </summary>
        /// <returns>Daftar unit yang memiliki status process siap untuk dilakukan pengecekan QC</returns>
        [Route("api/GetInspectorUnitList")]
        [HttpGet]
        [SwaggerResponse(HttpStatusCode.OK, Description = "Get inspector unitlist", Type = typeof(ProductionPlanning))]
        [SwaggerResponse(HttpStatusCode.InternalServerError, Description = "Internal Server Eror", Type = typeof(Exception))]
        [SwaggerOperation("GetInspectorUnitList")]

        public IHttpActionResult GetInspectorUnitList()
        {
            using (PowerAppsCMSEntities db = new PowerAppsCMSEntities())
            {
                List<ProductionPlanning> listInspectorUnitList = new List<ProductionPlanning>();
                try
                {
                    var itemProcess = db.Processes.Where(x => x.Unit.IsHold == false && x.Unit.SFSDueDate <= x.Unit.MPSDueDate && x.Status == 4).Select(x => x.Unit).Distinct();
                    foreach (Unit itemUnit in itemProcess)
                    {
                        ProductionPlanning inspectorUnitList = new ProductionPlanning();
                        inspectorUnitList.UnitID = itemUnit.ID;
                        inspectorUnitList.SerialNumber = itemUnit.SerialNumber;
                        inspectorUnitList.PRO = itemUnit.PRO.Number;
                        inspectorUnitList.Product = itemUnit.Product.Name;
                        inspectorUnitList.Customer = itemUnit.PRO.CustomerListInSODisplayText;// Customer;
                        inspectorUnitList.ProductID = itemUnit.ProductID;

                        listInspectorUnitList.Add(inspectorUnitList);
                    }
                    return Ok(listInspectorUnitList);
                }
                catch (Exception ex)
                {
                    return InternalServerError(ex);
                }
            }
        }

        /// <summary>
        /// Menampilkan process dari unit yang dipilih
        /// </summary>
        /// <param name="productID"></param>
        /// <param name="unitID"></param>
        /// <returns>Daftar process dari unit yang dipilih</returns>
        [Route("api/GetInspectorUnitDetail")]
        [HttpGet]
        [SwaggerResponse(HttpStatusCode.OK, Description = "Get inspector unit detail", Type = typeof(ProductionPlanningUnitDetail))]
        [SwaggerResponse(HttpStatusCode.InternalServerError, Description = "Internal Server Error", Type = typeof(Exception))]
        [SwaggerOperation("GetInspectorUnitDetail")]

        public IHttpActionResult GetInspectorUnitDetail(int productID, int unitID)
        {
            using (PowerAppsCMSEntities db = new PowerAppsCMSEntities())
            {
                List<ProductionPlanningUnitDetail> listInspectorUnitDetail = new List<ProductionPlanningUnitDetail>();
                try
                {
                    foreach (Process itemProcess in db.Processes.Where(x => x.MasterProcess.ProductID == productID && x.UnitID == unitID))
                    {
                        ProductionPlanningUnitDetail inspectorPlanningUnitDetail = new ProductionPlanningUnitDetail();
                        inspectorPlanningUnitDetail.ProcessID = itemProcess.ID;
                        inspectorPlanningUnitDetail.ProcessGroupID = itemProcess.MasterProcess.ProcessGroupID;
                        inspectorPlanningUnitDetail.StatusID = itemProcess.Status;
                        var statusID = inspectorPlanningUnitDetail.StatusID;
                        if (statusID == (int)ProcessStatus.NotStarted)
                        {
                            inspectorPlanningUnitDetail.StatusName = "Not Started";
                        }
                        else if (statusID == (int)ProcessStatus.OnProcess)
                        {
                            inspectorPlanningUnitDetail.StatusName = "On Process";
                        }
                        else if (statusID == (int)ProcessStatus.ONProcessLate)
                        {
                            inspectorPlanningUnitDetail.StatusName = "On Process (Late)";
                        }
                        else if (statusID == (int)ProcessStatus.StopByOperator)
                        {
                            inspectorPlanningUnitDetail.StatusName = "Stopped By Operator";
                        }
                        else if (statusID == (int)ProcessStatus.WaitForQC)
                        {
                            inspectorPlanningUnitDetail.StatusName = "Wait For QC";
                        }
                        else if (statusID == (int)ProcessStatus.QCPassed)
                        {
                            inspectorPlanningUnitDetail.StatusName = "QC Passed";
                        }
                        else if (statusID == (int)ProcessStatus.QCNotGood)
                        {
                            inspectorPlanningUnitDetail.StatusName = "QC Not Good";
                        }
                        else if (statusID == (int)ProcessStatus.Finish)
                        {

                            inspectorPlanningUnitDetail.StatusName = "Finish";
                        }

                        inspectorPlanningUnitDetail.ProcessName = itemProcess.MasterProcess.Name;
                        inspectorPlanningUnitDetail.ManHour = itemProcess.MasterProcess.ManHour;
                        inspectorPlanningUnitDetail.ManPower = itemProcess.MasterProcess.ManPower;
                        inspectorPlanningUnitDetail.MasterProcessID = itemProcess.MasterProcessID;
                        inspectorPlanningUnitDetail.LastModifiedby = itemProcess.LastModifiedBy;
                        listInspectorUnitDetail.Add(inspectorPlanningUnitDetail);
                    }
                    return Ok(listInspectorUnitDetail);
                }
                catch (Exception ex)
                {
                    return InternalServerError(ex);
                }
            }

        }

        /// <summary>
        /// Fungsi untuk mengubah status process menjadi QC Good, dan menghitung
        /// apakah semua process pada unit tersebut sudah QC Good, apabila suda maka statusnya menjadi Finishs
        /// </summary>
        /// <param name="productID"></param>
        /// <param name="unitID"></param>
        /// <param name="processID"></param>
        /// <param name="userName"></param>
        /// <returns></returns>
        [Route("api/QCCheck")]
        [HttpPost]
        public IHttpActionResult QCCheck(int productID, int unitID, int processID, string userName)
        {
            using (PowerAppsCMSEntities db = new PowerAppsCMSEntities())
            {
                DateTime now = DateTime.Now;

                //Update Status dan ActualEndDate process
                Process selectedProcess = db.Processes.Where(x => x.ID == processID).SingleOrDefault();
                selectedProcess.Status = (int)ProcessStatus.QCPassed;
                if (selectedProcess.MasterProcess.ProcessGroupID == 3)
                {
                    selectedProcess.ActualStartDate = selectedProcess.ActualEndDate = selectedProcess.LastModified = now;
                }
                selectedProcess.ActualEndDate = selectedProcess.LastModified = now;
                selectedProcess.LastModifiedBy = userName;
                db.SaveChanges();

                List<Process> selectedProcesslist = db.Processes.Where(x => x.MasterProcess.ProductID == productID && x.UnitID == unitID).ToList();
                if (selectedProcesslist.Where(x => x.Status != (int)ProcessStatus.QCPassed).Count() == 0)
                {
                    //Update ActualDeliveryDate unit
                    Unit selectedUnit = db.Units.Where(x => x.ID == unitID).SingleOrDefault();
                    selectedUnit.ActualDeliveryDate = selectedUnit.LastModified = now;
                    selectedUnit.LastModifiedBy = userName;

                    if (db.SaveChanges() > 0)
                    {
                        foreach (Process item in selectedProcesslist)
                        {
                            //Update Status process menjadi finish ketika semua process sudah QC Pass
                            item.Status = (int)ProcessStatus.Finish;
                            item.LastModified = now;
                            item.LastModifiedBy = userName;
                            if (db.SaveChanges() > 0)
                            {
                                ProcessStatusLogs processStatusLogs = new ProcessStatusLogs();
                                processStatusLogs.ProcessID = processID;
                                processStatusLogs.Description = "Finish";
                                processStatusLogs.Status = (int)ProcessStatus.Finish;
                                processStatusLogs.StatusName = "Finish";
                                processStatusLogs.CreatedBy = userName;
                                processStatusLogs.Created = now;
                                db.ProcessStatusLogs.Add(processStatusLogs);
                                db.SaveChanges();
                            }
                        }
                    }
                }
                return Ok(HttpStatusCode.OK);
            }
        }

        [Route("api/QCNotGood")]
        [HttpPost]
        public IHttpActionResult QCNotGood(int processID, string userName)
        {
            try
            {
                using (PowerAppsCMSEntities db = new PowerAppsCMSEntities())
                {
                    DateTime now = DateTime.Now;

                    Process selectedProcess = db.Processes.Where(x => x.ID == processID).SingleOrDefault();
                    selectedProcess.Status = (int)ProcessStatus.QCNotGood;
                    selectedProcess.LastModified = now;
                    selectedProcess.LastModifiedBy = userName;

                    if(db.SaveChanges() > 0)
                    {
                        ProcessStatusLogs processStatusLogs = new ProcessStatusLogs();
                        processStatusLogs.ProcessID = processID;
                        processStatusLogs.Description = "QC Not Good";
                        processStatusLogs.Status = (int)ProcessStatus.QCNotGood;
                        processStatusLogs.StatusName = "QC Not Good";
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
    }
}