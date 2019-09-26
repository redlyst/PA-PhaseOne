using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PowerAppsCMS.Models
{
    /// <summary>
    /// Merupakan partial class model PRO
    /// </summary>
    public partial class PRO
    {
        /// <summary>
        /// Menampilkan data tanggal DueDate dari sebuah PRO
        /// </summary>
        public string DueDateDisplayText
        {
            get
            {
                return this.DueDate.ToString("dd/MM/yyyy");
            }
        }

        /// <summary>
        /// Atribut yang menampilkan daftar nama SONumber yang dimapping pada PRO
        /// </summary>
        public string SONumberListDisplayText
        {
            get
            {
                return string.Join(", ", this.PROSalesOrders.Select(x => x.SalesOrder.Number).Distinct().ToList());
            }
        }

        /// <summary>
        /// Atribut yang menampilkan daftar nama customer dari PRO
        /// </summary>
        public string CustomerListInSODisplayText
        {
            get
            {
                return string.Join(", ", this.PROSalesOrders.Select(x => x.SalesOrder.CustomerName).Distinct().ToList());
            }
        }

        /// <summary>
        /// Atribut yang menampilkan jumlah unit yang tidak dihold dalam pro tersebut
        /// </summary>
        public int UnHoldUnitCount
        {
            get
            {
                return this.Units.Where(x => x.IsHold == false).Count();
            }
        }


        /// <summary>
        /// atribute yang menyimpan informasi apakah sebuah pro boleh di edit atau tidak
        /// </summary>
        public bool ItCanEdit
        {
            get
            {
                bool canEdit = true;

                if(this.Products.TotalDay == null)
                {
                    canEdit = false;
                }
                return canEdit; 
            }
        }

        /// <summary>
        /// atribut yang menyimpan informasi apakah PRO tersebut memiliki warning terhadap mps due date atau tidak
        /// </summary>
        public bool HaveWarningMPSDuedate
        {
            get
            {
                bool haveWarning = false;
                foreach (Unit item in this.Units)
                {
                    if (item.MPSDueDate < item.SFSDueDate && !item.IsHold)
                    {
                        haveWarning = true;
                    }
                }
                return haveWarning;
            }
        }

        /// <summary>
        /// atribut yang menyimpan informasi css class yang akan ditampilkan dihalaman MPS untuk PRO tersebut
        /// </summary>
        public string PROCategoryCSSClass
        {
            get
            {
                string cssClass = "mps-warning";
                int unitCount = this.Units.Count();
                int holdUnitCount = this.Units.Where(x => x.IsHold).Count();

                if (unitCount == holdUnitCount)
                {
                    cssClass = "mps-deactived";
                }
                else if (this.MasterPlanSchedules.Count() == 0 && this.DueDate.Date < DateTime.Now.Date)
                {
                    cssClass = "mps-danger";
                }
                else if (this.MasterPlanSchedules.Count() > 0)
                {
                    if(this.Units.Where(x=> x.IsHold != true && x.MPSDueDate == null).Count() > 0)
                    {
                        cssClass = "mps-working";
                    }
                    else
                    {
                        cssClass = "";
                    }
                }
                return cssClass;
            }
        }

        /// <summary>
        /// atribut tambahan yang akan digunakan untuk menyimpan nilai minimum due date yang bisa dimiliki sebuah PRO
        /// </summary>
        public DateTime MinimumDueDate
        {
            get;set;
        }

        /// <summary>
        /// atribut yang akan digunakan untuk menyimpan data daftar MPS Actual dari PRO tersebut
        /// </summary>
        public List<MPSActual> MPSActualList
        {
            get;set;
        }

        /// <summary>
        /// atribut yang akan digunakan untuk menyimpan informasi nilai carryover untuk current month dari pro 
        /// </summary>
        public int CurrentMonthCarryOver
        {
            get; set;
        }

        /// <summary>
        /// atribut yang akan digunakan untuk menyimpan informasi nilai carryover untuk satu bulan sebalun current month dari pro 
        /// </summary>
        public int LastMonthCarryOver
        {
            get; set;
        }

        /// <summary>
        /// atribut yang akan digunakan untuk menyimpan informasi nilai Maximal dari Plan Quantity yang dimiliki PRO 
        /// </summary>
        public int MaximumPlanQuantity
        {
            get; set;
        }
    }
}