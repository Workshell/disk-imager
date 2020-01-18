using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Workshell.DiskImager.Hashing
{
    public sealed class MD5HashWriter : HashSumWriter
    {
        public MD5HashWriter(string imageFilename) : base(imageFilename)
        {
        }

        #region Methods

        protected override HashAlgorithm GetAlgorithm()
        {
            return MD5.Create();
        }

        protected override string GetExtension()
        {
            return "md5";
        }

        #endregion
    }
}
