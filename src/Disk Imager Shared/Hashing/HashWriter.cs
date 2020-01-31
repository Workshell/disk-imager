﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Workshell.DiskImager.Hashing
{
    public abstract class HashWriter
    {
        protected HashWriter(string imageFilename)
        {
            ImageFilename = imageFilename;
        }

        #region Methods

        public virtual void Generate(Action<long, long, int> callback = null)
        {
            Generate(CancellationToken.None, callback);
        }

        public abstract void Generate(CancellationToken cancellationToken, Action<long, long, int> callback = null);

        #endregion

        #region Properties

        public string ImageFilename { get; }
        public abstract string HashFilename { get; }

        #endregion
    }
}
