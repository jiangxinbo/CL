using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Tool
{
    public class Time
    {
        /// <summary>
        /// @"yyyy-MM-dd- HH:mm:ss";
        /// </summary>
        public static readonly string format = @"yyyy-MM-dd HH:mm:ss";

        /// <summary>
        /// yyyy-MM-dd
        /// </summary>
        public static readonly string dayFormat = @"yyyy-MM-dd";


        //int dayCount = DateTime.DaysInMonth(year, month);//一共有多少天

        /// <summary>
        /// 获取当前日期字符串格式 yyyy-MM-dd- HH:mm:ss
        /// </summary>
        /// <returns></returns>
        public static string getTime()
        {
            return DateTime.Now.ToString(format);
        }
        public static string getTime(string fr)
        {
            return DateTime.Now.ToString(fr);
        }

        /// <summary>
        /// 获取当前日期字符串格式
        /// </summary>
        /// <returns></returns>
        public static string getTime(DateTime dt)
        {
            return dt.ToString(format);
        }

        public static string getTime(DateTime dt, string f)
        {
            return dt.ToString(f);
        }


        /// <summary>
        /// 将字符串转换成时间格式
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public static DateTime getTime(string time, string format = null)
        {
            if (format == null) format = Time.format;
            return DateTime.ParseExact(time, format, System.Globalization.CultureInfo.CurrentCulture);
        }

        /// <summary>
        /// 根据时间戳获取字符串格式时间
        /// </summary>
        /// <param name="timeStamp"></param>
        /// <param name="format">时间格式化字符串</param>
        /// <returns></returns>
        public static string getTime(long timeStamp, string format)
        {
            DateTime start = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
            start = start.AddSeconds(timeStamp);
            return start.ToString(format);
        }

        /// <summary>
        /// 返回 yyyy-MM-DD 格式的字符串
        /// </summary>
        /// <param name="year"></param>
        /// <param name="month"></param>
        /// <param name="day"></param>
        /// <returns></returns>
        public static string getTime(int year, int month, int day)
        {
            string y = year.ToString();

            string m = "";
            if (month < 10) m = "0";
            m += month.ToString();

            string d = "";
            if (day < 10) d = "0";
            d += day.ToString();
            return string.Format("{0}-{1}-{2}", y, m, d);

        }

        /// <summary>
        /// 获取距离明天凌晨30分钟的毫秒数
        /// </summary>
        /// <returns></returns>
        public static double getBeforeDawn()
        {
            DateTime old = DateTime.Now;
            DateTime tm = old.AddDays(1);
            string tstr = string.Format("{0}-{1}-{2}", tm.Year, tm.Month, tm.Day);
            DateTime ttime = Convert.ToDateTime(tstr);// Time.getTime(tstr, "yyyy-M-d");
            ttime = ttime.AddHours(0.5);
            var interval = (ttime - old).TotalMilliseconds;
            return interval;
        }

        /// <summary>
        /// 距离第二天凌晨2点钟相差的毫秒数
        /// </summary>
        /// <returns></returns>
        public static double get1Clock()
        {
            DateTime old = DateTime.Now;
            DateTime tm = old.AddDays(1);
            string tstr = string.Format("{0}-{1}-{2}", tm.Year, tm.Month, tm.Day);
            DateTime ttime = Convert.ToDateTime(tstr); //Time.getTime(tstr, "yyyy-M-d");
            ttime = ttime.AddHours(2);
            var interval = (ttime - old).TotalMilliseconds;
            return interval;
        }


        public static DateTime getYearMonthDay(int year, int month, int day)
        {
            string tstr = string.Format("{0}-{1}-{2}", year, month, day);
            return getTime(tstr, "yyyy-M-d");
        }


        /// <summary>
        /// 判断nowDay  是否是给点年月中月份的最后一天
        /// </summary>
        /// <param name="year"></param>
        /// <param name="month"></param>
        /// <param name="nowDay"></param>
        public static bool isLastDay(int year, int month, int nowDay)
        {
            if (nowDay == DateTime.DaysInMonth(year, month))
            {
                return true;
            }
            return false;
        }


        /// <summary>
        /// 获取当前时间戳
        /// </summary>
        /// <returns></returns>
        public static long getTimeStamp()
        {
            return getTimeStamp(DateTime.Now);
        }

        /// <summary>
        /// 获取带小数的毫秒字符串
        /// </summary>
        /// <returns></returns>
        public static string getLongTimeStamp()
        {
            DateTime dtStart = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
            TimeSpan toNow = DateTime.Now.Subtract(dtStart);
            string timetamp = toNow.Ticks.ToString();
            return toNow.TotalMilliseconds.ToString();
        }

        /// <summary>
        /// 获取时间戳
        /// </summary>
        /// <returns></returns>
        public static long getTimeStamp(DateTime dtNow)
        {
            DateTime dtStart = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
            TimeSpan toNow = dtNow.Subtract(dtStart);
            string timetamp = toNow.Ticks.ToString();
            return (long)toNow.TotalSeconds;
        }


        /// <summary>
        /// 获取时间戳8位
        /// </summary>
        /// <returns></returns>
        public static long getTimeStamp8(DateTime dtNow)
        {
            DateTime dtStart = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
            TimeSpan toNow = dtNow.Subtract(dtStart);
            string timetamp = toNow.Ticks.ToString();
            return (long)toNow.TotalMinutes;
        }

        /// <summary>
        /// 获取时间戳
        /// </summary>
        /// <param name="timeStr">日期字符串</param>
        /// <param name="format">日期格式  yyyy-MM-dd</param>
        /// <returns></returns>
        public static long getTimeStamp(string timeStr, string format)
        {
            DateTime dtNow = getTime(timeStr, format);
            return getTimeStamp(dtNow);
        }


        public static void runTimer()
        {
            int taskcount = 0;
            Timer timer = new Timer((x) =>
            {
                DateTime dt = DateTime.Now;
                Task tk = null;
                if (dt.Second % 5 == 0)
                {
                    tk = Task.Factory.StartNew(() => {
                        Console.WriteLine("开始新任务");
                        var thiscount = Interlocked.Increment(ref taskcount);

                        while (taskcount <= 1)
                        {
                            Console.Write(taskcount);
                            Thread.Sleep(1000);
                        }
                        Interlocked.Decrement(ref taskcount);
                    });
                }
            },
            null/*传递对象*/,
            0,
            1000
            );
        }


    }
}
