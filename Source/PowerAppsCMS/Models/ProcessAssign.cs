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
    
    public partial class ProcessAssign
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public ProcessAssign()
        {
            this.ProcessActivities = new HashSet<ProcessActivity>();
        }
    
        public int ID { get; set; }
        public int ProcessID { get; set; }
        public System.Guid UserID { get; set; }
        public int Status { get; set; }
        public Nullable<int> Type { get; set; }
        public System.DateTime Created { get; set; }
        public string CreatedBy { get; set; }
        public System.DateTime LastModified { get; set; }
        public string LastModifiedBy { get; set; }
    
        public virtual Process Process { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ProcessActivity> ProcessActivities { get; set; }
        public virtual User User { get; set; }
    }
}
