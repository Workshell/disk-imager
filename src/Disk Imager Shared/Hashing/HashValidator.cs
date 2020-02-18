#region License
//  Copyright(c) Workshell Ltd
//
//  Permission is hereby granted, free of charge, to any person obtaining a copy
//  of this software and associated documentation files (the "Software"), to deal
//  in the Software without restriction, including without limitation the rights
//  to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
//  copies of the Software, and to permit persons to whom the Software is
//  furnished to do so, subject to the following conditions:
//
//  The above copyright notice and this permission notice shall be included in all
//  copies or substantial portions of the Software.
//
//  THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//  IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//  FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
//  AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//  LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//  OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
//  SOFTWARE.
#endregion

using System;
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
