using IMS.Data;
using IMS.Logic.Configuration;
using IMS.Logic.Contract;
using IMS.Logic.ViewModels.BackOffice.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using static IMS.Data.NTAEnum;

namespace IMS.Logic.Extensions
{
    public static class ViewModelExtensions
    {
        public static string CheckForDestroy(this AppUserViewModel appUser, params ePermission[] permissions)
        {
            bool val = HasPermission(appUser, permissions);
            return val ? "" : "destroy-me";
        }

        public static string CheckForDestroy(this AppUserViewModel appUser, params eModule[] modules)
        {
            var val = HasPermission(appUser, modules);
            return val ? "" : "destroy-me";
        }

        public static string CheckForDisable(this AppUserViewModel appUser, params ePermission[] permissions)
        {
            bool val = HasPermission(appUser, permissions);
            return val ? "" : "disable-me";
        }

        public static string CheckForDisable(this AppUserViewModel appUser, params eModule[] modules)
        {
            bool val = HasPermission(appUser, modules);
            return val ? "" : "disable-me";
        }

        public static bool HasPermission(this AppUserViewModel appUser, params NTAEnum.ePermission[] permissions)
        {
            if (appUser == null)
                return false;

            if (SessionData.HasSystemConfigurationValue("SuperAdministrator", appUser.Username))
            {
                return true;
            }

            IAuthService authService = IMSAppConfig.Instance.Resolve<IAuthService>();
            int[] permissionList = permissions.Select(x => (int)x).ToArray();

            bool isAuthorized = authService.IsAuthorized(appUser.Username, permissionList);
            return isAuthorized;
        }

        public static bool HasPermission(this AppUserViewModel appUser, params NTAEnum.eModule[] modules)
        {
            if (appUser == null)
                return false;

            if (SessionData.HasSystemConfigurationValue("SuperAdministrator", appUser.Username))
            {
                return true;
            }

            IAuthService authService = IMSAppConfig.Instance.Resolve<IAuthService>();
            int[] moduleList = modules.Select(x => (int)x).ToArray();

            bool isAuthorized = authService.IsAuthorizedToModule(appUser.Username, moduleList);
            return isAuthorized;
        }

        public static string ConvertToSentence(this string word)
        {
            string output = "";

            if (!word.Any(char.IsLower))
            {
                return word;
            }

            foreach (char letter in word)
            {
                if (Char.IsUpper(letter) && output.Length > 0)
                    output += " " + letter.ToString().Trim();
                else
                    output += letter.ToString().Trim();
            }
            return output;
        }

        public static string ClientIP
        {
            get
            {
                HttpRequest currentRequest = HttpContext.Current.Request;
                string ipAddress = currentRequest.ServerVariables["HTTP_X_FORWARDED_FOR"];

                if (ipAddress == null || ipAddress.ToLower() == "unknown")
                    ipAddress = currentRequest.ServerVariables["REMOTE_ADDR"];

                return ipAddress;
            }
        }

        public static string ClientUserAgent
        {
            get
            {
                HttpRequest currentRequest = HttpContext.Current.Request;
                string userAgent = currentRequest.Browser != null ? currentRequest.Browser.Browser : "NA";

                return userAgent;
            }
        }

        public static string ClientDevice
        {
            get
            {
                HttpRequest currentRequest = HttpContext.Current.Request;
                string userAgent = currentRequest != null ? currentRequest.ServerVariables["REMOTE_HOST"] : "NA";

                return userAgent;
            }
        }

    }
}
