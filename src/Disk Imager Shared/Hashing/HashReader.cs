using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Workshell.DiskImager.Hashing
{
    public abstract class HashReader
    {
        protected HashReader(string fileName)
        {
            HashFilename = fileName;
        }

        #region Methods

        public abstract IDictionary<string, string> Read();

        #endregion

        #region Properties

        public string HashFilename { get; }

        #endregion
    }
}
