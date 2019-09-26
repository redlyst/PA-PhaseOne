using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace PowerAppsCMS.Models
{
    /// <summary>
    /// Partial class model process group
    /// </summary>
    [MetadataType(typeof(ProcessGroupDataAnotation))]
    public partial class ProcessGroup
    {
    }

    /// <summary>
    /// Data anotation process group
    /// </summary>
    public class ProcessGroupDataAnotation
    {
        [Required(ErrorMessage = "Process group is required")]
        public string Name { get; set; }
    }
}