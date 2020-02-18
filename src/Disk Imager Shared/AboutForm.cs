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
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Workshell.DiskImager
{
    internal sealed partial class AboutForm : Form
    {
        private const string WebsiteAddress = "https://www.workshell.co.uk/";
        private const string EmailAddress = "support@workshell.co.uk";

        public AboutForm()
        {
            InitializeComponent();
        }

        public AboutForm(string title, string version, Image icon) : this()
        {
            var copyright = lblCopyright.Text;
            var year_start = 2020;
            var year_now = DateTime.Now.Year;

            if (year_start == year_now)
            {
                copyright = string.Format(copyright, year_now);
            }
            else
            {
                copyright = string.Format(copyright, year_start + " - " + year_now);
            }

            Text = $"About {title}";
            pictureBox.Image = icon;
            lblTitle.Text = title;
            lblVersion.Text = $"Version {version}";
            lblCopyright.Text = copyright;
            lnkProduct.Text = WebsiteAddress;
            lnkMail.Text = EmailAddress;
            txtLicense.Text = Resources.License;
        }

        private async void AboutForm_Shown(object sender, EventArgs e)
        {
            await Task.Delay(250);
            btnOK.Focus();
        }

        private void lnkProduct_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start(WebsiteAddress);
        }

        private void lnkMail_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start($"mailto:{EmailAddress}");
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
