using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using PowerAppsCMS.Constants;
using PowerAppsCMS.CustomAuthentication;
using PowerAppsCMS.Models;

namespace PowerAppsCMS.Controllers
{
    [CustomAuthorize(Roles = RoleNames.SuperAdmin)]
    public class ProcessController : Controller
    {
        private PowerAppsCMSEntities db = new PowerAppsCMSEntities();

        // GET: Process
        public ActionResult ChangeQCStatus(int? id)
        {
            Process processData = new Process();

            if(id != null)
            {
                processData = db.Processes.Find(id);
            }
            else
            {
                processData = db.Processes.Find(0);
            }

            return View(processData);
        }


        // GET: Process
        public ActionResult UpdateStatus(int id)
        {
            Process processData = db.Processes.Find(id);

            if (processData != null)
            {
                processData = db.Processes.Find(id);

                if (processData.Status == (int)Constants.ProcessStatus.QCPassed)
                {
                    DateTime now = DateTime.Now;
                    ChangeQCStatusLog logData = new ChangeQCStatusLog();
                    logData.Description = "Change Process Status From QC Passed to Wating for QC";
                    logData.CreatedBy = User.Identity.Name;
                    logData.Created = now;

                    processData.ActualEndDate = null;
                    processData.Status = (int)Constants.ProcessStatus.WaitForQC;
                    processData.LastModified = now;
                    processData.LastModifiedBy = User.Identity.Name;
                    processData.ChangeQCStatusLog.Add(logData);

                    if (db.SaveChanges() > 0)
                    {
                        return View(processData);
                    }
                    else
                    {
                        return View("Error");
                    }
                }
                else
                {
                    return View("Error");
                }
            }
            return RedirectToAction("ChangeQCStatus", "Process");
        }
    }
}
