using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Workshell.DiskImager.Extensions
{
    public static class DisposableExtensions
    {
        #region Methods

        public static void Dispose(this IDisposable disposable, Action callback = null)
        {
            if (disposable != null)
            {
                disposable.Dispose();
                callback?.Invoke();
            }
        }

        #endregion
    }
}
