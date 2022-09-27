using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNFP.Shared.Models
{
    public class SyncInfo
    {
        public string FileId { get; set; } = "";
        public string FileName { get; set; } = "opennfp.json";
        public int FileSizeKb { get; set; } = 0;
        public DateTime SyncTimeStamp { get; set; } = DateTime.MinValue;

        public static readonly SyncInfo Empty = new();
    }
}
