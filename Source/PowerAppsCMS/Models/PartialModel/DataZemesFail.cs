using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PowerAppsCMS.Models
{
    /// <summary>
    /// merupakan class yang berfungsi untuk menambahkan kolom baru yang mana merepresentasikan kesuatu tabels 
    /// </summary>
    public partial class DataZemesFail
    {
        public string FailStatus
        {
            get
            {
                string result = "";

                if (this.Status == 1)
                {
                    result = "Product Not Found";
                }
                else
                {
                    result = "Unknown Status";
                }
                return result;
            }
        }
    }
}