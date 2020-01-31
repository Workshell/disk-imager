using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Win32.SafeHandles;

namespace Workshell.DiskImager
{
    public sealed class DiskReader : IDisposable
    {
        private readonly DiskInfo _diskInfo;
        private readonly Stack<SafeFileHandle> _volumeHandles;

        private readonly SafeFileHandle _deviceHandle;
        private readonly Stream _stream;
        private bool _closed;
        private long _totalRead;

        public DiskReader(DiskInfo diskInfo)
        {
            _diskInfo = diskInfo;
            _volumeHandles = new Stack<SafeFileHandle>(_diskInfo.Drives?.Length ?? 0);

            OpenVolumes();

            _deviceHandle = DiskUtils.GetDeviceHandle(diskInfo.DiskNumber);
            _stream = new FileStream(_deviceHandle, FileAccess.Read, CommonSizes._128K, false);
            _closed = false;
            _totalRead = 0;
        }

        #region Methods

        public void Dispose()
        {
            Close();
        }

        public void Close()
        {
            if (_closed)
            {
                return;
            }

            _stream.Dispose();
            _deviceHandle.Dispose();
            CloseVolumes();

            _closed = true;
        }

        public bool ReadSector(byte[] buffer)
        {
            if (buffer.Length < _diskInfo.SectorSize)
            {
                return false;
            }

            if (_totalRead >= _diskInfo.Size)
            {
                return false;
            }

            if ((_totalRead + buffer.Length) > _diskInfo.Size)
            {
                return false;
            }

            var numRead = _stream.Read(buffer, 0, _diskInfo.SectorSize);

            if (numRead != _diskInfo.SectorSize)
            {
                return false;
            }

            _totalRead += numRead;

            return true;
        }

        private void OpenVolumes()
        {
            if (!Utils.IsNullOrEmpty(_diskInfo.Drives))
            {
                foreach (var drive in _diskInfo.Drives)
                {
                    try
                    {
                        var volumeHandle = DiskUtils.GetVolumeHandle(drive.Name);

                        if (volumeHandle == null)
                        {
                            throw new Exception($"Could not open volume: {drive.Name}");
                        }

                        if (!DiskUtils.LockVolume(volumeHandle))
                        {
                            volumeHandle.Close();

                            throw new Exception($"Could not lock volume: {drive.Name}");
                        }

                        if (!DiskUtils.UnmountVolume(volumeHandle))
                        {
                            DiskUtils.UnlockVolume(volumeHandle);
                            volumeHandle.Close();

                            throw new Exception($"Could not unmount volume: {drive.Name}");
                        }

                        _volumeHandles.Push(volumeHandle);
                    }
                    catch
                    {
                        CloseVolumes();

                        throw;
                    }
                }
            }
        }

        private void CloseVolumes()
        {
            while (_volumeHandles.Count > 0)
            {
                var volumeHandle = _volumeHandles.Pop();

                DiskUtils.UnlockVolume(volumeHandle);
                volumeHandle.Dispose();
            }
        }

        #endregion
    }
}
