using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using PowerAppsCMS.Models;

namespace PowerAppsCMS.ViewModel
{
    /// <summary>
    /// Merupakan Class View Model Master Process yang digunakan untuk menampilkan dan mengelola data master process di modul CRUD. 
    /// Atribut yang digunakan hampir mirip dengan atribut model master process
    /// </summary>
    public class MasterProcessViewModel
    {
        public int ID { get; set; }
        public int ProductID { get; set; }
        public int ProcessGroupID { get; set; }
        [Required(ErrorMessage = "Process Name is required")]
        public string Name { get; set; }
        public int ProcessOrder { get; set; }
        [Required(ErrorMessage = "Man Hour is required")]
        public decimal ManHour { get; set; }
        [Required(ErrorMessage = "Man Power is required")]
        public int ManPower { get; set; }
        [Required(ErrorMessage = "Cycle Time is required")]
        public decimal CycleTime { get; set; }
        public List<MasterProcess> MasterProcessCollections { get; set; }
        public List<int> SelectedProcess { get; set; }
        public List<int> CurrentSelectedProcess { get; set; }

        public MasterProcessViewModel()
        {
            CurrentSelectedProcess = new List<int>();
            SelectedProcess = new List<int>();
            MasterProcessCollections = new List<MasterProcess>();
        }
    }
}