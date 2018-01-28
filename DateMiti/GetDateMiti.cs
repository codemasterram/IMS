using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Xml;

namespace DateMiti
{
    public static class GetDateMiti
    {
        private static string _separator;

        public static string Separator
        {
            get
            {
                return _separator == null ? CultureInfo.CreateSpecificCulture("en-US").DateTimeFormat.DateSeparator : _separator;
            }
            set
            {
                _separator = value;
            }
        }

        public static string GetMiti(this DateTime? date)
        {
            if (date == null)
                return null;
            DateTime dateToConvert = ((DateTime)date).Date;
            try
            {
                DateTime[] dates;
                TimeSpan ts = new TimeSpan();
                int day, month, year;

                year = dateToConvert.Year + 56;
                month = dateToConvert.Month + 8;
                if (month > 12)
                {
                    month -= 12;
                    year++;
                }
                dates = GetMonth(year, month);
                ts = dates[1].Subtract(dates[0]);
                ts = ts.Add(new TimeSpan(1, 0, 0, 0));
                day = 1;
                for (; dates[0] != dateToConvert; dates[0] = dates[0].AddDays(1))
                {
                    day++;
                }
                if (day > ts.Days)
                {
                    day -= ts.Days;
                    month++;
                    if (month > 12)
                    {
                        month -= 12;
                        year++;
                    }
                }
                return $"{year}{Separator}{month.ToString("00")}{Separator}{day.ToString("00")}";
            }
            catch { return null; }
        }

        public static string GetMiti(this DateTime date)
        {
            DateTime dateToConvert = ((DateTime)date).Date;
            try
            {
                DateTime[] dates;
                TimeSpan ts = new TimeSpan();
                int day, month, year;

                year = dateToConvert.Year + 56;
                month = dateToConvert.Month + 8;
                if (month > 12)
                {
                    month -= 12;
                    year++;
                }
                dates = GetMonth(year, month);
                ts = dates[1].Subtract(dates[0]);
                ts = ts.Add(new TimeSpan(1, 0, 0, 0));
                day = 1;
                for (; dates[0] != dateToConvert; dates[0] = dates[0].AddDays(1))
                {
                    day++;
                }
                if (day > ts.Days)
                {
                    day -= ts.Days;
                    month++;
                    if (month > 12)
                    {
                        month -= 12;
                        year++;
                    }
                }
                return $"{year}{Separator}{month.ToString("00")}{Separator}{day.ToString("00")}";
            }
            catch { return null; }
        }

        public static DateTime GetDate(this string miti)
        {
            try
            {
                DateTime english = new DateTime();
                DateTime[] dates;
                int day, year, month, ctr;

                int yr;
                int.TryParse(miti.Substring(0, 4), out yr);
                int mnth;
                int.TryParse(miti.Substring(5, 2), out mnth);
                int dy;
                int.TryParse(miti.Substring(8, 2), out dy);

                dates = GetMonth(yr, mnth);
                year = dates[0].Year;
                month = dates[0].Month;
                day = dates[0].Day;
                TimeSpan diff = dates[1].Subtract(dates[0]);
                diff = diff.Add(new TimeSpan(1, 0, 0, 0));
                if (dy > diff.Days)
                    return DateTime.MinValue;

                english = new DateTime(dates[0].Year, dates[0].Month, dates[0].Day);
                for (ctr = 1; ctr < dy; ctr++)
                {
                    english = english.AddDays(1);
                }
                return english;
            }
            catch
            {
                return DateTime.MinValue;
            }
        }

        public static int MitiValue(this string miti)
        {
            if (string.IsNullOrEmpty(miti))
                return 0;
            return miti.ExtractNumber();
        }

        private static int ExtractNumber(this string s)
        {
            s = s.TrimStart().Trim();
            string str = string.Empty;
            int val = 0;
            for (int i = 0; i < s.Length; i++)
            {
                if (Char.IsDigit(s[i]) || s[i] == '.')
                    str += s[i];
            }
            if (str.Length > 0)
                int.TryParse(str, out val);
            return val;
        }

        public static int GetMitiValue(this DateTime date)
        {
            return MitiValue(GetDateMiti.GetMiti(date));
        }

