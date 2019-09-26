using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace PowerAppsCMS.Models
{
    /// <summary>
    /// Partial class model material
    /// </summary>
    [MetadataType(typeof(MaterialDataAnotation))]
    public partial class Material
    {
    }

    /// <summary>
    /// Data anotation untuk model material
    /// </summary>
    public class MaterialDataAnotation
    {
        [Required(ErrorMessage = "Material is required")]
        public string Name { get; set; }
    }
}