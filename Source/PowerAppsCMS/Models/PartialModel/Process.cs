using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PowerAppsCMS.Models
{
    /// <summary>
    /// Merupakan partial class dari model Process
    /// </summary>
    public partial class Process
    {
        /// <summary>
        /// atribut yang menyimpan informasi apakah prosess sudah di assign atau belum
        /// </summary>
        public bool IsHaveProcessAssign
        {
            get
            {
                bool haveProcessAssign = false;

                if (this.ProcessAssigns.Count > 0)
                {
                    haveProcessAssign = true;
                }

                return haveProcessAssign;
            }
        }

        /// <summary>
        /// atribut yang menyimpan informasi apakah prosess memiliki issue atau tidak
        /// </summary>
        public bool IsHaveProcessIssue
        {
            get
            {
                bool haveProcessIssue = false;

                if (this.ProcessIssues.Count > 0)
                {
                    haveProcessIssue = true;
                }

                return haveProcessIssue;
            }
        }

        /// <summary>
        /// atribut yang menampilkan informasi status
        /// </summary>
        public string StatusDisplayText
        {
            get
            {
                return Enum.GetName(typeof(Constants.ProcessStatus), this.Status);
            }
        }
    }
}