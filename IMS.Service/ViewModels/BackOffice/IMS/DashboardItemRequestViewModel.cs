using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMS.Logic.ViewModels.BackOffice.IMS
{
    public class DashboardItemRequestViewModel : ServiceModel
    {
        public int ItemRequestId { get; set; }
        public DateTime RequestedDate { get; set; }
        public string RequestedMiti { get; set; }
        public string RequestNo { get; set; }
        public string Item { get; set; }
        public int Qty { get; set; }
        public string Status { get; set; }
    }
}
