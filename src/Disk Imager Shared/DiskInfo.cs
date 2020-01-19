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
            var diskInformation = new Dictionary<int, DiskInfo>();

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

                    diskInfo.Drives = ArrayUtils.Add<DriveInfo>(diskInfo.Drives, drive)
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

            if (!ArrayUtils.IsNullOrEmpty(Drives))
            {
                var drives = string.Join(", ", Drives.Select(_ => _.Name));

                result = $"{result} ({drives})";
            }

            return result;
        }

        #endregion

        #region Properties

        public int DiskNumber { get; private set; }
        public long Size { get; private set; }
        public int SectorSize { get; private set; }
        public long SectorCount { get; private set; }
        public DriveInfo[] Drives { get; private set; }

        #endregion
    }
}
