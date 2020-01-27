﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Workshell.DiskImager.Hashing
{
    public sealed class MD5HashValidator : HashValidator
    {
        public MD5HashValidator(string fileName, string expectedHash) : base(fileName, expectedHash)
        {
        }

        #region Methods

        protected override HashAlgorithm GetAlgorithm()
        {
            return MD5.Create();
        }

        #endregion
    }
}