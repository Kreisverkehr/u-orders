using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UOrders.Service.Extensions
{
    internal static class DateTimeExtensions
    {
        public static DateTime NextFullHour(this DateTime dt) =>
            dt.Date.AddHours(dt.Hour + 1);

        public static DateTime NextDay(this DateTime dt) =>
            dt.Date.AddDays(1);

        public static DateTime NextQuarterPastHour(this DateTime dt) =>
            dt.Minute switch
            {
                >= 15 => dt.Date.AddHours(dt.Hour + 1).AddMinutes(15),
                < 15 => dt.Date.AddHours(dt.Hour).AddMinutes(15),
            };

        public static DateTime NextHalfHour(this DateTime dt) =>
            dt.Minute switch
            {
                >= 30 => dt.Date.AddHours(dt.Hour + 1).AddMinutes(30),
                < 30 => dt.Date.AddHours(dt.Hour).AddMinutes(30),
            };

        public static DateTime NextThreeQuarterPastHour(this DateTime dt) =>
            dt.Minute switch
            {
                >= 45 => dt.Date.AddHours(dt.Hour + 1).AddMinutes(45),
                < 45 => dt.Date.AddHours(dt.Hour).AddMinutes(45),
            };
    }
}
