using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Workshell.DiskImager.Hashing
{
    public sealed class SHA512HashWriter : HashSumWriter
    {
        public SHA512HashWriter(string imageFilename) : base(imageFilename)
        {
        }

        #region Methods

        protected override HashAlgorithm GetAlgorithm()
        {
            return SHA512.Create();
        }

        protected override string GetExtension()
        {
            return "sha512";
        }

        #endregion
    }
}
