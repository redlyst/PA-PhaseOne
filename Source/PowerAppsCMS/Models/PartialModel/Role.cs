using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace PowerAppsCMS.Models
{
    /// <summary>
    /// Partial class model role
    /// </summary>
    [MetadataType(typeof(RoleDataAnotation))]
    public partial class Role
    {
    }

    /// <summary>
    /// Data anotaion role
    /// </summary>
    public class RoleDataAnotation
    {
        [Required(ErrorMessage = "Role is required")]
        public string Name { get; set; }
    }
}