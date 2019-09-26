using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace PowerAppsCMS.Models
{
    /// <summary>
    /// Partial class model memo
    /// </summary>
    [MetadataType(typeof(MemoDataAnotation))]
    public partial class Memo
    {
    }

    /// <summary>
    /// Data anotation untuk model memo
    /// </summary>
    public class MemoDataAnotation
    {
        [Required(ErrorMessage ="Type must be selected")]
        public int MemoTypeID { get; set; }
        [Required(ErrorMessage = "Product must be selected")]
        public int ProductID { get; set; }
    }
}