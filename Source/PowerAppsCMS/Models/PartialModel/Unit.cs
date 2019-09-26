using System;
using System.Linq;

namespace PowerAppsCMS.Models
{
    /// <summary>
    /// Merupakan partial class dari model Unit yang menyimpan informasi atribut tambahan yang dibutuhkan
    /// </summary>
    public partial class Unit
    {
        /// <summary>
        /// atribut yang menyimpan informasi apakah sebuah unit sudah memelik process assign atau belum
        /// </summary>
        public bool IsHaveProcessAssign
        {
            get
            {
                bool haveProcessAssign = false;
                
                if(this.Processes.Count > 0)
                {
                    if(this.Processes.Where(x=> x.IsHaveProcessAssign == true).Count() > 0)
                    {
                        haveProcessAssign = true;
                    }
                }

                return haveProcessAssign;
            }
        }

        /// <summary>
        /// atribut yang menyimpan informasi apakah sebuah unit sudah memiliki tanggal sfs
        /// </summary>
        public bool IsHaveSFS
        {
            get
            {
                return this.SFSDueDate != null;
            }
        }


        /// <summary>
        /// atribut yang menyimpan informasi css class yang akan digunakan pada halaman sfs untuk unit tersebut
        /// </summary>
        public string SFSCategoryCSSClass
        {
            get
            {
                string cssClass = "sfs-warning";

                if (this.IsHold)
                {
                    cssClass = "sfs-deactived";
                }
                else if (this.SFSDueDate != null)
                {
                    cssClass = "";
                }
                else if ( ( this.MPSDueDate != null && ((DateTime)this.MPSDueDate).Date < DateTime.Now.Date) || this.DueDate.Date < DateTime.Now.Date)
                {
                    cssClass = "sfs-danger";
                }
                return cssClass;
            }
        }

        /// <summary>
        /// atribut  yang menyimpan informasi css class yang dibutuhkan sebuah unit dihalaman MPS
        /// </summary>
        public string MPSCategoryCSSClass
        {
            get
            {
                string cssClass = "";

                if (this.IsHold)
                {
                    cssClass = "sn-hold badge badge-secondary";
                }
                else if (this.ActualDeliveryDate != null)
                {
                    cssClass = "badge badge-success";
                }
                else if (this.IsHaveProcessAssign)
                {
                    cssClass = "badge badge-primary";
                }else if (this.MPSID != null)
                {
                    cssClass = "badge badge-warning";
                }
                return cssClass;
            }
        }


        public string Process { get; set; }


        public string SfsCssClass
        {
            get
            {
                string cssClass = "mps-row ";

                if (this.SFSDueDate != null)
                {
                    cssClass += "sfs-row ";
                }

                if ((this.MPSDueDate <= DateTime.Now.AddDays(-1)))
                {
                    cssClass += "mps-row-disabled ";
                }

                if ((this.MPSDueDate > DateTime.Now.AddDays(-1)))
                {
                    cssClass += "available-day ";
                }
                return cssClass;
            }
        }

        /// <summary>
        /// atribut yang menyimpan informasi apakah adah prosess issue yang ada pada prosses diunit tersebut
        /// </summary>
        public bool IsHaveProcessIssue
        {
            get
            {
                bool haveProcessIssue = false;

                if (this.Processes.Count > 0)
                {
                    if (this.Processes.Where(x => x.IsHaveProcessIssue == true).Count() > 0)
                    {
                        haveProcessIssue = true;
                    }
                }

                return haveProcessIssue;
            }
        }
    }
}