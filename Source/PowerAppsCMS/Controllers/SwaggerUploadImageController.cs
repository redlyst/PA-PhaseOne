using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using Microsoft.WindowsAzure.Storage;
using System.Configuration;
using PowerAppsCMS.Models;
using Swashbuckle.Swagger.Annotations;
using System.Web;
using System.Drawing.Imaging;
using System.Drawing;
using Microsoft.WindowsAzure.Storage.Blob;

namespace PowerAppsCMS.Controllers
{
    /// <summary>
    /// Controller yang berisikan fungsi-fungsi yang digunakan untuk upload image ketika login di aplikasi powerapps
    /// </summary>
    public class UploadController : ApiController
    {
        /// <summary>
        /// Upload foto wajah user ke blobstorage
        /// </summary>
        /// <remarks>Upload foto wajah user ke blobstorage dan mengembalikan informasi file yang diupload, 
        /// seperti extensi file, nama file dan alamat URL file</remarks>
        /// <returns>Info dari foto yang diupload, seperti URL foto dan nama file</returns>
        [Route("api/Upload")]
        [HttpPost]
        [SwaggerResponse(HttpStatusCode.OK,
                Description = "Created",
                Type = typeof(UploadedFileInfo))]
        [SwaggerResponse(HttpStatusCode.InternalServerError,
                Description = "Internal Server Error",
               Type = typeof(Exception))]
        [SwaggerOperation("Upload")]
        public async Task<IHttpActionResult> PostFormData()
        {

            if (!Request.Content.IsMimeMultipartContent("form-data"))
            {
                return BadRequest();
            }

            try
            {
                var provider = await Request.Content.ReadAsMultipartAsync(new InMemoryMultipartFormDataStreamProvider());

                var files = provider.Files;
                var file1 = files[0];
                var fileStream = await file1.ReadAsStreamAsync();

                var extension = ExtractExtension(file1);

                Bitmap bitmap = new Bitmap(fileStream);

                int oldWidth = bitmap.Width;
                int oldHeight = bitmap.Height;

                GraphicsUnit units = System.Drawing.GraphicsUnit.Pixel;
                RectangleF r = bitmap.GetBounds(ref units);
                Size newSize = new Size();

                decimal expectedWidth = Convert.ToDecimal(300);
                decimal expectedHeight = (oldHeight * expectedWidth) / oldWidth;

                newSize.Width = (int)Math.Round(expectedWidth);
                newSize.Height = (int)Math.Round(expectedHeight);

                Bitmap b = new Bitmap(bitmap, newSize);

                Image img = (Image)b;
                byte[] data = ImageToByte(img);

                var contentType = file1.Headers.ContentType.ToString();
                var imageName = string.Concat(Guid.NewGuid().ToString(), extension);
                var storageConnectionString = "";// ";
                storageConnectionString = ConfigurationManager.AppSettings["FaceStorageConnectionString"];
                var storageAccount = CloudStorageAccount.Parse(storageConnectionString);
                var blobClient = storageAccount.CreateCloudBlobClient();
                var container = blobClient.GetContainerReference("faceapiimages");
                container.CreateIfNotExists();

                //var blockBlob = container.GetBlockBlobReference(imageName);
                //blockBlob.Properties.ContentType = contentType;
                //blockBlob.UploadFromStream(fileStream);
                CloudBlockBlob blockBlob = container.GetBlockBlobReference(imageName);
                blockBlob.Properties.ContentType = contentType;

                blockBlob.UploadFromByteArray(data, 0, data.Length);

                var fileInfo = new UploadedFileInfo
                {
                    FileName = imageName,
                    FileExtension = extension,
                    ContentType = contentType,
                    FileURL = blockBlob.Uri.ToString()
                };
                return Ok(fileInfo);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        /// <summary>
        /// Upload the image to the blob storage
        /// </summary>
        /// <remarks>Upload the image to the blob storage and returns the UploadedFileInfo</remarks>
        /// <returns>UploadedFileInfo</returns>
        [Route("api/Upload3")]
        [HttpPost]
        [SwaggerResponse(HttpStatusCode.OK,
                Description = "Created",
                Type = typeof(UploadedFileInfo))]
        [SwaggerResponse(HttpStatusCode.InternalServerError,
                Description = "Internal Server Error",
               Type = typeof(Exception))]
        [SwaggerOperation("Upload3")]
        public async Task<IHttpActionResult> PostFormData2()
        {

            if (!Request.Content.IsMimeMultipartContent("form-data"))
            {
                return BadRequest();
            }

            try
            {
                var provider = await Request.Content.ReadAsMultipartAsync(new InMemoryMultipartFormDataStreamProvider());

                var files = provider.Files;
                var file1 = files[0];
                var fileStream = await file1.ReadAsStreamAsync();

                var extension = ExtractExtension(file1);

                var contentType = file1.Headers.ContentType.ToString();
                var imageName = string.Concat(Guid.NewGuid().ToString(), extension);
                var storageConnectionString = "";// ";
                storageConnectionString = ConfigurationManager.AppSettings["FaceStorageConnectionString"];
                var storageAccount = CloudStorageAccount.Parse(storageConnectionString);
                var blobClient = storageAccount.CreateCloudBlobClient();
                var container = blobClient.GetContainerReference("faceapiimages");
                container.CreateIfNotExists();

                var blockBlob = container.GetBlockBlobReference(imageName);
                blockBlob.Properties.ContentType = contentType;
                blockBlob.UploadFromStream(fileStream);

                var fileInfo = new UploadedFileInfo
                {
                    FileName = imageName,
                    FileExtension = extension,
                    ContentType = contentType,
                    FileURL = blockBlob.Uri.ToString()
                };
                return Ok(fileInfo);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        /// <summary>
        /// Image to byte
        /// </summary>
        /// <param name="img">Image</param>
        /// <returns>byte array</returns>
        public static byte[] ImageToByte(Image img)
        {
            byte[] byteArray = new byte[0];
            using (MemoryStream stream = new MemoryStream())
            {
                img.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
                stream.Close();

                byteArray = stream.ToArray();
            }
            return byteArray;
        }

        private static string ExtractExtension(HttpContent file)
        {
            var invalidChars = Path.GetInvalidFileNameChars();
            var fileStreamName = file.Headers.ContentDisposition.FileName;
            var fileName = new string(fileStreamName.Where(x => !invalidChars.Contains(x)).ToArray());
            var extension = Path.GetExtension(fileName);

            return extension;
        }

        [Route("api/Upload2")]
        [HttpPost]
        [SwaggerResponse(HttpStatusCode.OK,
                Description = "Created",
                Type = typeof(UploadedFileInfo))]
        [SwaggerResponse(HttpStatusCode.InternalServerError,
                Description = "Internal Server Error",
               Type = typeof(Exception))]
        [SwaggerOperation("Upload2")]
        public async Task<IHttpActionResult> Upload2()
        {

            if (!Request.Content.IsMimeMultipartContent("form-data"))
            {
                return BadRequest();
            }

            try
            {
                var provider = await Request.Content.ReadAsMultipartAsync(new InMemoryMultipartFormDataStreamProvider());

                var files = provider.Files;
                var file1 = files[0];
                var fileStream = await file1.ReadAsStreamAsync();

                var extension = ExtractExtension(file1);

                Bitmap bitmap = new Bitmap(fileStream);

                int oldWidth = bitmap.Width;
                int oldHeight = bitmap.Height;

                GraphicsUnit units = System.Drawing.GraphicsUnit.Pixel;
                RectangleF r = bitmap.GetBounds(ref units);
                Size newSize = new Size();

                decimal expectedWidth = Convert.ToDecimal(500);
                decimal expectedHeight = (oldHeight * expectedWidth) / oldWidth;

                newSize.Width = (int)Math.Round(expectedWidth);
                newSize.Height = (int)Math.Round(expectedHeight);

                Bitmap b = new Bitmap(bitmap, newSize);

                Image img = (Image)b;
                byte[] data = ImageToByte(img);

                var contentType = file1.Headers.ContentType.ToString();
                var imageName = string.Concat(Guid.NewGuid().ToString(), extension);
                var storageConnectionString = "";// ";
                storageConnectionString = ConfigurationManager.AppSettings["FaceStorageConnectionString"];
                var storageAccount = CloudStorageAccount.Parse(storageConnectionString);
                var blobClient = storageAccount.CreateCloudBlobClient();
                var container = blobClient.GetContainerReference("faceapiimages");
                container.CreateIfNotExists();

                //var blockBlob = container.GetBlockBlobReference(imageName);
                //blockBlob.Properties.ContentType = contentType;
                //blockBlob.UploadFromStream(fileStream);
                CloudBlockBlob blockBlob = container.GetBlockBlobReference(imageName);
                blockBlob.Properties.ContentType = contentType;

                blockBlob.UploadFromByteArray(data, 0, data.Length);

                var fileInfo = new UploadedFileInfo
                {
                    FileName = imageName,
                    FileExtension = extension,
                    ContentType = contentType,
                    FileURL = blockBlob.Uri.ToString()
                };
                return Ok(fileInfo);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }


        //[Route("api/Upload2")]
        //[HttpPost]
        //[SwaggerResponse(HttpStatusCode.OK,
        //        Description = "Created",
        //        Type = typeof(UploadedFileInfo))]
        //[SwaggerResponse(HttpStatusCode.InternalServerError,
        //        Description = "Internal Server Error",
        //       Type = typeof(Exception))]
        //[SwaggerOperation("Upload2")]
        //public async Task<IHttpActionResult> Upload2()
        //{
        //    if (!Request.Content.IsMimeMultipartContent("form-data"))
        //    {
        //        return BadRequest();
        //    }
        //    try
        //    {
        //        var provider = await Request.Content.ReadAsMultipartAsync(new InMemoryMultipartFormDataStreamProvider());

        //        var files = provider.Files;
        //        var file1 = files[0];
        //        var fileStream = await file1.ReadAsStreamAsync();

        //        var extension = ExtractExtension(file1);

        //        Bitmap bitmap = new Bitmap(fileStream);

        //        int oldWidth = bitmap.Width;
        //        int oldHeight = bitmap.Height;

        //        GraphicsUnit units = System.Drawing.GraphicsUnit.Pixel;
        //        RectangleF r = bitmap.GetBounds(ref units);
        //        Size newSize = new Size();

        //        decimal expectedWidth = Convert.ToDecimal(100);
        //        decimal expectedHeight = (oldHeight * expectedWidth) / oldWidth;

        //        newSize.Width = (int)Math.Round(expectedWidth);
        //        newSize.Height = (int)Math.Round(expectedHeight);

        //        Bitmap b = new Bitmap(bitmap, newSize);

        //        Image img = (Image)b;
        //        byte[] data = ImageToByte(img);

        //        var contentType = file1.Headers.ContentType.ToString();
        //        var imageName = string.Concat(Guid.NewGuid().ToString(), extension);

        //        var mappedPath = System.Web.Hosting.HostingEnvironment.MapPath("~/Images/" + imageName);
        //        var fileInfo = new UploadedFileInfo();

        //        try
        //        {
        //            MemoryStream ms = new MemoryStream(data);
        //            FileStream fs = new FileStream(mappedPath, FileMode.Create);
        //            ms.WriteTo(fs);
        //            ms.Close();
        //            fs.Close();
        //            fs.Dispose();

        //            fileInfo = new UploadedFileInfo
        //            {
        //                FileName = imageName,
        //                FileExtension = extension,
        //                ContentType = contentType,
        //                FileURL = mappedPath
        //            };
        //        }
        //        catch (Exception ex)
        //        {
        //            fileInfo = new UploadedFileInfo
        //            {
        //                FileName = ex.Message + "" + ex.StackTrace,
        //                FileExtension = extension,
        //                ContentType = contentType,
        //                FileURL = mappedPath
        //            };

        //        }
        //        return Ok(fileInfo);
        //    }
        //    catch (Exception ex)
        //    {
        //        return InternalServerError(ex);
        //    }
        //}
    }
}
