using IMS.Logic.Contract;
using IMS.Logic.ViewModels.BackOffice.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace IMS.Logic.Extensions
{
    public static class SessionData
    {
        public static string GetSystemConfigurationValue(string key)
        {
            if (HttpContext.Current.Session["SystemConfiguration"] == null)
            {
                ICommonService commonService = DependencyResolver.Current.GetService<ICommonService>();
                HttpContext.Current.Session["SystemConfiguration"] = commonService.GetSystemConfiguration();
            }
            var data = (IList<SystemConfigurationViewModel>)HttpContext.Current.Session["SystemConfiguration"];
            var val = data.Where(x => x.Key == key).FirstOrDefault();
            return val == null ? null : val.Value;
        }

        public static bool ValueIsEqual(this string key, string value)
        {
            if (HttpContext.Current.Session["SystemConfiguration"] == null)
            {
                ICommonService commonService = DependencyResolver.Current.GetService<ICommonService>();
                HttpContext.Current.Session["SystemConfiguration"] = commonService.GetSystemConfiguration();
            }
            var data = (IList<SystemConfigurationViewModel>)HttpContext.Current.Session["SystemConfiguration"];
            var val = data.Where(x => x.Key == key).FirstOrDefault();
            return val == null ? false : val.Value == value ? true : false;
        }

        public static bool HasSystemConfigurationValue(string key, string value)
        {
            return GetSystemConfigurationValue(key) == value;
        }
    }
}