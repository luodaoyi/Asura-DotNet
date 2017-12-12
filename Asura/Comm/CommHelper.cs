using System;
using System.Runtime.InteropServices.ComTypes;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Diagnostics;

namespace Asura.Comm
{
    public static class CommHelper
    {
        /// <summary>
        /// 将时间转换为间隔
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string ConvertStr(string str)
        {
            var t = DateTime.Parse(str);
            var now = DateTime.UtcNow;

            var y1 = t.Year;
            var m1 = t.Month;
            var d1 = t.Day;

            var y2 = now.Year;
            var m2 = now.Month;
            var d2 = now.Day;

            var h1 = t.Hour;
            var mi1 = t.Minute;
            var s1 = t.Second;

            var h2 = now.Hour;
            var mi2 = now.Minute;
            var s2 = now.Second;

            var y = y2 - y1;
            var m = y * 12 + (m2 - m1);
            var d = m * DateTime.DaysInMonth(y1, m1) + d2 - d1;
            var h = d * 24 + h2 - h1;
            var mi = h * 60 + mi2 - mi1;

            if (y > 1 || (y == 1 && m2 - m1 >= 0))
                return $"{y}年前";
            else if (m > 1 || (m == 1 && d2 - d1 >= 0))
                return $"{m}月前";
            else if (d > 1 || (d == 1 && h2 - h1 >= 0))
                return $"{d}天前";
            else if (h > 1 || (h == 1 && mi2 - mi1 >= 0))
                return $"{h}小时前";
            else if (mi > 1 || (mi == 1 && s2 - s1 >= 0))
                return $"{mi}分钟前";
            return "几秒前";
        }
    }
}