        public static DateTime[] GetMonth(int yy, int mm)
        {
            string s;
            DateTime[] dates = new DateTime[2];
            int year = yy;
            year = year - year % 10;
            Stream stream = typeof(DateMiti.GetDateMiti).Assembly.GetManifestResourceStream("DateMiti.Library." + year.ToString() + ".xml");
            XmlReader xr = XmlReader.Create(stream);
            while (xr.Read())
            {
                if (xr.NodeType == XmlNodeType.Element && xr.Name == "Year")
                {
                    s = xr.GetAttribute("yearid");
                    if (Convert.ToInt32(s) == yy)
                    {
                        while (xr.Read())
                        {
                            if (xr.NodeType == XmlNodeType.Element && xr.Name == "Month")
                            {
                                s = xr.GetAttribute("id");
                                if (Convert.ToInt32(s) == ((int)mm))
                                {
                                    while (xr.Read())
                                    {
                                        if (xr.NodeType == XmlNodeType.Element && xr.Name == "StartDate")
                                        {
                                            xr.Read();
                                            string[] fdate = xr.Value.Split('/');
                                            dates[0] = new DateTime(int.Parse(fdate[2]), int.Parse(fdate[0]), int.Parse(fdate[1]));
                                        }
                                        if (xr.NodeType == XmlNodeType.Element && xr.Name == "EndDate")
                                        {
                                            xr.Read();
                                            string[] fdate = xr.Value.Split('/');
                                            dates[1] = new DateTime(int.Parse(fdate[2]), int.Parse(fdate[0]), int.Parse(fdate[1]));
                                            break;
                                        }
                                    }
                                    xr.Close();
                                    return dates;
                                }
                            }
                        }
                    }
                }
            }
            xr.Close();
            return null;
        }

        public static DateTime[] DatesInMonth(int yyyy, int mm)
        {
            var dates = GetMonth(yyyy, mm);
            var totalDays = TotalDaysInMonth(yyyy, mm);
            List<DateTime> returnVal = new List<DateTime>();
            for (double i = 0; i < totalDays; i++)
            {
                returnVal.Add(dates[0].AddDays(i));
            }
            return returnVal.ToArray();
        }

        public static int TotalDaysInMonth(int yyyy, int mm)
        {
            var dates = GetMonth(yyyy, mm);
            var datediff = (dates[1] - dates[0]).TotalDays;
            var lastMiti = DateMiti.GetDateMiti.GetMiti(dates[1]);
            var totalDays = lastMiti.Substring(lastMiti.Length - 2, 2);
            return Convert.ToInt16(totalDays);
        }

        public static List<DateInMonth> GetDateInMonth(int yyyy, int mm)
        {
            List<DateInMonth> data = new List<DateMiti.DateInMonth>();
            DateTime[] dates = DatesInMonth(yyyy, mm);
            foreach (var date in dates)
            {
                DateInMonth dm = new DateInMonth();
                dm.Date = date;
                dm.Miti = GetMiti(date);
                dm.DateValueAD = date.Day;
                dm.DateValueBS = Convert.ToInt32(GetMiti(date).Split(Convert.ToChar(Separator)).LastOrDefault());//.Split('-').LastOrDefault());
                dm.DayOfWeek = date.DayOfWeek;
                data.Add(dm);
            }

            return data;
        }

        public static int CurrentYear
        {
            get
            {
                return Convert.ToInt16(GetMiti(DateTime.Today).Substring(0, 4));
            }
        }

        public static int CurrentMonth
        {
            get
            {
                return Convert.ToInt16(DateMiti.GetDateMiti.GetMiti(DateTime.Today).Substring(5, 2));
            }
        }

        public static string CurrentMonthName(int currentMonth = 0)
        {
            var currentMonthVal = currentMonth == 0 ? CurrentMonth : currentMonth;
            var retVal = "";
            switch (currentMonthVal)
            {
                case 1:
                    retVal = "Baishakh";
                    break;
                case 2:
                    retVal = "Jestha";
                    break;
                case 3:
                    retVal = "Ashadh";
                    break;
                case 4:
                    retVal = "Srawan";
                    break;
                case 5:
                    retVal = "Bhadra";
                    break;
                case 6:
                    retVal = "Aswin";
                    break;
                case 7:
                    retVal = "Kartik";
                    break;
                case 8:
                    retVal = "Mansir";
                    break;
                case 9:
                    retVal = "Paush";
                    break;
                case 10:
                    retVal = "Marg";
                    break;
                case 11:
                    retVal = "Falgun";
                    break;
                case 12:
                    retVal = "Chaitra";
                    break;

            }
            return retVal;
        }
    }


    public class DateInMonth
    {
        public DateTime Date { get; set; }
        public string Miti { get; set; }
        public int DateValueAD { get; set; }
        public int DateValueBS { get; set; }
        public DayOfWeek DayOfWeek { get; set; }
    }
}