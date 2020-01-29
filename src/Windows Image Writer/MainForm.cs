using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
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
        private long _totalRead;
        private long _readRate;
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
            _totalRead = 0;
            _readRate = 0;
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

        private async Task<bool> ValidateHashAsync()
        {
            lblStatus.Text = "Validating image hash...";
            progressBar.Value = 0;

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
                Func<long, long, int, bool> validationCallback = (total, totalWrite, numWrite) =>
                {
                    Interlocked.Exchange(ref _total, total);
                    Interlocked.Exchange(ref _totalWrite, totalWrite);
                    Interlocked.Add(ref _writeRate, numWrite);

                    return !_cts.Token.IsCancellationRequested;
                };

                hashValid = validator.Validate(validationCallback);
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
            Interlocked.Exchange(ref _total, Utils.GetFileSize(_imageFilename));
            Interlocked.Exchange(ref _totalRead, 0);
            Interlocked.Exchange(ref _readRate, 0);
            Interlocked.Exchange(ref _totalWrite, 0);
            Interlocked.Exchange(ref _writeRate, 0);

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
                MessageBox.Show($"Exception:\r\n\r\n{ex}", "Disk Image Writer Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

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
