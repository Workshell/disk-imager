using System;
using System.Collections.Generic;
using System.ComponentModel;
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
    public sealed partial class MainForm : Form
    {
        private DriveDetector _driveDetector;
        private DiskInfo[] _disks;
        private CancellationTokenSource _cts;
        private DiskInfo _selectedDisk;
        private string _imageFilename;
        private string _hashFilename;
        private string _hash;
        private string _compression;
        private long _started;
        private long _total;
        private long _totalRead;
        private long _readRate;

        public MainForm()
        {
            InitializeComponent();

            _driveDetector = new DriveDetector();
            _disks = DiskInfo.GetDisks();
            _cts = null;
            _selectedDisk = null;
            _imageFilename = string.Empty;
            _hashFilename = string.Empty;
            _hash = string.Empty;
            _compression = string.Empty;
            _started = 0;
            _total = 0;
            _totalRead = 0;
            _readRate = 0;

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

        private Stream GetCompressionStream(FileStream fileStream)
        {
            Stream result;

            if (string.Compare("none", _compression, StringComparison.OrdinalIgnoreCase) == 0)
            {
                result = fileStream;
            }
            else if (string.Compare("gzip", _compression, StringComparison.OrdinalIgnoreCase) == 0)
            {
                result = new GZipStream(fileStream, CompressionLevel.Optimal, true);
            }
            else
            {
                result = fileStream;
            }

            return result;
        }

        private async Task CreateImageFileAsync()
        {
            lblStatus.Text = "Creating disk image...";
            progressBar.Value = 0;

            await Task.Run(() =>
            {
                using (var file = new FileStream(_imageFilename, FileMode.Create, FileAccess.Write, FileShare.None))
                {
                    using (var writer = GetCompressionStream(file))
                    {
                        using (var reader = new DiskReader(_selectedDisk))
                        {
                            var buffer = new byte[_selectedDisk.SectorSize];

                            for (var i = 0; i < _selectedDisk.SectorCount; i++)
                            {
                                if (_cts.Token.IsCancellationRequested)
                                {
                                    break;
                                }

                                if (!reader.ReadSector(buffer, 0))
                                {
                                    break;
                                }

                                Interlocked.Add(ref _totalRead, buffer.Length);
                                Interlocked.Add(ref _readRate, buffer.Length);

                                writer.Write(buffer, 0, buffer.Length);
                            }
                        }

                        writer.Flush();
                    }
                }
            });

            if (_cts.Token.IsCancellationRequested)
            {
                if (File.Exists(_imageFilename))
                {
                    File.Delete(_imageFilename);
                }

                return;
            }

            await Task.Delay(1000); // Lets the progress get to 100%
        }

        private async Task CreateHashFileAsync()
        {
            if (!_cts.IsCancellationRequested && !string.IsNullOrWhiteSpace(_hash) && string.Compare("none", _hash, StringComparison.OrdinalIgnoreCase) != 0)
            {
                lblStatus.Text = "Creating hash file...";
                progressBar.Value = 0;

                await Task.Run(() =>
                {
                    HashWriter hashWriter = null;

                    if (string.Compare("md5", _hash, StringComparison.OrdinalIgnoreCase) == 0)
                    {
                        hashWriter = new MD5HashWriter(_imageFilename);
                    }
                    else if (string.Compare("sha1", _hash, StringComparison.OrdinalIgnoreCase) == 0)
                    {
                        hashWriter = new SHA1HashWriter(_imageFilename);
                    }
                    else if (string.Compare("sha256", _hash, StringComparison.OrdinalIgnoreCase) == 0)
                    {
                        hashWriter = new SHA256HashWriter(_imageFilename);
                    }
                    else if (string.Compare("sha512", _hash, StringComparison.OrdinalIgnoreCase) == 0)
                    {
                        hashWriter = new SHA512HashWriter(_imageFilename);
                    }

                    if (hashWriter != null)
                    {
                        Func<long, long, int, bool> generateCallback = (total, totalRead, numRead) =>
                        {
                            Interlocked.Exchange(ref _total, total);
                            Interlocked.Exchange(ref _totalRead, totalRead);
                            Interlocked.Add(ref _readRate, numRead);

                            return !_cts.Token.IsCancellationRequested;
                        };
                        _hashFilename = hashWriter.HashFilename;

                        hashWriter.Generate(generateCallback);
                    }
                });

                if (_cts.Token.IsCancellationRequested)
                {
                    if (File.Exists(_imageFilename))
                    {
                        File.Delete(_imageFilename);
                    }

                    if (!string.IsNullOrWhiteSpace(_hashFilename) && File.Exists(_hashFilename))
                    {
                        File.Delete(_hashFilename);
                    }

                    return;
                }

                await Task.Delay(1000); // Lets the progress get to 100%
            }
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            MinimumSize = Size;

            saveDlg.Filter = Utils.BuildDialogFilters(Resources.OpenDialogFilters);

            foreach (var disk in _disks)
            {
                ddlDevice.Items.Add(disk);
            }

            ddlHash.SelectedIndex = 0;
            ddlCompression.SelectedIndex = 0;

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

        private async void btnCreate_Click(object sender, EventArgs e)
        {
            timer.Stop();
            processTimer.Start();

            gbxDisk.Enabled = false;
            gbxImage.Enabled = false;
            gbxOptions.Enabled = false;
            gbxProgress.Enabled = true;

            btnCreate.Enabled = false;
            btnCancel.Enabled = true;

            _cts = new CancellationTokenSource();
            _selectedDisk = (DiskInfo)ddlDevice.SelectedItem;
            _imageFilename = txtFilename.Text;
            _hashFilename = string.Empty;
            _hash = ddlHash.SelectedItem.ToString();
            _compression = ddlCompression.SelectedItem.ToString();

            Interlocked.Exchange(ref _started, DateTime.UtcNow.Ticks);
            Interlocked.Exchange(ref _total, _selectedDisk.Size);
            Interlocked.Exchange(ref _totalRead, 0);
            Interlocked.Exchange(ref _readRate, 0);

            try
            {
                await CreateImageFileAsync();
                await CreateHashFileAsync();
            }
            catch (Exception ex)
            {
                if (File.Exists(_imageFilename))
                {
                    File.Delete(_imageFilename);
                }

                if (!string.IsNullOrWhiteSpace(_hashFilename) && File.Exists(_hashFilename))
                {
                    File.Delete(_hashFilename);
                }

                MessageBox.Show($"Exception:\r\n\r\n{ex}", "Disk Image Reader Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

                #if DEBUG
                throw; // When debugging re-throw for the debugger
                #endif
            }
            finally
            {
                _cts.Dispose(() => _cts = null);

                gbxDisk.Enabled = true;
                gbxImage.Enabled = true;
                gbxOptions.Enabled = true;
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
            saveDlg.FileName = txtFilename.Text;

            if (saveDlg.ShowDialog() != DialogResult.OK)
            {
                return;
            }

            txtFilename.Text = saveDlg.FileName;

            var compression = ddlCompression.SelectedItem.ToString();

            if (string.Compare("gzip", compression, StringComparison.OrdinalIgnoreCase) == 0)
            {
                var ext = Path.GetExtension(txtFilename.Text);

                if (string.Compare(".gz", ext, StringComparison.OrdinalIgnoreCase) != 0)
                {
                    txtFilename.Text = $"{txtFilename.Text}.gz";
                }
            }
        }

        private void ddlCompression_SelectedIndexChanged(object sender, EventArgs e)
        {
            var fileName = txtFilename.Text;
            var compression = ddlCompression.SelectedItem.ToString();

            if (string.Compare("gzip", compression, StringComparison.OrdinalIgnoreCase) == 0 && !fileName.EndsWith(".gz"))
            {
                txtFilename.Text = $"{fileName}.gz";
            }
            else
            {
                if (fileName.EndsWith(".gz"))
                {
                    txtFilename.Text = Path.ChangeExtension(fileName, null);
                }
            }
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            if (ddlDevice.SelectedIndex < 0 || string.IsNullOrWhiteSpace(txtFilename.Text))
            {
                btnCreate.Enabled = false;
                btnCancel.Enabled = false;
            }
            else
            {
                btnCreate.Enabled = true;
                btnCancel.Enabled = false;
            }
        }

        private void processTimer_Tick(object sender, EventArgs e)
        {
            var started = Interlocked.Read(ref _started);
            var timeTaken = DateTime.UtcNow - (new DateTime(started, DateTimeKind.Utc));
            var total = Interlocked.Read(ref _total);
            var totalRead = Interlocked.Read(ref _totalRead);
            var percentComplete = Math.Min(100, (int)Math.Round((double)(100 * totalRead) / total));
            var readRate = Interlocked.Exchange(ref _readRate, 0);

            progressBar.Value = percentComplete;
            lblTimeTakenValue.Text = timeTaken.ToString(timeTaken.Days == 0 ? @"hh\:mm\:ss" : @"d\.hh\:mm\:ss");
            lblPercentCompleteValue.Text = $"{percentComplete}%";
            lblSpeedValue.Text = $"{Utils.FormatBytes(readRate)}/sec";
        }
    }
}
