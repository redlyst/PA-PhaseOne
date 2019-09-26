using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PowerAppsCMS.Models;

namespace PowerAppsCMS.ViewModel
{
    /// <summary>
    /// View model dari product group yang berisi object ProductGroup, object list ProductSubGroups, dan object list ProductGroupCapacities
    /// </summary>
    public class GroupProductViewModel
    {
        public ProductGroup ProductGroup { get; set; }
        public List<ProductSubGroup> ProductSubGroups { get; set; }
        public List<ProductGroupCapacity> ProductGroupCapacities { get; set; }
    }
}