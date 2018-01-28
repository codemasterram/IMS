using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMS.Logic.ViewModels.BackOffice.Master
{
    public class LeaveTypeViewModel : ServiceModel
    {
        public int LeaveTypeId { get; set; }
        public string Name { get; set; }
        public string NameNP { get; set; }
        public string Alias { get; set; }
        public Nullable<int> DisplayOrder { get; set; }
        public bool IsDeleted { get; set; }
    }
}
