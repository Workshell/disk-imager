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
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Workshell.DiskImager.Hashing
{
    public sealed class HashSumReader : HashReader
    {
        public HashSumReader(string fileName) : base(fileName)
        {
        }

        #region Methods

        public override IDictionary<string, string> Read()
        {
            var results = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            if (!File.Exists(HashFilename))
            {
                return results;
            }

            var file = new FileStream(HashFilename, FileMode.Open, FileAccess.Read, FileShare.Read, CommonSizes._64K, FileOptions.SequentialScan);

            using (var reader = new StreamReader(file, Encoding.UTF8, true))
            {
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();

                    if (line == null)
                    {
                        break;
                    }

                    if (string.IsNullOrWhiteSpace(line))
                    {
                        continue;
                    }

                    var matches = Regex.Matches(line, @"([a-f0-9]+)\s+(.+)", RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace);

                    foreach (Match match in matches)
                    {
                        if (match.Groups.Count < 3)
                        {
                            continue;
                        }

                        var hash = match.Groups[1].Value;
                        var fileName = match.Groups[2].Value;

                        if (string.IsNullOrWhiteSpace(hash) || string.IsNullOrWhiteSpace(fileName))
                        {
                            continue;
                        }

                        results[fileName] = hash;
                    }
                }
            }

            return results;
        }

        #endregion
    }
}
