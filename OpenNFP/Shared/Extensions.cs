using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNFP.Shared
{
    public static class Extensions
    {
        public static string ToKey(this DateTime date)
        {
            return date.ToString("yyyyMMdd");
        }
        public static string ToKey(this DateTime? date)
        {
            if (date.HasValue)
            {
                return date.Value.ToString("yyyyMMdd");
            }
            else
            {
                return "";
            }
        }

        public static string ToKey(this DateOnly date)
        {
            return date.ToString("yyyyMMdd");
        }
        public static string ToKey(this DateOnly? date)
        {
            if (date.HasValue)
            {
                return date.Value.ToString("yyyyMMdd");
            }
            else
            {
                return "";
            }
        }

        public static DateTime? ToDateTime(this string self)
        {
            if (DateTime.TryParseExact(self, "yyyyMMdd", null, DateTimeStyles.None, out DateTime vaule))
            {
                return vaule;
            }
            else
            {
                return null;
            }
        }
    }
}
