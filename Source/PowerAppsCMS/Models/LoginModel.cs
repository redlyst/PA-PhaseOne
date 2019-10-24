using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PowerAppsCMS.Models
{
    /// <summary>
    /// Class model yang menyimpan informasi user powerapps
    /// </summary>
    public class LoginModel
    {
        /// <summary>
        ///  Login Status, 0 is Failed, 1 is Success
        /// </summary>
        public int LoginStatus { get; set; }

    }
}