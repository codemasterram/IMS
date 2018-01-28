using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMS.Logic.ViewModels.BackOffice.Common
{
    public class DocumentFileModel
    {
        public int DataId { get; set; }
        public string Name { get; set; }
        public byte[] FileData { get; set; }
        public string MimeType { get; set; }
    }
}
