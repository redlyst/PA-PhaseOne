using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PowerAppsCMS.Models
{
    /// <summary>
    /// Class yang digunakan untuk menyimpan data daftar yang sudag di assign keoperator
    /// </summary>
    public class TaskListOperator
    {
        /// <summary>
        /// ID process assign
        /// </summary>
        public int ProcessAssignID { get; set; }

        /// <summary>
        /// ID user
        /// </summary>
        public Guid UserID { get; set; }

        /// <summary>
        /// Serial number
        /// </summary>
        public string SerialNumber { get; set; }

        /// <summary>
        /// ID proses
        /// </summary>
        public int ProcessID { get; set; }

        /// <summary>
        /// Nama proses
        /// </summary>
        public string ProcessName { get; set; }

        /// <summary>
        /// PRO
        /// </summary>
        //public int PROID { get; set; }
        public string PRO { get; set; }

        /// <summary>
        /// Nama produk
        /// </summary>
        //public int ProductID { get; set; }
        public string ProductName { get; set; }

        /// <summary>
        /// Nama pelanggan
        /// </summary>
        public string Customer { get; set; }    

        /// <summary>
        /// ID status operator
        /// </summary>
        public int StatusID { get; set; }

        /// <summary>
        /// ID status proses
        /// </summary>
        public int ProcessStatus { get; set; }
        
        /// <summary>
        /// Tipe pekerjaan operator
        /// </summary>
        public Nullable<int> Type { get; set; }
        //public System.DateTime Created { get; set; }
        //public System.DateTime LastModified { get; set; }
    }
}