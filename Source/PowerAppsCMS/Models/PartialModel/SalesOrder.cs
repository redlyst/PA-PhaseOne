using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using PowerAppsCMS.Models;

namespace PowerAppsCMS.Models
{
    /// <summary>
    /// Partial class model sales order
    /// </summary>
    [MetadataType(typeof(SalesOrderDataAnotation))]
    public partial class SalesOrder
    {
    }

    /// <summary>
    /// Data anotation sales order
    /// </summary>
    public class SalesOrderDataAnotation
    {
        [DisplayFormat(DataFormatString = "{0:N2}")]
        [DataType(DataType.Currency)]
        public decimal TotalPrice { get; set; }
    }
}