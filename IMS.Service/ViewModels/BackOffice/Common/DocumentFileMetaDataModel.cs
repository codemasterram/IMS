using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMS.Logic.ViewModels.BackOffice.Common
{
    public class DocumentFileMetaDataModel
    {
        public int FileId { get; set; }
        public int MasterId { get; set; }
        public int DataId { get; set; }
        public string Name { get; set; }
        public string FileName { get; set; }
        public string MimeType { get; set; }
        public string Remarks { get; set; }
        public int Module { get; set; }
    }
}
