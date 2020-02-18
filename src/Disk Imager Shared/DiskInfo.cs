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
using System.Text;
using System.Threading.Tasks;

namespace Workshell.DiskImager
{
    public sealed class DiskInfo
    {
        #region Static Methods

        public static DiskInfo[] GetDisks(bool onlyRemovable = true, bool onlyReady = true)
        {
            var drives = DriveInfo.GetDrives();

            return GetDisks(drives, onlyRemovable, onlyReady);
        }

        public static DiskInfo[] GetDisks(DriveInfo[] drives, bool onlyRemovable = true, bool onlyReady = true)
        {
            var diskInformation = new Dictionary<ushort, DiskInfo>();

            foreach (var drive in drives)
            {
                if (onlyReady && !drive.IsReady)
                {
                    continue;
                }

                if (onlyRemovable && drive.DriveType != DriveType.Removable)
                {
                    continue;
                }

                var diskNumbers = DiskUtils.GetPhysicalDiskNumbersFromDrive(drive.RootDirectory.Name);

                foreach (var diskNumber in diskNumbers)
                {
                    DiskInfo diskInfo;

                    if (!diskInformation.TryGetValue(diskNumber, out diskInfo))
                    {
                        diskInfo = new DiskInfo()
                        {
                            DiskNumber = diskNumber,
                            Drives = new DriveInfo[0]
                        };

                        diskInformation.Add(diskNumber, diskInfo);
                    }

                    diskInfo.Drives = Utils.Add<DriveInfo>(diskInfo.Drives, drive)
                        .OrderBy(_ => _.Name)
                        .ToArray();
                }
            }

            foreach (var kvp in diskInformation)
            {
                kvp.Value.Size = DiskUtils.GetPhysicalDiskSize(kvp.Key);
                kvp.Value.SectorSize = DiskUtils.GetPhysicalDiskSectorSize(kvp.Key);
                kvp.Value.SectorCount = DiskUtils.GetPhysicalDiskSectorCount(kvp.Key);
            }

            var results = diskInformation.Values.OrderBy(_ => _.DiskNumber)
                .ToArray();

            return results;
        }

        #endregion

        #region Methods

        public override string ToString()
        {
            var result = $"Disk {DiskNumber}";

            if (!Utils.IsNullOrEmpty(Drives))
            {
                var drives = string.Join(", ", Drives.Select(_ => _.Name));

                result = $"{result} ({drives})";
            }

            return result;
        }

        #endregion

        #region Properties

        public ushort DiskNumber { get; private set; }
        public long Size { get; private set; }
        public int SectorSize { get; private set; }
        public long SectorCount { get; private set; }
        public DriveInfo[] Drives { get; private set; }

        #endregion
    }
}
