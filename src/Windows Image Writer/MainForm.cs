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
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

using Workshell.DiskImager.Extensions;
using Workshell.DiskImager.Hashing;

namespace Workshell.DiskImager
{
    public partial class MainForm : Form
    {
        private const string ErrorTitle = "Disk Image Writer Error";

        private DriveDetector _driveDetector;
        private DiskInfo[] _disks;
        private CancellationTokenSource _cts;
        private DiskInfo _selectedDisk;
        private string _imageFilename;
        private string _hashFilename;
        private long _started;
        private long _total;
        private long _totalWrite;
        private long _writeRate;

        public MainForm()
        {
            InitializeComponent();

            _driveDetector = new DriveDetector();
            _disks = DiskInfo.GetDisks();
            _cts = null;
            _selectedDisk = null;
            _imageFilename = string.Empty;
            _hashFilename = string.Empty;
            _started = 0;
            _total = 0;
            _totalWrite = 0;
            _writeRate = 0;

            Disposed += OnDisposed;
            _driveDetector.DeviceArrived += OnDeviceAddRemove;
            _driveDetector.DeviceRemoved += OnDeviceAddRemove;
        }

        private void OnDisposed(object sender, EventArgs e)
        {
            _driveDetector.Dispose();
        }

        private void OnDeviceAddRemove(object sender, DriveDetectorEventArgs e)
        {
            _disks = DiskInfo.GetDisks();

            ddlDevice.Items.Clear();

            foreach (var disk in _disks)
            {
                ddlDevice.Items.Add(disk);

                if (_selectedDisk != null && _selectedDisk.DiskNumber == disk.DiskNumber)
                {
                    ddlDevice.SelectedItem = disk;
                }
            }
        }

        private Stream GetDecompressionStream(string fileName, FileStream fileStream)
        {
            Stream result;

            if (fileName.EndsWith(".gz", StringComparison.OrdinalIgnoreCase))
            {
                result = new GZipStream(fileStream, CompressionMode.Decompress, false);
            }
            else
            {
                result = fileStream;
            }

            return result;
        }

