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

            var file = new FileStream(HashFilename, FileMode.Open, FileAccess.Read, FileShare.Read);

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
