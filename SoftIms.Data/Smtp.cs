using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SoftIms.Data
{
    public class Smtp
    {
        public string SMTPSender { get; set; } = "feedback.softech.nepal@gmail.com";
        public string SMTPSenderDisplayName { get; set; } = "IMS Notification Service";
        public string SMTP { get; set; } = "smtp.gmail.com";
        public int SMTPPort { get; set; } = 587;
        public string SMTPPassword { get; set; } = "support@softech2016";
    }
}
