namespace PowerAppsCMS.Models
{
    /// <summary>
    /// Informasi untuk file yang diupload
    /// </summary>
    public class UploadedFileInfo
    {
        /// <summary>
        /// Nama file
        /// </summary>
        public string FileName { get; set; }
        /// <summary>
        /// Extensi file
        /// </summary>
        public string FileExtension { get; set; }
        /// <summary>
        /// URL file
        /// </summary>
        public string FileURL { get; set; }
        /// <summary>
        /// Tipe content file
        /// </summary>
        public string ContentType { get; set; }
    }
}