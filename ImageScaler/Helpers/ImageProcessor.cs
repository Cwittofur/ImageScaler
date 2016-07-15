using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Security.RightsManagement;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using ImageScaler.Interfaces;
using System.Drawing.Imaging;
using System.Windows;

namespace ImageScaler.Helpers
{
    public class ImageProcessor : IProgressOperation, IDisposable
    {

        private int _total;
        private int _current;
        private bool _isCancelationPending;
        private int _shrinkPercent;
        private string[] _imageList;
        private string _folderPath;

        public ImageProcessor(int shrinkPercent, string[] imageList, string folderPath)
        {            
            _current = 0;
            _isCancelationPending = false;
            _shrinkPercent = shrinkPercent;
            _folderPath = folderPath;
            _imageList = imageList;
            
        }

        void worker_DoWork(object sender, DoWorkEventArgs e)
        {
            if (_shrinkPercent <= 0)
            {
                _shrinkPercent = 25;
            }
            int total = 0;
            foreach (var file in _imageList)
            {
                total++;
            }
            this.Total = total;

            foreach (var file in _imageList)
            {
                var resizedImage = ReduceImageByPercentage(file);
                if (!System.IO.File.Exists($"{_folderPath}\\SmallerImages\\{System.IO.Path.GetFileName(file)}"))
                {
                    SaveImage(resizedImage, System.IO.Path.GetFileName(file));
                }
                else
                {
                    SaveImage(resizedImage, string.Format("{0}_{1}{2}", System.IO.Path.GetFileNameWithoutExtension(file), Guid.NewGuid(), System.IO.Path.GetExtension(file)));
                }
                this.Current++;
                resizedImage.Dispose();
            }
        }

        void worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            OnComplete(EventArgs.Empty);
        }

        protected virtual void OnProgressChanged(EventArgs e)
        {
            if (ProgressChanged != null)
            {
                ProgressChanged(this, e);
            }
        }

        protected virtual void OnProgressTotalChanged(EventArgs e)
        {
            if (ProgressTotalChanged != null)
            {
                ProgressTotalChanged(this, e);
            }
        }

        protected virtual void OnComplete(EventArgs e)
        {
            if (Complete != null)
            {
                Complete(this, e);
            }
        }

        #region IProgressOperation Members
        public int Total
        {
            get
            {
                return this._total;
            }
            private set
            {
                this._total = value;
                OnProgressTotalChanged(EventArgs.Empty);
            }
        }

        public int Current
        {
            get
            {
                return this._current;
            }
            private set
            {
                this._current = value;
                OnProgressChanged(EventArgs.Empty);
            }
        }
        public void Start()
        {
            BackgroundWorker worker = new BackgroundWorker();
            worker.DoWork += new DoWorkEventHandler(worker_DoWork);
            worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(worker_RunWorkerCompleted);
            worker.RunWorkerAsync();
        }

        public void CancelAsync()
        {
            this._isCancelationPending = true;
        }

        public void Shrink()
        {

        }

        private bool SaveImage(Image newImage, string imageName)
        {
            if (FolderExists())
            {
                try
                {
                    newImage.Save(String.Format("{0}\\SmallerImages\\{1}", _folderPath, imageName), ImageFormat.Jpeg);
                }
                catch (Exception ex)
                {
                    return false;
                }
            }
            return true;
        }
        private bool FolderExists()
        {
            var newPath = _folderPath + "\\SmallerImages";
            try
            {
                if (!System.IO.Directory.Exists(newPath))
                {
                    System.IO.Directory.CreateDirectory(newPath);
                }
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }
        // Need to have this handle the file shrinking process.
        private Image ReduceImageByPercentage(string fileName)
        {
            using (var imageFile = Image.FromFile(fileName))
            {
                var imageHeight = imageFile.Height;
                var imageWidth = imageFile.Width;
                double shrink = (_shrinkPercent / (double)100);

                int newHeight = (int)(shrink * imageHeight);
                int newWidth = (int)(shrink * imageWidth);

                Image newImage = new Bitmap(imageFile, new System.Drawing.Size(newWidth, newHeight));

                return newImage;
            }
        }

        public void Dispose()
        {
            Dispose();
        }

        public event EventHandler ProgressChanged;
        public event EventHandler ProgressTotalChanged;
        public event EventHandler Complete;

        #endregion
    }
}
