using IMS.Logic.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMS.Logic.ViewModels.BackOffice.User
{
    public class LoginHistoryViewModel : ServiceModel
    {
        public int LoginHistoryId { get; set; }
        public string UserName { get; set; }
        public string Browser { get; set; } = ViewModelExtensions.ClientUserAgent;
        public string Device { get; set; } = ViewModelExtensions.ClientDevice;
        public string IPAddress { get; set; } = ViewModelExtensions.ClientIP;
        public System.DateTime LogDate { get; set; } = DateTime.Now;
        public bool IsLoginSuccessful { get; set; }
    }
}