        private async Task<bool> ValidateHashAsync()
        {
            lblStatus.Text = "Validating image hash...";
            progressBar.Value = 0;

            Interlocked.Exchange(ref _total, Utils.GetFileSize(_imageFilename));
            Interlocked.Exchange(ref _totalWrite, 0);
            Interlocked.Exchange(ref _writeRate, 0);

            // Get hash for file from hash sum file
            var fileName = Path.GetFileName(_imageFilename);
            var hashExt = Path.GetExtension(_hashFilename);
            var reader = new HashSumReader(_hashFilename);
            var fileHashes = reader.Read();
            string fileHash;

            if (!fileHashes.TryGetValue(fileName, out fileHash) || string.IsNullOrWhiteSpace(fileHash))
            {
                MessageBox.Show($"Could not find hash for file: {fileName}", ErrorTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);

                return false;
            }

            var hashValid = true;

            await Task.Run(() =>
            {
                // Get hash validator
                HashValidator validator;

                if (string.Compare(".md5", hashExt, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    validator = new MD5HashValidator(_imageFilename, fileHash);
                }
                else if (string.Compare(".sha1", hashExt, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    validator = new SHA1HashValidator(_imageFilename, fileHash);
                }
                else if (string.Compare(".sha256", hashExt, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    validator = new SHA256HashValidator(_imageFilename, fileHash);
                }
                else if (string.Compare(".sha512", hashExt, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    validator = new SHA512HashValidator(_imageFilename, fileHash);
                }
                else
                {
                    throw new Exception($"Cannot find hash validator for file extension: {hashExt}");
                }

                // Perform validation
                Action<long, long, int> validationCallback = (total, totalWrite, numWrite) =>
                {
                    Interlocked.Exchange(ref _totalWrite, totalWrite);
                    Interlocked.Add(ref _writeRate, numWrite);
                };

                hashValid = validator.Validate(_cts.Token, validationCallback);
            });

            if (_cts.Token.IsCancellationRequested)
            {
                return false;
            }

            if (!hashValid)
            {
                MessageBox.Show("The computed hash does not match the hash from the file.", ErrorTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);

                return false;
            }

            return true;
        }

        private async Task<bool> WriteImageFileAsync()
        {
            lblStatus.Text = "Writing image...";
            progressBar.Value = 0;

            Interlocked.Exchange(ref _total, _selectedDisk.Size);
            Interlocked.Exchange(ref _totalWrite, 0);
            Interlocked.Exchange(ref _writeRate, 0);

            await Task.Run(() =>
            {
                var file = new FileStream(_imageFilename, FileMode.Open, FileAccess.Read, FileShare.Read, CommonSizes._64K, FileOptions.SequentialScan);

                using (var reader = GetDecompressionStream(_imageFilename, file))
                {
                    using (var writer = new DiskWriter(_selectedDisk))
                    {
                        var buffer = new byte[_selectedDisk.SectorSize];
                        var numRead = reader.Read(buffer, 0, buffer.Length);

                        while (!_cts.Token.IsCancellationRequested && numRead > 0)
                        {
                            Interlocked.Add(ref _totalWrite, numRead);
                            Interlocked.Add(ref _writeRate, numRead);

                            if (!writer.WriteSector(buffer))
                            {
                                break;
                            }

                            numRead = reader.Read(buffer, 0, buffer.Length);
                        }
                    }
                }
            });

            if (Interlocked.Read(ref _totalWrite) > Interlocked.Read(ref _total))
            {
                MessageBox.Show("Cannot continue, it would appear we have reached the end of the disk but there's still more image to write.", "Disk Image Writer Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

                return false;
            }

            if (_cts.Token.IsCancellationRequested)
            {
                return false;
            }

            return true;
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            MinimumSize = Size;

            openDlg.Filter = Utils.BuildDialogFilters(Resources.OpenDialogFilters);
            hashDlg.Filter = Utils.BuildDialogFilters(Resources.HashDialogFilters);

            foreach (var disk in _disks)
            {
                ddlDevice.Items.Add(disk);
            }

            lblTimeTakenValue.Text = string.Empty;
            lblPercentCompleteValue.Text = string.Empty;
            lblSpeedValue.Text = string.Empty;
            lblStatus.Text = string.Empty;
            progressBar.Value = 0;
            gbxProgress.Enabled = false;

            timer.Start();
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing && _cts != null)
            {
                if (MessageBox.Show("Operation is in progress, do you wish to cancel it?", "Confirm Exit", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                {
                    e.Cancel = true;

                    return;
                }

                _cts.Cancel();
            }
        }

        private async void btnWrite_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("This operation will overwrite any existing data on the selected physical disk. Are you sure you want to continue?", "Confirm Write", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
            {
                return;
            }

            timer.Stop();
            processTimer.Start();

            gbxImage.Enabled = false;
            gbxDisk.Enabled = false;
            gbxHash.Enabled = false;
            gbxProgress.Enabled = true;

            btnWrite.Enabled = false;
            btnCancel.Enabled = true;

            _cts = new CancellationTokenSource();
            _selectedDisk = (DiskInfo)ddlDevice.SelectedItem;
            _imageFilename = txtFilename.Text;
            _hashFilename = txtHash.Text;

            Interlocked.Exchange(ref _started, DateTime.UtcNow.Ticks);

            try
            {
                if (!string.IsNullOrWhiteSpace(txtHash.Text) && File.Exists(txtHash.Text))
                {
                    if (!await ValidateHashAsync())
                    {
                        return;
                    }

                    await Task.Delay(1000); // Lets the progress get to 100%
                }

                if (!await WriteImageFileAsync())
                {
                    return;
                }

                await Task.Delay(1000); // Lets the progress get to 100%
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Disk Image Writer Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

                #if DEBUG
                throw; // When debugging re-throw for the debugger
                #endif
            }
            finally
            {
                _cts.Dispose(() => _cts = null);

                gbxImage.Enabled = true;
                gbxDisk.Enabled = true;
                gbxHash.Enabled = true;
                gbxProgress.Enabled = false;

                lblTimeTakenValue.Text = string.Empty;
                lblPercentCompleteValue.Text = string.Empty;
                lblSpeedValue.Text = string.Empty;
                lblStatus.Text = string.Empty;
                progressBar.Value = 0;

                processTimer.Stop();
                timer.Start();
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            _cts?.Cancel();
        }

        private void btnAbout_Click(object sender, EventArgs e)
        {
            Utils.ShowAboutDialog(Text, null, Icon.ToBitmap());
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void btnBrowseImage_Click(object sender, EventArgs e)
        {
            if (openDlg.ShowDialog() == DialogResult.OK)
            {
                txtFilename.Text = openDlg.FileName;
            }
        }

        private void btnBrowseHash_Click(object sender, EventArgs e)
        {
            if (hashDlg.ShowDialog() == DialogResult.OK)
            {
                txtHash.Text = hashDlg.FileName;
            }
        }

        private void btnClearHash_Click(object sender, EventArgs e)
        {
            txtHash.Text = string.Empty;
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            btnClearHash.Enabled = !string.IsNullOrWhiteSpace(txtHash.Text);
            btnCancel.Enabled = false;

            if (ddlDevice.SelectedIndex < 0 || string.IsNullOrWhiteSpace(txtFilename.Text) || !File.Exists(txtFilename.Text) || !string.IsNullOrWhiteSpace(txtHash.Text) && !File.Exists(txtHash.Text))
            {
                btnWrite.Enabled = false;
            }
            else
            {
                btnWrite.Enabled = true;
            }
        }

        private void processTimer_Tick(object sender, EventArgs e)
        {
            var started = Interlocked.Read(ref _started);
            var timeTaken = DateTime.UtcNow - (new DateTime(started, DateTimeKind.Utc));
            var total = Interlocked.Read(ref _total);
            var totalWrite = Interlocked.Read(ref _totalWrite);
            var percentComplete = Math.Min(100, (int)Math.Round((double)(100 * totalWrite) / total));
            var writeRate = Interlocked.Exchange(ref _writeRate, 0);

            progressBar.Value = percentComplete;
            lblTimeTakenValue.Text = timeTaken.ToString(timeTaken.Days == 0 ? @"hh\:mm\:ss" : @"d\.hh\:mm\:ss");
            lblPercentCompleteValue.Text = $"{percentComplete}%";
            lblSpeedValue.Text = $"{Utils.FormatBytes(writeRate)}/sec";
        }
    }
}
