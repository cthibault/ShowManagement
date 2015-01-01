using Entities.Pattern;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShowManagement.Entity
{
    public partial class ShowDownload : Entities.Pattern.Entity
    {
        public int ShowDownloadId { get; set; }
        public string OriginalPath { get; set; }
        public string CurrentPath { get; set; }
        public DateTime ModifiedDate { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
