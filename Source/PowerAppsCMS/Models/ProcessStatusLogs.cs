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
    
    public partial class ProcessStatusLogs
    {
        public int ID { get; set; }
        public Nullable<int> ProcessID { get; set; }
        public string Description { get; set; }
        public Nullable<int> Status { get; set; }
        public string StatusName { get; set; }
        public string CreatedBy { get; set; }
        public Nullable<System.DateTime> Created { get; set; }
    }
}
