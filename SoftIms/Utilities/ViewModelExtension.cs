using SoftIms.Data;
using SoftIms.Data.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using static SoftIms.Data.Enums;

namespace SoftIms.Utilities
{
    public static class ViewModelExtension
    {
        public static string CheckForDestroy(this AppUserViewModel appUser, params ePermission[] permissions)
        {
            bool val = HasPermission(appUser, permissions);
            return val ? "" : "destroy-me";
        }

        public static string CheckForDisable(this AppUserViewModel appUser, params ePermission[] permissions)
        {
            bool val = HasPermission(appUser, permissions);
            return val ? "" : "disable-me";
        }

        public static bool HasPermission(this AppUserViewModel appUser, params ePermission[] permissions)
        {
            if (appUser == null)
                return false;

            if (SessionData.HasSystemConfigurationValue("SuperAdministrator", appUser.UserName))
            {
                return true;
            }

            using (UnitOfWork db = new UnitOfWork())
            {
                var userRoles = db.AppUserRepo.Table().FirstOrDefault(x => x.UserName == appUser.UserName).UserRoles.Select(x => x.RoleId).ToArray();
                var query = db.RolePermissionRepo.Table().Where(x => userRoles.Contains(x.RoleId) && permissions.Any(x1 => (int)x1 == x.PermissionId));

                return query.Any();
            }
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

        public static string GetExceptionMessages(this Exception ex)
        {
            if (ex == null) return string.Empty;
            string msg = ex.Message;
            if (ex.InnerException != null)
            {
                msg = GetExceptionMessages(ex.InnerException);
            }
            return msg;
        }
    }
}