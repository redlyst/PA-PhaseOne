using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PowerAppsCMS.Models
{
    /// <summary>
    /// Model Class yang digunakan untuk menyimpan informasi range data bulan yang ditampilkan pada MPS
    /// </summary>
    public class MonthRange
    {
        /// <summary>
        /// atribut untuk menyimpan data nama bulan
        /// </summary>
        public string MonthDisplayText { get; set; }
        /// <summary>
        /// atribut untuk menyimpan data nomor bulan
        /// </summary>
        public int Month { get; set; }
        /// <summary>
        /// atribut untuk menyimpan tahun 
        /// </summary>
        public int Year { get; set; }
        /// <summary>
        /// atribut menyimpan capacity mps pada sebuah bulan
        /// </summary>
        public int? Capacity { get; set; }
        /// <summary>
        /// atribut menyimpan daftar weeknumber sebuah bulan
        /// </summary>
        public List<WeekNumber> WeekNumberList { get; set; }
        /// <summary>
        /// fungsi yang dijalankan ketika class monthrange diinisiasi lalu membuat sebuah objek weeknumberlist
        /// </summary>
        public MonthRange()
        {
            WeekNumberList = new List<WeekNumber>();
        }
    }
}