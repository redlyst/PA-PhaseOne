using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PowerAppsCMS.Models
{
    /// <summary>
    /// Model yang digunakan untuk menyimpan daftar isu proses
    /// </summary>
    public class ListProcessIssue
    {
        /// <summary>
        /// ID Process Issue
        /// </summary>
        public int ProcessIssueID { get; set; }

        /// <summary>
        /// ID alasan issue
        /// </summary>
        public int ReasonIssueID { get; set; }

        /// <summary>
        /// Nama issue
        /// </summary>
        public string IssueName { get; set; }

        /// <summary>
        /// Status issue
        /// </summary>
        public bool Status { get; set; }

        /// <summary>
        /// Tanggal issu dibuat
        /// </summary>
        public string StartDate { get; set; }

        /// <summary>
        /// Tanggal issue selesai
        /// </summary>
        public string FinishDate { get; set; }
    }
}