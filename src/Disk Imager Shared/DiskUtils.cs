﻿#region License
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
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Win32.SafeHandles;

namespace Workshell.DiskImager
{
    internal static class DiskUtils
    {
        #region Methods

        public static bool GetDiskGeometry(ushort diskNumber, out NativeInterop.DISK_GEOMETRY geometry)
        {
            geometry = new NativeInterop.DISK_GEOMETRY();

            var deviceHandle = GetDeviceHandle(diskNumber);

            if (deviceHandle == null)
            {
                return false;
            }

            using (deviceHandle)
            {
                var geometrySize = 0U;
                
                return NativeInterop.DeviceIoControl(deviceHandle, NativeInterop.IOCTL_DISK_GET_DRIVE_GEOMETRY, IntPtr.Zero, 0, ref geometry, (uint)Marshal.SizeOf(geometry), out geometrySize, IntPtr.Zero);
            }
        }

        public static long GetPhysicalDiskSize(ushort diskNumber)
        {
            NativeInterop.DISK_GEOMETRY geometry;

            if (!GetDiskGeometry(diskNumber, out geometry))
            {
                return -1;
            }

            var result = geometry.BytesPerSector * geometry.SectorsPerTrack * geometry.TracksPerCylinder * geometry.Cylinders;

            return result;
        }

        public static int GetPhysicalDiskSectorSize(ushort diskNumber)
        {
            NativeInterop.DISK_GEOMETRY geometry;

            if (!GetDiskGeometry(diskNumber, out geometry))
            {
                return -1;
            }

            var result = geometry.BytesPerSector;

            return result;
        }

        public static long GetPhysicalDiskSectorCount(ushort diskNumber)
        {
            NativeInterop.DISK_GEOMETRY geometry;

            if (!GetDiskGeometry(diskNumber, out geometry))
            {
                return -1;
            }

            var result = geometry.SectorsPerTrack * geometry.TracksPerCylinder * geometry.Cylinders;

            return result;
        }

        public static SafeFileHandle GetVolumeHandle(string drive, bool readOnly = true)
        {
            var path = $"\\\\.\\{drive.TrimEnd('\\')}".ToUpperInvariant();
            var handle = NativeInterop.CreateFile(path, (readOnly ? FileAccess.Read : FileAccess.ReadWrite), FileShare.ReadWrite, IntPtr.Zero, FileMode.Open, 0, IntPtr.Zero);

            if (handle.IsInvalid)
            {
                handle.Dispose();

                return null;
            }

            return handle;
        }

        public static bool LockVolume(SafeFileHandle volumeHandle)
        {
            uint bytesReturned;
            var result = NativeInterop.DeviceIoControl(volumeHandle, NativeInterop.FSCTL_LOCK_VOLUME, IntPtr.Zero, 0, IntPtr.Zero, 0, out bytesReturned, IntPtr.Zero);

            return result;
        }

        public static bool UnlockVolume(SafeFileHandle volumeHandle)
        {
            uint bytesReturned;

            return NativeInterop.DeviceIoControl(volumeHandle, NativeInterop.FSCTL_UNLOCK_VOLUME, IntPtr.Zero, 0, IntPtr.Zero, 0, out bytesReturned, IntPtr.Zero);
        }

        public static bool UnmountVolume(SafeFileHandle volumeHandle)
        {
            uint bytesReturned;
            var result = NativeInterop.DeviceIoControl(volumeHandle, NativeInterop.FSCTL_DISMOUNT_VOLUME, IntPtr.Zero, 0, IntPtr.Zero, 0, out bytesReturned, IntPtr.Zero);

            return result;
        }

        public static ushort[] GetPhysicalDiskNumbersFromDrive(string drive)
        {
            if (string.IsNullOrWhiteSpace(drive))
            {
                return new ushort[0];
            }

            var diskNumbers = new List<uint>();
            var volumeHandle = GetVolumeHandle(drive);

            if (volumeHandle == null)
            {
                return new ushort[0];
            }

            using (volumeHandle)
            {
                var diskExtents = new NativeInterop.VOLUME_DISK_EXTENTS();
                var diskExtentsSize = 0U;
                var success = NativeInterop.DeviceIoControl(volumeHandle, NativeInterop.IOCTL_VOLUME_GET_VOLUME_DISK_EXTENTS, IntPtr.Zero, 0, ref diskExtents, (uint)Marshal.SizeOf(diskExtents), out diskExtentsSize, IntPtr.Zero);

                if (!success || Utils.IsNullOrEmpty(diskExtents.Extents))
                {
                    return new ushort[0];
                }

                for (var i = 0; i < diskExtents.NumberOfDiskExtents; i++)
                {
                    diskNumbers.Add(diskExtents.Extents[i].DiskNumber);
                }

                return diskNumbers.Distinct()
                    .Select(Convert.ToUInt16)
                    .OrderBy(_ => _)
                    .ToArray();
            }
        }

        public static SafeFileHandle GetDeviceHandle(ushort diskNumber, bool readOnly = true)
        {
            var path = $"\\\\.\\PhysicalDrive{diskNumber}";
            var handle = NativeInterop.CreateFile(path, (readOnly ? FileAccess.Read : FileAccess.ReadWrite), FileShare.ReadWrite, IntPtr.Zero, FileMode.Open, 0, IntPtr.Zero);

            if (handle.IsInvalid)
            {
                handle.Dispose();

                return null;
            }

            return handle;
        }

        #endregion
    }
}
