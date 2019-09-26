using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace PowerAppsCMS.Models
{    public partial class tempImage
    {
        /// <summary>
        /// Menampilkan data long dengan tambahan satuan milimeter
        /// </summary>
        public string ImageDataUrl
        {
            get
            {
                return "https://blobstorageutpe.blob.core.windows.net/images/" + this.imageName;
            }
        }
    }
}