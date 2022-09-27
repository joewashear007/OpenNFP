namespace OpenNFP.Shared.Models
{
    /// <summary>
    /// Class used to sync data to remote service, at time of writing only google drive
    /// </summary>
    public class SyncInfo
    {
        /// <summary>
        /// The file id in the remote system, aka Google Drive file id
        /// </summary>
        public string FileId { get; set; } = "";
        /// <summary>
        /// The filename of the file to sync, should be changable but not tested
        /// </summary>
        public string FileName { get; set; } = "opennfp.json";

        /// <summary>
        /// The file size, probably could be removed
        /// </summary>
        public int FileSizeKb { get; set; } = 0;

        /// <summary>
        /// The date of the last time the file was modifiled on the remote storage
        /// </summary>
        public DateTime SyncTimeStamp { get; set; } = DateTime.MinValue;

        /// <summary>
        /// Default empty sync object when no sync info 
        /// </summary>
        public static readonly SyncInfo Empty = new();
    }
}
