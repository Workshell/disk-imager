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
    public abstract class HashSumWriter : HashWriter
    {
        protected HashSumWriter(string imageFilename) : base(imageFilename)
        {
        }

        #region Methods

        public override void Generate(CancellationToken cancellationToken, Action<long, long, int> callback = null)
        {
            var hash = string.Empty;

            using (var algorithm = GetAlgorithm())
            {
                using (var imageFile = new FileStream(ImageFilename, FileMode.Open, FileAccess.Read, FileShare.Read, CommonSizes._64K, FileOptions.SequentialScan))
                {
                    var buffer = new byte[CommonSizes._4K];
                    var totalRead = 0L;
                    var numRead = imageFile.Read(buffer, 0, buffer.Length);

                    while (numRead > 0 && !cancellationToken.IsCancellationRequested)
                    {
                        totalRead += numRead;

                        algorithm.TransformBlock(buffer, 0, numRead, null, 0);
                        callback?.Invoke(imageFile.Length, totalRead, numRead);

                        numRead = imageFile.Read(buffer, 0, buffer.Length);
                    }

                    algorithm.TransformFinalBlock(new byte[0], 0, 0);
                    callback?.Invoke(imageFile.Length, totalRead, numRead);
                }

                hash = Utils.BytesToString(algorithm.Hash);
            }

            var file = new FileStream(HashFilename, FileMode.Create, FileAccess.Write, FileShare.None, CommonSizes._64K, FileOptions.SequentialScan);

            using (var writer = new StreamWriter(file, Encoding.UTF8))
            {
                writer.WriteLine($"{hash} {Path.GetFileName(ImageFilename)}");
                writer.Flush();
            }
        }

        protected abstract HashAlgorithm GetAlgorithm();
        protected abstract string GetExtension();

        #endregion

        #region Properties

        public override string HashFilename => $"{ImageFilename}.{GetExtension()}";

        #endregion
    }
}
