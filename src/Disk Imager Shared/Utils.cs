﻿#region License
//  Copyright(c) Workshell Ltd
//
//  Permission is hereby granted, free of charge, to any person obtaining a copy
//  of this software and associated documentation files (the "Software"), to deal
//  in the Software without restriction, including without limitation the rights
//  to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
//  copies of the Software, and to permit persons to whom the Software is
//  furnished to do so, subject to the following conditions:
//
//  The above copyright notice and this permission notice shall be included in all
//  copies or substantial portions of the Software.
//
//  THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//  IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//  FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
//  AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//  LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//  OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
//  SOFTWARE.
#endregion

using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Workshell.DiskImager
{
    public static class Utils
    {
        private static readonly string[] suf = { "Bytes", "KB", "MB", "GB", "TB", "PB", "EB" };

        #region Methods

        public static bool IsNullOrEmpty<T>(T[] array)
        {
            return (array == null || array.Length == 0);
        }

        public static T[] Add<T>(T[] array, T item)
        {
            var result = new T[array.Length + 1];

            Array.Copy(array, 0, result, 0, array.Length);

            result[result.Length - 1] = item;

            return result;
        }

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

        public static string BuildDialogFilters(string sourceFilters)
        {
            var filters = new string[0];

            if (!string.IsNullOrWhiteSpace(sourceFilters))
            {
                filters = sourceFilters.Split(new string[] { Environment.NewLine, "\r\n", "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries);
            }

            return string.Join("|", filters);
        }

        public static long GetFileSize(string fileName)
        {
            var info = new FileInfo(fileName);

            if (!info.Exists)
            {
                return 0;
            }

            return info.Length;
        }

        public static string GetVersion()
        {
            var assembly = Assembly.GetEntryAssembly();
            var assemblyName = AssemblyName.GetAssemblyName(assembly.Location);
            var result = $"{assemblyName.Version.Major}.{assemblyName.Version.Minor}.{assemblyName.Version.Build}";

            return result;
        }

        public static void ShowAboutDialog(string title, string version = null, Image icon = null)
        {
            if (string.IsNullOrWhiteSpace(version))
            {
                version = GetVersion();
            }

            var dialog = new AboutForm(title, version, icon);

            try
            {
                dialog.ShowDialog();
            }
            finally
            {
                dialog.Close();
                GC.Collect();
            }
        }

        #endregion
    }
}
