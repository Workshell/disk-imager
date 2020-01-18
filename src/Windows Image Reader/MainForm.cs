using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using ICSharpCode.SharpZipLib.BZip2;
using ICSharpCode.SharpZipLib.GZip;
using Workshell.DiskImager.Hashing;

namespace Workshell.DiskImager
{
    public sealed partial class MainForm : Form
    {
        private readonly object _locker;
        private readonly DiskInfo[] _disks;

        private DiskInfo _selectedDisk;
        private string _imageFilename;
        private string _hash;
        private string _compression;
        private long _started;
        private long _total;
        private long _totalRead;
        private long _readRate;

        public MainForm()
        {
            InitializeComponent();

            _locker = new object();
            _disks = DiskInfo.GetDisks();

            _selectedDisk = null;
            _imageFilename = string.Empty;
            _hash = string.Empty;
            _compression = string.Empty;
            _started = 0;
            _total = 0;
            _totalRead = 0;
            _readRate = 0;
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
                result = new GZipOutputStream(fileStream, 4096);

                ((GZipOutputStream)result).SetLevel(9);
            }
            else if (string.Compare("bzip2", _compression, StringComparison.OrdinalIgnoreCase) == 0)
            {
                result = new BZip2OutputStream(fileStream, 9);
            }
            else
            {
                result = fileStream;
            }

            return result;
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            MinimumSize = Size;

            saveDlg.Filter = "Disk Images (*.img)|*.img|All Files (*.*)|*.*";

            foreach (var disk in _disks)
            {
                ddlDevice.Items.Add(disk);
            }

            ddlHash.SelectedIndex = 0;
            ddlCompression.SelectedIndex = 0;

            lblTimeTakenValue.Text = string.Empty;
            lblPercentCompleteValue.Text = string.Empty;
            lblReadSpeedValue.Text = string.Empty;
            lblStatus.Text = string.Empty;
            progressBar.Value = 0;
            gbxProgress.Enabled = false;

