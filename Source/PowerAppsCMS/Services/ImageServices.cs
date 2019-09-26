using System;
using System.Web;
using System.IO;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System.Configuration;
using System.Drawing;
using System.Linq;
using System.Drawing.Imaging;

namespace PowerAppsCMS.Services
{
    /// <summary>
    /// Berfungsi untuk mengupload gambar ke blob storage, berisikan fungsi UploadImage, dan DeleteImage
    /// </summary>
    public class ImageServices
    {
        /// <summary>
        /// Berfungsi untuk mengupload gambar ke dalam blob storage
        /// </summary>
        /// <param name="imageUpload">Object yang berisikan file gambar</param>
        /// <returns>Gambar berhasil di upload ke blob storage</returns>
        public async Task<string> UploadImage(HttpPostedFileBase imageUpload)
        {
            string imageFullPath = null;
            if (imageUpload == null || imageUpload.ContentLength == 0)
            {
                return null;
            }
            try
            {
                byte[] imageData = new byte[imageUpload.ContentLength];
                imageUpload.InputStream.Read(imageData, 0, imageUpload.ContentLength);

                MemoryStream ms = new MemoryStream(imageData);
                Image originalImage = Image.FromStream(ms);

                if (originalImage.PropertyIdList.Contains(0x0112))
                {
                    int rotationValue = originalImage.GetPropertyItem(0x0112).Value[0];

                    switch (rotationValue)
                    {
                        case 1:
                            originalImage.RotateFlip(rotateFlipType: RotateFlipType.RotateNoneFlipNone);
                        break;
                        case 2:
                            originalImage.RotateFlip(rotateFlipType: RotateFlipType.RotateNoneFlipX);
                        break;
                        case 3:
                            originalImage.RotateFlip(rotateFlipType: RotateFlipType.Rotate180FlipNone);
                        break;
                        case 4:
                            originalImage.RotateFlip(rotateFlipType: RotateFlipType.Rotate180FlipX);
                        break;
                        case 5:
                            originalImage.RotateFlip(rotateFlipType: RotateFlipType.Rotate90FlipX);
                        break;
                        case 6:
                            originalImage.RotateFlip(rotateFlipType: RotateFlipType.Rotate90FlipNone);
                        break;
                        case 7:
                            originalImage.RotateFlip(rotateFlipType: RotateFlipType.Rotate270FlipX);
                        break;
                        case 8: 
                            originalImage.RotateFlip(rotateFlipType: RotateFlipType.Rotate270FlipNone);
                        break;
                    }
                }

                Bitmap bitmap = new Bitmap(originalImage);
                                
                Bitmap b = bitmap;
                int oldWidth = bitmap.Width;
                int oldHeight = bitmap.Height;

                GraphicsUnit units = System.Drawing.GraphicsUnit.Pixel;
                RectangleF r = bitmap.GetBounds(ref units);
                Size newSize = new Size();

                if (imageUpload.ContentLength > 4000)
                {
                    decimal expectedWidth = Convert.ToDecimal(1000);
                    decimal expectedHeight = (oldHeight * expectedWidth) / oldWidth;

                    newSize.Width = (int)Math.Round(expectedWidth);
                    newSize.Height = (int)Math.Round(expectedHeight);
                    b = new Bitmap(bitmap, newSize);
                }
                Image img = (Image)b;
                byte[] data = ImageToByte(img);

                CloudStorageAccount cloudStorageAccount = CloudStorageAccount.Parse(ConfigurationManager.AppSettings["BlobStorageConnectionString"]);
                CloudBlobClient cloudBlobClient = cloudStorageAccount.CreateCloudBlobClient();
                CloudBlobContainer cloudBlobContainer = cloudBlobClient.GetContainerReference("images");

                if (await cloudBlobContainer.CreateIfNotExistsAsync())
                {
                    await cloudBlobContainer.SetPermissionsAsync(
                        new BlobContainerPermissions
                        {
                            PublicAccess = BlobContainerPublicAccessType.Blob
                        });
                }
                string imageName = Guid.NewGuid().ToString() + "-" + Path.GetExtension(imageUpload.FileName);

                CloudBlockBlob cloudBlockBlob = cloudBlobContainer.GetBlockBlobReference(imageName);
                cloudBlockBlob.Properties.ContentType = imageUpload.ContentType;

                cloudBlockBlob.UploadFromByteArray(data, 0, data.Length);

                //await cloudBlockBlob.UploadFromStreamAsync(imageUpload.InputStream);

                imageFullPath = cloudBlockBlob.Uri.ToString();

                
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            return imageFullPath;
        }

        /// <summary>
        /// Berfungsi untuk menghapus gambar di dalam blob storage
        /// </summary>
        /// <param name="url">Sebuah string yang berisikan url dari gambar</param>
        public void DeleteImage(string url)
        {
            System.Uri imageUrl = new System.Uri(url);
            CloudStorageAccount cloudStorageAccount = CloudStorageAccount.Parse(ConfigurationManager.AppSettings["BlobStorageConnectionString"]);
            CloudBlobClient cloudBlobClient = cloudStorageAccount.CreateCloudBlobClient();
            CloudBlobContainer cloudBlobContainer = cloudBlobClient.GetContainerReference("images");

            string imageName = Path.GetFileName(imageUrl.LocalPath);
            CloudBlockBlob cloudBlockBlob = cloudBlobContainer.GetBlockBlobReference(imageName);
            cloudBlockBlob.Delete();
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
                img.Save(stream, System.Drawing.Imaging.ImageFormat.Jpeg);
                stream.Close();

                byteArray = stream.ToArray();
            }
            return byteArray;
        }
    }
}