﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Workshell.DiskImager.Hashing
{
    public abstract class HashValidator
    {
        protected HashValidator(string fileName, string expectedHash)
        {
            Filename = fileName;
            ExpectedHash = expectedHash;
        }

        #region Methods

        public virtual bool Validate(CancellationToken cancellationToken, Action<long, long, int> callback = null)
        {
            var totalSize = Utils.GetFileSize(Filename);
            var generatedHash = new StringBuilder(128); // Might as well set a capacity of at least 512 bits

            using (var file = new FileStream(Filename, FileMode.Open, FileAccess.Read, FileShare.Read, CommonSizes._64K, FileOptions.SequentialScan))
            {
                using (var algorithm = GetAlgorithm())
                {
                    var buffer = new byte[CommonSizes._4K];
                    var totalRead = 0L;
                    var numRead = file.Read(buffer, 0, buffer.Length);

                    while (numRead > 0 && !cancellationToken.IsCancellationRequested)
                    {
                        algorithm.TransformBlock(buffer, 0, numRead, null, 0);

                        if (callback != null)
                        {
                            totalRead += numRead;

                            callback.Invoke(totalSize, totalRead, numRead);
                        }

                        numRead = file.Read(buffer, 0, buffer.Length);
                    }

                    algorithm.TransformFinalBlock(new byte[0], 0, 0);

                    foreach (var b in algorithm.Hash)
                    {
                        generatedHash.Append(b.ToString("x2"));
                    }
                }
            }

            return string.Compare(ExpectedHash, generatedHash.ToString(), StringComparison.OrdinalIgnoreCase) == 0;
        }

        protected abstract HashAlgorithm GetAlgorithm();

        #endregion

        #region Properties

        public string Filename { get; }
        public string ExpectedHash { get; }

        #endregion
    }
}
