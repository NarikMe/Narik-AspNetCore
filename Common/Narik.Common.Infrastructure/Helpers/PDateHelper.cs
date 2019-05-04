using System;
using System.Globalization;

namespace Narik.Common.Infrastructure.Helpers
{
    /// <summary>
    /// Persian Date Helper class
    /// </summary>
    /// <author>
    ///   <name>Vahid Nasiri</name>
    ///   <email>vahid_nasiri@yahoo.com</email>
    /// </author>    
    public class PDateHelper
    {
        #region Methods (5)



        public static string[] PersianMonths =
        {
            "فروردین" , "اردیبهشت", "خرداد", "تیر", "مرداد", "شهریور", "مهر", "آبان", "آذر", "دی", "بهمن", "اسفند" 

        };
       

        // Public Methods (5) 
        /// <summary>
        /// Finds 1st day of the given year and month.
        /// </summary>
        /// <param name="year"></param>
        /// <param name="monthIndex"></param>
        /// <param name="day"></param>
        /// <returns></returns>
        public static int FindDayOfMonth(int year, int monthIndex,int day)
        {
            int dayWeek = 1;
           // HijriToGregorian(year, monthIndex, 1, out outYear, out outMonth, out outDay);

            var res = new DateTime(year, monthIndex, day);

            switch (res.DayOfWeek)
            {
                case DayOfWeek.Saturday:
                    dayWeek = 0;
                    break;

                case DayOfWeek.Sunday:
                    dayWeek = 1;
                    break;

                case DayOfWeek.Monday:
                    dayWeek = 2;
                    break;

                case DayOfWeek.Tuesday:
                    dayWeek = 3;
                    break;

                case DayOfWeek.Wednesday:
                    dayWeek = 4;
                    break;

                case DayOfWeek.Thursday:
                    dayWeek = 5;
                    break;

                case DayOfWeek.Friday:
                    dayWeek = 6;
                    break;
            }

            return dayWeek;
        }

        /// <summary>
        /// Converts Gregorian date To Hijri date.
        /// </summary>
        /// <param name="inYear"></param>
        /// <param name="inMonth"></param>
        /// <param name="inDay"></param>
        /// <param name="outYear"></param>
        /// <param name="outMonth"></param>
        /// <param name="outDay"></param>
        /// <returns></returns>
        public static bool GregorianToHijri(int inYear, int inMonth, int inDay,
                                            out int outYear, out int outMonth, out int outDay)
        {
            try
            {
                var ym = inYear;
                var mm = inMonth;
                var dm = inDay;

                var sss = new PersianCalendar();
                outYear = sss.GetYear(new DateTime(ym, mm, dm, new GregorianCalendar()));
                outMonth = sss.GetMonth(new DateTime(ym, mm, dm, new GregorianCalendar()));
                outDay = sss.GetDayOfMonth(new DateTime(ym, mm, dm, new GregorianCalendar()));
                return true;
            }
            catch //invalid date
            {
                outYear = -1;
                outMonth = -1;
                outDay = -1;
                return false;
            }
        }

        /// <summary>
        /// Converts Hijri date To Gregorian date.
        /// </summary>
        /// <param name="inYear"></param>
        /// <param name="inMonth"></param>
        /// <param name="inDay"></param>
        /// <param name="outYear"></param>
        /// <param name="outMonth"></param>
        /// <param name="outDay"></param>
        /// <returns></returns>
        public static bool HijriToGregorian(
                    int inYear, int inMonth, int inDay,
                    out int outYear, out int outMonth, out int outDay)
        {
            try
            {
                var ys = inYear;
                var ms = inMonth;
                var ds = inDay;

                var sss = new GregorianCalendar();
                outYear = sss.GetYear(new DateTime(ys, ms, ds, new PersianCalendar()));
                outMonth = sss.GetMonth(new DateTime(ys, ms, ds, new PersianCalendar()));
                outDay = sss.GetDayOfMonth(new DateTime(ys, ms, ds, new PersianCalendar()));

                return true;
            }
            catch //invalid date
            {
                outYear = -1;
                outMonth = -1;
                outDay = -1;
                return false;
            }
        }

