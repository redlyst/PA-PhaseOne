using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PowerAppsCMS.Models
{
    /// <summary>
    /// Class model yang menyimpan informasi data yang digunakan untuk week number sebuah bulan pada MPS
    /// </summary>
    public class WeekNumber
    {
        /// <summary>
        /// bilangan dari minggu 
        /// </summary>
        public int Number { get; set; }

        /// <summary>
        /// tanggal akhir yang berlaku pada hari kerja pada minggu tersebut
        /// </summary>
        public DateTime EndsWorkingDate { get; set; }

        /// <summary>
        /// tanggal awal yang berlaku pada hari kerja pada minggu tersebut
        /// </summary>
        public DateTime StartsWorkingDate { get; set; }

        /// <summary>
        /// tanggal akhir yang berlaku pada minggu tersebut
        /// </summary>
        public DateTime EndsDateWeek { get; set; }

        /// <summary>
        /// tanggal awal yang berlaku pada minggu tersebut
        /// </summary>
        public DateTime StartsDateWeek { get; set; }
    }


    /// <summary>
    /// class yang menyimpan informasi actual dari sebuah MPS
    /// </summary>
    public class MPSActual
    {
        /// <summary>
        /// total unit yang sudah memiliki actual
        /// </summary>
        public int Total { get{ return this.UnitList.Count; } }

        /// <summary>
        /// bilangan minggu dari mps actual
        /// </summary>
        public int Week { get; set; }

        /// <summary>
        /// bilangan bulan dari mps actual
        /// </summary>
        public int Month { get; set; }

        /// <summary>
        /// tahun dari mps actual
        /// </summary>
        public int Year { get; set; }

        /// <summary>
        /// tanggal akhir yang berlaku pada minggu tersebut
        /// </summary>
        public DateTime EndsDateWeek { get; set; }

        /// <summary>
        /// tanggal awal yang berlaku pada minggu tersebut
        /// </summary>
        public DateTime StartsDateWeek { get; set; }

        /// <summary>
        /// daftar unit yag sudah finis/selesai pada minggu tersebut
        /// </summary>
        public List<Unit> UnitList { get; set; }

        public MPSActual()
        {
            UnitList = new List<Unit>();
        }

    }
}