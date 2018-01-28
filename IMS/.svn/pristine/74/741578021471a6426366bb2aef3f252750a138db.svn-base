
using SoftIms.Data;
using SoftIms.Data.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SoftIms.Utilities
{
    public static class SessionData
    {
        private static IList<SystemConfigurationViewModel> GetSystemConfiguration()
        {
            using (UnitOfWork db = new UnitOfWork())
            {
                var s = db.SystemConfigurationRepo.Table();
                var settings = AutomapperConfig.Mapper.Map<IList<SystemConfigurationViewModel>>(s);
                return settings;
            }
        }

        public static string GetSystemConfigurationValue(string key)
        {
            if (HttpContext.Current.Session["SystemConfiguration"] == null)
            {
                HttpContext.Current.Session["SystemConfiguration"] = GetSystemConfiguration();
            }
            var data = (IList<SystemConfigurationViewModel>)HttpContext.Current.Session["SystemConfiguration"];
            var val = data.Where(x => x.Key == key).FirstOrDefault();
            return val == null ? null : val.Value;
        }

        public static bool ValueIsEqual(this string key, string value)
        {
            if (HttpContext.Current.Session["SystemConfiguration"] == null)
            {
                HttpContext.Current.Session["SystemConfiguration"] = GetSystemConfiguration();
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