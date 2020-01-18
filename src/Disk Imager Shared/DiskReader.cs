using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Workshell.DiskImager
{
    public sealed class DiskReader : IDisposable
    {
        private readonly DiskInfo _diskInfo;
        private readonly Stream _stream;
        private volatile bool _disposed;
        private long _readSize;

        public DiskReader(DiskInfo diskInfo)
        {
            var diskPath = $"\\\\.\\PhysicalDrive{diskInfo.DiskNumber}";
            var diskHandle = NativeInterop.CreateFile(diskPath, FileAccess.Read, FileShare.ReadWrite, IntPtr.Zero, FileMode.Open, 0, IntPtr.Zero);

            if (diskHandle.IsInvalid)
            {
                throw new Exception($"Failed to open disk: {diskPath}");
            }

            _diskInfo = diskInfo;
            _stream = new FileStream(diskHandle, FileAccess.Read, ushort.MaxValue + 1, false);
            _disposed = false;
            _readSize = 0;
        }

        #region Methods

        public void Dispose()
        {
            if (_disposed)
            {
                return;
            }

            Close();

            _disposed = true;
        }

        public void Close()
        {
            _stream.Close();
        }

        public bool ReadSector(byte[] buffer, int offset)
        {
            if (_readSize >= _diskInfo.Size)
            {
                return false;
            }

            if (buffer.Length < _diskInfo.SectorSize)
            {
                return false;
            }

            if ((offset + _diskInfo.SectorSize) > buffer.Length)
            {
                return false;
            }

            var numRead = _stream.Read(buffer, offset, _diskInfo.SectorSize);

            _readSize += numRead;

            return (numRead == _diskInfo.SectorSize);
        }

        #endregion
    }
}
