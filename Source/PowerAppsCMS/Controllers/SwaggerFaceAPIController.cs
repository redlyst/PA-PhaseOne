using System;
using Microsoft.ProjectOxford.Face;
using System.Collections.Generic;
using System.Configuration;
using System.Web;
using System.Web.Http;

namespace PowerAppsCMS.Controllers
{
    /// <summary>
    /// Menentukan FaceAPIKey dan FaceAPIRoot yang akan digunakan
    /// </summary>
    public class SwaggerFaceAPIController : ApiController
    {
        protected FaceServiceClient faceServiceClient;

        /// <summary>
        /// Mengambil FaceAPIKey dan FaceAPIRoot yang akan digunakan dari web config
        /// </summary>
        public SwaggerFaceAPIController()
        {
            faceServiceClient = new FaceServiceClient(
                    ConfigurationManager.AppSettings["FaceAPIKey"],
                    ConfigurationManager.AppSettings["FaceAPIRoot"]
                );
        }
    }
}
