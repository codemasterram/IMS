using System;
using System.Configuration;
using System.Web.Mvc;
using static IMS.Data.NTAEnum;
using static System.Configuration.ConfigurationManager;


namespace IMS.Logic.Configuration
{
    public class IMSAppConfig
    {
        private static IMSAppConfig instance;

        private string currentLanguage;
        private IDependencyResolver dependencyResolver;

        private IMSAppConfig()
        {
            currentLanguage = ConfigurationManager.AppSettings["Language"];
        }

        public static IMSAppConfig Instance
        {
            get
            {
                return instance ?? (instance = new IMSAppConfig());
            }
        }

        public string CurrentLanguage
        {
            get
            {
                return currentLanguage;
            }
            set
            {
                currentLanguage = value;
            }
        }

        public CurrentLang CurrentLang
        {
            get
            {
                return CurrentLang.En;
                //return CurrentLanguage.ToLower() == "np" ? CurrentLang.Np : CurrentLang.En;
            }
        }

        public T Resolve<T>()
        {
            return DependencyResolver.GetService<T>();
        }

        public IDependencyResolver DependencyResolver
        {
            get
            {
                return dependencyResolver;
            }
            set
            {
                dependencyResolver = value;
            }
        }

        #region File Encryption Key
        public string FileEncryptionKey { get { return "NTA_Office_Automation_System_Key_For_Files"; } }
        #endregion

        #region Default Page size
        public int DefaultPageSize { get; set; } = 100;
        #endregion

        #region SMTP Properties
        public string SMTP => System.Configuration.ConfigurationManager.AppSettings["SMTP"];

        public int SMTPPort => Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["SMTPPort"]);

        public string SMTPSender => System.Configuration.ConfigurationManager.AppSettings["SMTPSender"];

        public string SMTPSenderDisplayName => System.Configuration.ConfigurationManager.AppSettings["SMTPSenderDisplayName"];

        public string SMTPPassword => System.Configuration.ConfigurationManager.AppSettings["SMTPPassword"];

        public string SMSAPI => System.Configuration.ConfigurationManager.AppSettings["SMSAPI"];
        #endregion

        #region AttendanceRelatedProperties
        public string PresentSymbol => AppSettings["PresentSymbol"];
        public string PresentColor => AppSettings["PresentColor"];
        public string AbsentSymbol => AppSettings["AbsentSymbol"];
        public string AbsentColor => AppSettings["AbsentColor"];
        public string HolidaySymbol => AppSettings["HolidaySymbol"];
        public string HolidayColor => AppSettings["HolidayColor"];
        public string TravelSymbol => AppSettings["TravelSymbol"];
        public string TravelColor => AppSettings["TravelColor"];
        public string LeaveSymbol => AppSettings["LeaveSymbol"];
        public string LeaveColor => AppSettings["LeaveColor"];
        public string CheckInColor => AppSettings["CheckInColor"];
        public string CheckOutColor => AppSettings["CheckOutColor"];
        public string TotalWorkedHourColor => AppSettings["TotalWorkedHourColor"];
        #endregion

        public string HttpContextPath => AppSettings["HttpContextPath"];

        public bool IsDebugMode => AppSettings["DebugMode"] != null && AppSettings["DebugMode"].ToUpper() == "TRUE";
    }
}
