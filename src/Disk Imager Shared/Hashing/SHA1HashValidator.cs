using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Workshell.DiskImager.Hashing
{
    public sealed class SHA1HashValidator : HashValidator
    {
        public SHA1HashValidator(string fileName, string expectedHash) : base(fileName, expectedHash)
        {
        }

        #region Methods

        protected override HashAlgorithm GetAlgorithm()
        {
            return SHA1.Create();
        }

        #endregion
    }
}
