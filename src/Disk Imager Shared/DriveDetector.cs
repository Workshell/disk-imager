using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace Workshell.DiskImager
{
    public delegate void DriveDetectorEventHandler(object sender, DriveDetectorEventArgs e);

    public sealed class DriveDetectorEventArgs : EventArgs
    {
        public DriveDetectorEventArgs(string drive)
        {
            Drive = drive;
        }

        public string Drive { get; }
    }

    public sealed class DriveDetector : IDisposable
    {
        private sealed class DetectorForm : Form
        {
            private readonly DriveDetector _detector = null;

            public DetectorForm(DriveDetector detector)
            {
                _detector = detector;

                MinimizeBox = false;
                MaximizeBox = false;
                ShowInTaskbar = false;
                ShowIcon = false;
                FormBorderStyle = FormBorderStyle.None;
                Size = new Size(1, 1);
                MinimumSize = Size;
                Location = new Point(-ushort.MaxValue, -ushort.MaxValue);

                Load += (sender, args) => { Visible = false; };
                Shown += (sender, args) => { Visible = false; };
                Activated += (sender, args) => { Visible = false; };
            }

            protected override void WndProc(ref Message m)
            {
                base.WndProc(ref m);

                _detector?.WndProc(ref m);
            }
        }

        private DetectorForm _form;

        public DriveDetector()
        {
            _form = new DetectorForm(this);

            _form.Show();
        }

        #region Methods

        public void Dispose()
        {
            if (_form == null)
            {
                return;
            }

            _form.Dispose();

            _form = null;
        }

        internal void WndProc(ref Message m)
        {
            if (m.Msg == NativeInterop.WM_DEVICECHANGE)
            {
                var wParam = m.WParam.ToInt32();

                switch (wParam)
                {
                    case NativeInterop.DBT_DEVICEARRIVAL:
                    case NativeInterop.DBT_DEVICEREMOVECOMPLETE:
                    {
                        var devType = Marshal.ReadInt32(m.LParam, 4);

                        if (devType == NativeInterop.DBT_DEVTYP_VOLUME)
                        {
                            var vol = (NativeInterop.DEV_BROADCAST_VOLUME)Marshal.PtrToStructure(m.LParam, typeof(NativeInterop.DEV_BROADCAST_VOLUME));
                            var driveLetter = DriveMaskToLetter(vol.dbcv_unitmask);
                            DriveDetectorEventHandler handler = null;

                            if (wParam == NativeInterop.DBT_DEVICEARRIVAL)
                            {
                                handler = DeviceArrived;
                            }
                            else
                            {
                                handler = DeviceRemoved;
                            }

                            if (handler != null)
                            {
                                var args = new DriveDetectorEventArgs($"{driveLetter}:\\");

                                handler(this, args);
                            }
                        }

                        break;
                    }
                }
            }
        }

        private static char DriveMaskToLetter(int mask)
        {
            char letter;
            const string drives = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            var cnt = 0;
            var pom = mask / 2;

            while (pom != 0)
            {
                pom = pom / 2;
                cnt++;
            }

            if (cnt < drives.Length)
            {
                letter = drives[cnt];
            }
            else
            {
                letter = '?';
            }

            return letter;
        }

        #endregion

        #region Events

        public event DriveDetectorEventHandler DeviceArrived;
        public event DriveDetectorEventHandler DeviceRemoved;

        #endregion
    }
}
