﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Workshell.DiskImager.Hashing
{
    public sealed class SHA1HashWriter : HashSumWriter
    {
        public SHA1HashWriter(string imageFilename) : base(imageFilename)
        {
        }

        #region Methods

        protected override HashAlgorithm GetAlgorithm()
        {
            return SHA1.Create();
        }

        protected override string GetExtension()
        {
            return "sha1";
        }

        #endregion
    }
}
