using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Workshell.DiskImager.Hashing
{
    public abstract class HashSumWriter : HashWriter
    {
        protected HashSumWriter(string imageFilename) : base(imageFilename)
        {
        }

        #region Methods

        public override void Generate(Func<long, long, int, bool> callback = null)
        {
            var hash = string.Empty;

            using (var algorithm = GetAlgorithm())
            {
                using (var imageFile = new FileStream(ImageFilename, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    var buffer = new byte[1024 * 64];
                    var totalRead = 0L;
                    var numRead = imageFile.Read(buffer, 0, buffer.Length);

                    while (numRead > 0)
                    {
                        totalRead += numRead;

                        algorithm.TransformBlock(buffer, 0, numRead, null, 0);

                        if (callback != null && !callback.Invoke(imageFile.Length, totalRead, numRead))
                        {
                            return;
                        }

                        numRead = imageFile.Read(buffer, 0, buffer.Length);
                    }

                    algorithm.TransformFinalBlock(new byte[0], 0, 0);

                    if (callback != null && !callback.Invoke(imageFile.Length, totalRead, numRead))
                    {
                        return;
                    }
                }

                hash = Utils.BytesToString(algorithm.Hash);
            }

            var file = new FileStream(HashFilename, FileMode.Create, FileAccess.Write, FileShare.None);

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
