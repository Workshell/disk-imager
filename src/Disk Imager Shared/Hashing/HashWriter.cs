using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Workshell.DiskImager.Hashing
{
    public abstract class HashWriter
    {
        protected HashWriter(string imageFilename)
        {
            ImageFilename = imageFilename;
        }

        #region Methods

        public abstract void Generate(Func<long, long, int, bool> callback = null);

        #endregion

        #region Properties

        public string ImageFilename { get; }

        #endregion
    }
}
