using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using IMS.Logic.DataAnnotations;
using IMS.Logic.Contract;
using IMS.Logic.Configuration;
using IMS.Logic.Implementation;
using System.Web.Mvc;
using IMS.Logic.ViewModels;
using System.Drawing;
using System.Net.NetworkInformation;

namespace IMS.Logic.Utilities
{
    public static class Common
    {
        public static string SelectLimited(this string s, int limit)
        {
            if (s.Length <= limit)
                return s;
            else
                return s.Substring(limit) + "...";
        }

        public static bool IsFutureDate(this DateTime? date)
        {
            if (!date.HasValue)
                return false;
            return date.Value > DateTime.Now;
        }

        public static bool IsFutureDate(this DateTime date)
        {
            return date > DateTime.Now && date != DateTime.MinValue;
        }

        public static bool IsPastDate(this DateTime date)
        {
            return date < DateTime.Now && date != DateTime.MinValue;
        }

        public static bool IsPastDate(this DateTime? date)
        {
            return date < DateTime.Now && date != DateTime.MinValue;
        }

        public static bool IsFuturMiti(this string miti)
        {
            var date = DateMiti.GetDateMiti.GetDate(miti);
            if (date == DateTime.MinValue)
                return false;
            return Common.IsFutureDate(date);
        }

        public static string GetDisplayName<TModel, TProperty>(this TModel model, Expression<Func<TModel, TProperty>> expression, string pushText = null)
        {
            Type type = typeof(TModel);
            MemberExpression memberExpression = (MemberExpression)expression.Body;
            string propertyName = ((memberExpression.Member is PropertyInfo) ? memberExpression.Member.Name : null);
            var attr = (DisplayAttribute)type.GetProperty(propertyName).GetCustomAttributes(typeof(DisplayAttribute), true).SingleOrDefault();
            var attrLoc = (LocalizedDisplayNameAttribute)type.GetProperty(propertyName).GetCustomAttributes(typeof(LocalizedDisplayNameAttribute), true).SingleOrDefault();
            string displayName = attr != null ? attr.Name : attrLoc != null ? attrLoc.DisplayName : propertyName;
            return $"{pushText} {displayName} {pushText}".TrimStart().TrimEnd();
        }

        public static string[] GetPropertyName<TModel, TProperty>(this TModel model, Expression<Func<TModel, TProperty>> expression)
        {
            Type type = typeof(TModel);

            MemberExpression memberExpression = (MemberExpression)expression.Body;
            string propertyName = ((memberExpression.Member is PropertyInfo) ? memberExpression.Member.Name : null);
            return new string[] { propertyName };
        }

        public static ValidationResult AddModelError<TModel, TProperty>(this TModel model, Expression<Func<TModel, TProperty>> expression, string key, string defaultMessage = null) where TModel : ServiceModel
        {
            ILocalizationService service = IMSAppConfig.Instance.DependencyResolver.GetService<ILocalizationService>();
            Type type = typeof(TModel);
            MemberExpression memberExpression = (MemberExpression)expression.Body;
            string propertyName = ((memberExpression.Member is PropertyInfo) ? memberExpression.Member.Name : null);
            var properties = new string[] { propertyName };

            var validationResult = new ValidationResult(service.GetLocalizedText(key, IMSAppConfig.Instance.CurrentLanguage, defaultMessage), properties);
            model.Errors.Add(validationResult);
            return validationResult;
        }

        public static ValidationResult AddError<TModel, TProperty>(this TModel model, Expression<Func<TModel, TProperty>> expression, string errorMessage) where TModel : ServiceModel
        {
            MemberExpression memberExpression = (MemberExpression)expression.Body;
            string propertyName = ((memberExpression.Member is PropertyInfo) ? memberExpression.Member.Name : null);
            var properties = new string[] { propertyName };

            var validationResult = new ValidationResult(errorMessage, properties);
            model.Errors.Add(validationResult);
            return validationResult;
        }

        public static ValidationResult AddError<TModel>(this TModel model, string propertyName, string errorMessage) where TModel : ServiceModel
        {
            var properties = new string[] { string.IsNullOrEmpty(propertyName) ? "" : propertyName };

            var validationResult = new ValidationResult(errorMessage, properties);
            model.Errors.Add(validationResult);
            return validationResult;
        }


        public static ValidationResult AddModelError<TModel>(this TModel model, string key, string defaultMessage = null) where TModel : ServiceModel
        {
            ILocalizationService service = IMSAppConfig.Instance.DependencyResolver.GetService<ILocalizationService>();
            Type type = typeof(TModel);

            var validationResult = new ValidationResult(service.GetLocalizedText(key, IMSAppConfig.Instance.CurrentLanguage, defaultMessage));
            model.Errors.Add(validationResult);
            return validationResult;
        }

        public static Bitmap CreateThumb(Bitmap bmp, int width, int height)
        {
            var img = new Bitmap(width, height);

            var g = Graphics.FromImage(img);
            g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            g.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;
            g.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
            g.DrawImage(bmp, 0, 0, img.Width, img.Height);
            return img;
        }

        private const string MatchEmailPattern =
            @"^(([\w-]+\.)+[\w-]+|([a-zA-Z]{1}|[\w-]{2,}))@"
     + @"((([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])\.([0-1]?
				[0-9]{1,2}|25[0-5]|2[0-4][0-9])\."
     + @"([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])\.([0-1]?
				[0-9]{1,2}|25[0-5]|2[0-4][0-9])){1}|"
     + @"([a-zA-Z0-9]+[\w-]+\.)+[a-zA-Z]{1}[a-zA-Z0-9-]{1,23})$";

        public static bool IsValidEmail(this string email)
        {
            if (string.IsNullOrEmpty(email))
                return true;

            if (email != null)
                return Regex.IsMatch(email, MatchEmailPattern);
            else
                return false;
        }

        public static bool InternetConnectionIsAvailable
        {
            get
            {
                try
                {
                    Ping ping = new Ping();
                    PingReply pingReply = ping.Send("google.com");
                    return pingReply.Status == IPStatus.Success;
                }
                catch
                {
                    return false;
                }
            }
        }

        public static Data.NTAEnum.eModule[] GetClientModule
        {
            get
            {
                return new Data.NTAEnum.eModule[]
                {
                    Data.NTAEnum.eModule.TypeApproval,
                    Data.NTAEnum.eModule.FrequencyManagement,
                    Data.NTAEnum.eModule.LicenseManagement,
                    Data.NTAEnum.eModule.ShortCodeAssignmentAndManagement,
                    Data.NTAEnum.eModule.QOSManagement,
                    Data.NTAEnum.eModule.WHOISISPDatabaseManagement
                };
            }
        }

    }
}
