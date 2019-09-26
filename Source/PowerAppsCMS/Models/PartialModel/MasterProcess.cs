using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace PowerAppsCMS.Models
{
    /// <summary>
    /// Merupakan partial class model dari MasterProcess yang berisikan atribut-atribut tambahan yang tidak ada dalam database tetapi bisa diperoleh dari hasil perhitungan dari atribut lainya yang ada
    /// </summary>
    [MetadataType(typeof(MasterProcessDataAnotation))]
    public partial class MasterProcess
    {
        /// <summary>
        /// Merupakan atribut yang menandakan apakah sebuah MasterProcess boleh didelete
        /// </summary>
        public bool CanDeleted
        {
            get
            {
                return (this.Processes.Count == 0);
            }
        }

        /// <summary>
        /// Atribut bertipe decimal yang menampilkan jumlah MaxDay yang digunaan dalam sabuah master proses
        /// </summary>
        public decimal MaxDayUsed
        {
            get
            {
                if (this.ProcessDailySchedules.Count > 0)
                {
                    ProcessDailySchedule data = this.ProcessDailySchedules.OrderByDescending(x => x.Day).ThenByDescending(x => x.UsedDay).FirstOrDefault();
                    if (data != null)
                    {
                        if (data.UsedDay < 1)
                        {
                            return (data.Day - 1 + data.UsedDay);
                        }
                        else
                        {
                            return data.Day;
                        }
                    } 
                }
                return 0;
            }
        }

        /// <summary>
        /// atribut yang menampilkan jumlah cycletime + satuannya sebuah master process
        /// </summary>
        public string CycleTimeDisplayText
        {
            get
            {               
                return this.CycleTime + " Days";
            }
        }
    }

    /// <summary>
    /// Data anotation untuk model component
    /// </summary>
    public class MasterProcessDataAnotation
    {
        [Required (ErrorMessage ="Process Name is required")]
        public string Name { get; set; }
        public int ProcessOrder { get; set; }
        [Required(ErrorMessage = "Man Hour is required")]
        public decimal ManHour { get; set; }
        [Required(ErrorMessage = "Man Power is required")]
        public int ManPower { get; set; }
        [Required(ErrorMessage = "Cycle Time is required")]
        public decimal CycleTime { get; set; }
    }
}