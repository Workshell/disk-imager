using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Workshell.DiskImager
{
    internal static class DiskUtils
    {
        #region Methods

        public static long GetPhysicalDiskSize(int diskNumber)
        {
            var drivePath = $"\\\\.\\PhysicalDrive{diskNumber}";
            var driveHandle = NativeInterop.CreateFile(drivePath, 0, FileShare.ReadWrite, IntPtr.Zero, FileMode.Open, 0, IntPtr.Zero);

            if (driveHandle.IsInvalid)
            {
                return -1;
            }

            try
            {
                var geometry = new NativeInterop.DISK_GEOMETRY();
                var geometrySize = 0U;
                var success = NativeInterop.DeviceIoControl(driveHandle, NativeInterop.IOCTL_DISK_GET_DRIVE_GEOMETRY, IntPtr.Zero, 0, ref geometry, (uint)Marshal.SizeOf(geometry), out geometrySize, IntPtr.Zero);

                if (!success)
                {
                    return -1;
                }

                var result = geometry.BytesPerSector * geometry.SectorsPerTrack * geometry.TracksPerCylinder * geometry.Cylinders;

                return result;
            }
            finally
            {
                if (!driveHandle.IsClosed)
                {
                    driveHandle.Close();
                }
            }
        }

        public static int GetPhysicalDiskSectorSize(int diskNumber)
        {
            var drivePath = $"\\\\.\\PhysicalDrive{diskNumber}";
            var driveHandle = NativeInterop.CreateFile(drivePath, 0, FileShare.ReadWrite, IntPtr.Zero, FileMode.Open, 0, IntPtr.Zero);

            if (driveHandle.IsInvalid)
            {
                return -1;
            }

            try
            {
                var geometry = new NativeInterop.DISK_GEOMETRY();
                var geometrySize = 0U;
                var success = NativeInterop.DeviceIoControl(driveHandle, NativeInterop.IOCTL_DISK_GET_DRIVE_GEOMETRY, IntPtr.Zero, 0, ref geometry, (uint)Marshal.SizeOf(geometry), out geometrySize, IntPtr.Zero);

                if (!success)
                {
                    return -1;
                }

                var result = geometry.BytesPerSector;

                return result;
            }
            finally
            {
                if (!driveHandle.IsClosed)
                {
                    driveHandle.Close();
                }
            }
        }

        public static int[] GetPhysicalDiskNumbersFromDrive(string drive)
        {
            if (string.IsNullOrWhiteSpace(drive))
            {
                return new int[0];
            }

            var diskNumbers = new List<uint>();
            var drivePath = $"\\\\.\\{drive.TrimEnd('\\')}";
            var driveHandle = NativeInterop.CreateFile(drivePath, FileAccess.Read, FileShare.ReadWrite, IntPtr.Zero, FileMode.Open, 0, IntPtr.Zero);

            if (driveHandle.IsInvalid)
            {
                return new int[0];
            }

            try
            {
                var diskExtents = new NativeInterop.VOLUME_DISK_EXTENTS();
                var diskExtentsSize = 0U;
                var success = NativeInterop.DeviceIoControl(driveHandle, NativeInterop.IOCTL_VOLUME_GET_VOLUME_DISK_EXTENTS, IntPtr.Zero, 0, ref diskExtents, (uint)Marshal.SizeOf(diskExtents), out diskExtentsSize, IntPtr.Zero);

                if (!success || ArrayUtils.IsNullOrEmpty(diskExtents.Extents))
                {
                    return new int[0];
                }

                for (var i = 0; i < diskExtents.NumberOfDiskExtents; i++)
                {
                    diskNumbers.Add(diskExtents.Extents[i].DiskNumber);
                }

                return diskNumbers.Distinct()
                    .Select(Convert.ToInt32)
                    .OrderBy(_ => _)
                    .ToArray();
            }
            finally
            {
                if (!driveHandle.IsClosed)
                {
                    driveHandle.Close();
                }
            }
        }

        #endregion
    }
}
