using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Workshell.DiskImager
{
    public static class Utils
    {
        private static readonly string[] suf = { "Bytes", "KB", "MB", "GB", "TB", "PB", "EB" };

        #region Methods

        public static string FormatBytes(long byteCount)
        {
            if (byteCount == 0)
            {
                return "0 Bytes";
            }

            var bytes = Math.Abs(byteCount);
            var place = Convert.ToInt32(Math.Floor(Math.Log(bytes, 1024)));
            var num = Math.Round(bytes / Math.Pow(1024, place), 1);

            return $"{(Math.Sign(byteCount) * num)} {suf[place]}";
        }

        public static string BytesToString(byte[] bytes, bool upperCase = false)
        {
            var builder = new StringBuilder(bytes.Length * 2);

            for (var i = 0; i < bytes.Length; i++)
            {
                builder.Append($"{bytes[i]:x2}");
            }

            var result = builder.ToString();

            if (upperCase)
            {
                result = result.ToUpperInvariant();
            }

            return result;
        }

        #endregion
    }
}