            timer.Start();
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing && backgroundWorker.IsBusy)
            {
                backgroundWorker.CancelAsync();

                while (backgroundWorker.IsBusy)
                {
                    Application.DoEvents();
                }
            }
        }

        private void btnCreate_Click(object sender, EventArgs e)
        {
            if (!backgroundWorker.IsBusy && !backgroundWorker.CancellationPending)
            {
                _selectedDisk = (DiskInfo)ddlDevice.SelectedItem;
                _imageFilename = txtFilename.Text;
                _hash = ddlHash.SelectedItem.ToString();
                _compression = ddlCompression.SelectedItem.ToString();

                backgroundWorker.RunWorkerAsync();
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            if (backgroundWorker.IsBusy && !backgroundWorker.CancellationPending)
            {
                backgroundWorker.CancelAsync();
            }
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            Close();
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
            var compression = ddlCompression.SelectedItem.ToString();
            var ext = Path.GetExtension(txtFilename.Text);

            if (string.Compare("gzip", compression, StringComparison.OrdinalIgnoreCase) == 0)
            {
                if (string.Compare(".gz", ext, StringComparison.OrdinalIgnoreCase) != 0)
                {
                    txtFilename.Text = $"{txtFilename.Text}.gz";
                }
            }
            if (string.Compare("bzip2", compression, StringComparison.OrdinalIgnoreCase) == 0)
            {
                if (string.Compare(".bz2", ext, StringComparison.OrdinalIgnoreCase) != 0)
                {
                    txtFilename.Text = $"{txtFilename.Text}.bz2";
                }
            }
            else
            {
                if (string.Compare(".gz", ext, StringComparison.OrdinalIgnoreCase) == 0 || string.Compare(".bz2", ext, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    txtFilename.Text = Path.ChangeExtension(txtFilename.Text, null);
                }
            }
        }

        private void backgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            backgroundWorker.ReportProgress(1);

            try
            {
                lock (_locker)
                {
                    _started = DateTime.UtcNow.Ticks;
                    _total = _selectedDisk.Size;
                    _totalRead = 0;
                    _readRate = 0;
                }

                /*
                Interlocked.Exchange(ref _started, DateTime.UtcNow.Ticks);
                Interlocked.Exchange(ref _total, _selectedDisk.Size);
                Interlocked.Exchange(ref _totalRead, 0);
                Interlocked.Exchange(ref _readRate, 0);
                */

                using (var file = new FileStream(_imageFilename, FileMode.Create, FileAccess.Write, FileShare.None))
                {
                    using (var writer = GetCompressionStream(file))
                    {
                        using (var reader = new DiskReader(_selectedDisk))
                        {
                            var buffer = new byte[_selectedDisk.SectorSize];

                            while (true)
                            {
                                if (backgroundWorker.CancellationPending)
                                {
                                    break;
                                }

                                if (!reader.ReadSector(buffer, 0))
                                {
                                    break;
                                }

                                writer.Write(buffer, 0, buffer.Length);

                                lock (_locker)
                                {
                                    _totalRead += buffer.Length;
                                    _readRate += buffer.Length;
                                }

                                /*
                                Interlocked.Add(ref _totalRead, buffer.Length);
                                Interlocked.Add(ref _readRate, buffer.Length);
                                */
                            }
                        }

                        writer.Flush();
                    }

                    //file.Flush();

                    if (!backgroundWorker.CancellationPending)
                    {
                        Thread.Sleep(1000); // Lets the progress get to 100%
                    }
                }

                if (backgroundWorker.CancellationPending && File.Exists(_imageFilename))
                {
                    File.Delete(_imageFilename);
                }

                if (!backgroundWorker.CancellationPending && !string.IsNullOrWhiteSpace(_hash))
                {
                    backgroundWorker.ReportProgress(2);

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

                    Func<long, long, int, bool> generateCallback = (total, totalRead, numRead) =>
                    {
                        lock (_locker)
                        {
                            _total = total;
                            _totalRead = totalRead;
                            _readRate += numRead;
                        }

                        /*
                        Interlocked.Exchange(ref _total, total);
                        Interlocked.Exchange(ref _totalRead, totalRead);
                        Interlocked.Add(ref _readRate, numRead);
                        */

                        return !backgroundWorker.CancellationPending;
                    };

                    hashWriter?.Generate(generateCallback);

                    if (!backgroundWorker.CancellationPending)
                    {
                        Thread.Sleep(1000); // Lets the progress get to 100%
                    }
                }
            }
            catch (Exception ex)
            {
                backgroundWorker.ReportProgress(99, ex);
            }
            finally
            {
                backgroundWorker.ReportProgress(100);
            }
        }

        private void backgroundWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            switch (e.ProgressPercentage)
            {
                case 1: // Start of processing
                {
                    timer.Stop();
                    processTimer.Start();

                    gbxDisk.Enabled = false;
                    gbxImage.Enabled = false;
                    gbxOptions.Enabled = false;
                    gbxProgress.Enabled = true;

                    btnCreate.Enabled = false;
                    btnCancel.Enabled = true;

                    lblStatus.Text = "Creating disk image...";
                    progressBar.Value = 0;

                    break;
                }
                case 2:
                {
                    lblStatus.Text = "Creating hash file...";
                    progressBar.Value = 0;

                    break;
                }
                case 99: // Exception
                {
                    var ex = (Exception)e.UserState;

                    MessageBox.Show($"Exception:\r\n\r\n{ex}", "Disk Image Reader Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

                    break;
                }
                case 100: // End of processing
                {
                    gbxDisk.Enabled = true;
                    gbxImage.Enabled = true;
                    gbxOptions.Enabled = true;

                    lblTimeTakenValue.Text = string.Empty;
                    lblPercentCompleteValue.Text = string.Empty;
                    lblReadSpeedValue.Text = string.Empty;
                    lblStatus.Text = string.Empty;
                    progressBar.Value = 0;
                    gbxProgress.Enabled = false;

                    processTimer.Stop();
                    timer.Start();

                    break;
                }
            }
        }

        private void processTimer_Tick(object sender, EventArgs e)
        {
            var started = Interlocked.Read(ref _started);
            var timeTaken = DateTime.UtcNow - (new DateTime(started, DateTimeKind.Utc));
            var total = Interlocked.Read(ref _total);
            var totalRead = Interlocked.Read(ref _totalRead);
            var percentComplete = (int) Math.Round((double) (100 * totalRead) / total);
            var readRate = Interlocked.Exchange(ref _readRate, 0);

            progressBar.Value = percentComplete;
            lblTimeTakenValue.Text = timeTaken.ToString(timeTaken.Days == 0 ? @"hh\:mm\:ss" : @"d\.hh\:mm\:ss");
            lblPercentCompleteValue.Text = $"{percentComplete}%";
            lblReadSpeedValue.Text = $"{Utils.FormatBytes(readRate)}/sec";
        }
    }
}
