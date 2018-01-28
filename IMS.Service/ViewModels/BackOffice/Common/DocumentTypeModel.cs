using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMS.Logic.ViewModels.BackOffice.Common
{
    public class DocumentTypeModel
    {
        public int Id { get; set; }
    	public int ModuleId { get; set; }
        public string Name { get; set; }
        public string DisplayName { get; set; }
    	public string DataModule { get; set; }
        public string Accept { get; set; }
        public bool IsMultipleAllowed { get; set; }
    }
}
