using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PowerAppsCMS.Models
{
    /// <summary>
    /// merupakan partial class dari model MasterPlanSchedule
    /// </summary>
    public partial class MasterPlanSchedule
    {
        /// <summary>
        /// Merupakan atribut yang mengembalikan sebuah bilangan bulat yang menandakan jumlah plan yang sudah di assign pada sebuah MPS
        /// </summary>
        public int AssignedPlanCount
        {
            get
            {
                if(this.Units.Count > 0)
                {
                    return this.Units.Where(x => x.IsHaveProcessAssign == true).Count();
                }
                else
                {
                    return 0;
                }                
            }
        }

        /// <summary>
        /// Merupakan atribut bertipe integer yang mengembalikan nilai jumlah quantity MPS yang tidak di hold
        /// </summary>
        public int CurrentPlannedQuantity
        {
            get
            {
                if (this.Units.Count != 0 && this.Units.Where(x=>x.IsHold==false).Count() != this.PlannedQuantity)
                {
                    return this.Units.Where(x => x.IsHold == false).Count();
                }
                else
                {
                    return this.PlannedQuantity;
                }
            }
        }

        /// <summary>
        /// merupakan atribut yang menampilkan jumlah Quantity dari MPS yang dihold
        /// </summary>
        public int HoldUnitQuantity
        {
            get
            {
                if (this.Units.Count != 0)
                {
                    return this.Units.Where(x => x.IsHold == true).Count();
                }
                else
                {
                    return 0;
                }
            }
        }
    }
}