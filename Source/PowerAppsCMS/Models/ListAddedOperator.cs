using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PowerAppsCMS.Models
{
    /// <summary>
    /// Model yang digunakan untuk menyimpan data List Operator pada halaman assign operator
    /// </summary>
    public class ListAddedOperator
    {
        /// <summary>
        /// ID Process Assign
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// user ID operator
        /// </summary>
        public Guid UserID { get; set; }

        /// <summary>
        /// ID status aktivitas operator
        /// </summary>
        public int StatusID { get; set; }

        /// <summary>
        /// Nama operator
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Nama status aktivitas operator
        /// </summary>
        public string StatusName { get; set; }

        /// <summary>
        /// Tipe pekerjaan operator
        /// </summary>
        public Nullable<int> Type { get; set; }
    }
}