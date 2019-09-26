using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PowerAppsCMS.Models
{
    public class ListAssignedOperator
    {
        public string NRP { get; set; }
        public string UserName { get; set; }
        public int StatusID { get; set; }
        public string StatusName { get; set; }
        public string PRO { get; set; }
        public string SN { get; set; }
        public string ProcessName { get; set; }
        public string ProductName { get; set; }
        public string AssignBy { get; set; }
    }
}