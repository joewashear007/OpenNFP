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
        /// <summary>
        /// https://stackoverflow.com/a/33104162
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <param name="items"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public static void AddRange<T>(this IList<T> list, IEnumerable<T> items)
        {
            if (list == null) throw new ArgumentNullException(nameof(list));
            if (items == null) throw new ArgumentNullException(nameof(items));

            if (list is List<T> asList)
            {
                asList.AddRange(items);
            }
            else
            {
                foreach (var item in items)
                {
                    list.Add(item);
                }
            }
        }

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
