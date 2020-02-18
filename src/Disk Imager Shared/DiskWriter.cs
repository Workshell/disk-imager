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

using Microsoft.Win32.SafeHandles;

namespace Workshell.DiskImager
{
    public sealed class DiskWriter : IDisposable
    {
        private readonly DiskInfo _diskInfo;
        private readonly Stack<SafeFileHandle> _volumeHandles;

        private readonly SafeFileHandle _deviceHandle;
        private readonly Stream _stream;
        private bool _disposed;
        private bool _closed;
        private long _totalWrite;

        public DiskWriter(DiskInfo diskInfo)
        {
            _diskInfo = diskInfo;
            _volumeHandles = new Stack<SafeFileHandle>(_diskInfo.Drives?.Length ?? 0);

            OpenVolumes();

            _deviceHandle = DiskUtils.GetDeviceHandle(diskInfo.DiskNumber, false);
            _stream = new FileStream(_deviceHandle, FileAccess.Write, CommonSizes._128K, false);
            _disposed = false;
            _closed = false;
            _totalWrite = 0;
        }

        #region Methods

        public void Dispose()
        {
            if (_disposed)
            {
                return;
            }

            Close();
            GC.SuppressFinalize(this);

            _disposed = true;
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

        public bool WriteSector(byte[] buffer)
        {
            if (buffer.Length < _diskInfo.SectorSize)
            {
                return false;
            }

            if (_totalWrite == _diskInfo.Size)
            {
                return false;
            }

            if ((_totalWrite + buffer.Length) > _diskInfo.Size)
            {
                return false;
            }

            _stream.Write(buffer, 0, buffer.Length);

            _totalWrite += buffer.Length;

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
