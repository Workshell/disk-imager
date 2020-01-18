using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Workshell.DiskImager
{
    public static class ArrayUtils
    {
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

        #endregion
    }
}