        /// <summary>
        /// Is a given year leap?
        /// </summary>
        /// <param name="year"></param>
        /// <returns></returns>
        public static bool IsLeapYear(int year)
        {
            var r = year % 33;
            return (r == 1 || r == 5 || r == 9 || r == 13 || r == 17 || r == 22 || r == 26 || r == 30);
        }

        /// <summary>
        /// Is a given date valid?
        /// </summary>
        /// <param name="year"></param>
        /// <param name="month"></param>
        /// <param name="day"></param>
        /// <returns></returns>
        public static bool IsValid(int year, int month, int day)
        {
            if (month < 1 || month > 12) return false;
            if (day < 1) return false;
            if (month < 7 && day > 31) return false;
            if (month >= 7 && day > 30) return false;
            return month != 12 || day <= 29 || IsLeapYear(year);
        }

        public static string IsValid(string inData)
        {
            if (string.IsNullOrWhiteSpace(inData))
            {
                return "لطفا تاريخي را وارد نمائيد.";
            }

            var parts = inData.Split('/');
            if (parts.Length != 3)
            {
                return ("تاريخ شمسي وارد شده معتبر نيست.");
            }

            int year;
            if (!int.TryParse(parts[0], out year))
            {
                return ("سال شمسي وارد شده معتبر نيست.");
            }

            int month;
            if (!int.TryParse(parts[1], out month))
            {
                return ("ماه شمسي وارد شده معتبر نيست.");
            }

            int day;
            if (!int.TryParse(parts[2], out day))
            {
                return ("روز شمسي وارد شده معتبر نيست.");
            }

            if (!IsValid(year, month, day))
            {
                return ("تاريخ شمسي وارد شده معتبر نيست.");
            }
            return null;

        }
        /// <summary>
        /// Extracts year/month/day parts of a given Persian date string.
        /// </summary>
        /// <param name="inData">Persian date</param>
        /// <returns></returns>
        public static Tuple<int, int, int> ExtractPersianDateParts(string inData)
        {
            if (string.IsNullOrWhiteSpace(inData))
            {
                throw new Exception("لطفا تاريخي را وارد نمائيد.");
            }

            var parts = inData.Split('/');
            if (parts.Length != 3)
            {
                throw new Exception("تاريخ شمسي وارد شده معتبر نيست.");
            }

            int year;
            if (!int.TryParse(parts[0], out year))
            {
                throw new Exception("سال شمسي وارد شده معتبر نيست.");
            }

            int month;
            if (!int.TryParse(parts[1], out month))
            {
                throw new Exception("ماه شمسي وارد شده معتبر نيست.");
            }

            int day;
            if (!int.TryParse(parts[2], out day))
            {
                throw new Exception("روز شمسي وارد شده معتبر نيست.");
            }

            if (!IsValid(year, month, day))
            {
                throw new Exception("تاريخ شمسي وارد شده معتبر نيست.");
            }

            return new Tuple<int, int, int>(year, month, day);
        }

        #endregion Methods

        public static DateTime? HijriToGregorian(string date)
        {
            if (string.IsNullOrEmpty(date))
                return null;
            if (date.Length != 10)
                date = "13" + date;
            int iyear, imonth, iday;
            HijriToGregorian(Convert.ToInt32(date.Substring(0,4)), Convert.ToInt32(date.Substring(5,2)), Convert.ToInt32(date.Substring(8,2)), out iyear, out imonth, out iday);
            return new DateTime(iyear,imonth,iday);
        }

        public static string GregorianToHijri(DateTime? date)
        {
            if (date==null || date==DateTime.MinValue)
                return null;
            int iyear, imonth, iday;
            GregorianToHijri(date.Value.Year,date.Value.Month ,date.Value.Day, out iyear, out imonth, out iday);
            return $"{iyear}/{imonth.ToString().PadLeft(2, '0')}/{iday.ToString().PadLeft(2, '0')}";
        }
        public static string GregorianToTitleHijri(DateTime? date)
        {
            if (date == null || date == DateTime.MinValue)
                return null;
            int iyear, imonth, iday;
            GregorianToHijri(date.Value.Year, date.Value.Month, date.Value.Day, out iyear, out imonth, out iday);

            var monthTitle = PersianMonths[imonth-1];
            return $"{iday.ToString().PadLeft(2, '0')}  {monthTitle}   {iyear}";
        }
    }
}
