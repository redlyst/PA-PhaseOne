using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PowerAppsCMS.Models;

namespace PowerAppsCMS.ViewModel
{
    public class MappingSalesOrderViewModel
    {
        public SalesOrder SalesOrder { get; set; }
        public Products Products { get; set; }
        public List<Unit> Units { get; set; }

        public int[] selectedUnitID { get; set; }
        public List<Unit> MappedUnits { get; set; }
    }

    public class CurrentMappingSalesOrder
    {
        //private List<Unit> actualTempUnits = new List<Unit>();
        //private List<Unit> deletedTempUnits  = new List<Unit>();
        //private List<Unit> additionalTempUnits = new List<Unit>();

        //public CurrentMappingSalesOrder()
        //{ }

        //public CurrentMappingSalesOrder(List<Unit> ActualUnit) //, List<Unit> DeletedUnit, List<Unit> AdditionalUnit
        //{
            //ActualTempUnits = ActualUnit;
            //DeletedTempUnits = null;
            //AdditionalTempUnits = null;
        //}

        public List<Unit> ActualTempUnits { get; set; }
        public List<Unit> DeletedTempUnits { get; set; }
        public List<Unit> AdditionalTempUnits { get; set; }
    }
}