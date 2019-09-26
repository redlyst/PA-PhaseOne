using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PowerAppsCMS.Models;

namespace PowerAppsCMS.ViewModel
{
    /// <summary>
    /// View model dari MPSMapping yang berisikan object SalesOrder, Product, List object ProCollections, ListOfPROSalesOrder,
    /// dan integer array SelectedPROID
    /// </summary>
    public class MPSMappingViewModel
    {
        public SalesOrder SalesOrder { get; set; }
        public List<PRO> ProCollections { get; set; }
        public int[] SelectedPROID { get; set; }
        public List<PROSalesOrder> ListOfPROSalesOrder { get; set; }
        public Products Product { get; set; }
    }
}