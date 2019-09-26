//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace PowerAppsCMS.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class MasterProcess
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public MasterProcess()
        {
            this.ProcessDependencies = new HashSet<ProcessDependency>();
            this.PredecessorProcessDependencies = new HashSet<ProcessDependency>();
            this.ProcessDailySchedules = new HashSet<ProcessDailySchedule>();
            this.Processes = new HashSet<Process>();
        }
    
        public int ID { get; set; }
        public int ProductID { get; set; }
        public int ProcessGroupID { get; set; }
        public string Name { get; set; }
        public int ProcessOrder { get; set; }
        public decimal ManHour { get; set; }
        public int ManPower { get; set; }
        public decimal CycleTime { get; set; }
        public System.DateTime Created { get; set; }
        public string CreatedBy { get; set; }
        public System.DateTime LastModified { get; set; }
        public string LastModifiedBy { get; set; }
    
        public virtual ProcessGroup ProcessGroup { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ProcessDependency> ProcessDependencies { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ProcessDependency> PredecessorProcessDependencies { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ProcessDailySchedule> ProcessDailySchedules { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Process> Processes { get; set; }
        public virtual Products Products { get; set; }
    }
}