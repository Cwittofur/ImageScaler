using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using Microsoft.Win32;
using MessageBox = System.Windows.MessageBox;

namespace ImageScaler
{
    public class ScalerViewModel : INotifyPropertyChanged
    {
        private string _folderPath;
        private readonly SynchronizationContext _synchronizationContext = SynchronizationContext.Current;
        
        #region Properties
        public string FolderPath
        {
            get { return _folderPath; }
            set
            {
                _folderPath = value;
                Notify();
            }
        }

        private int _count;
        public int ImageCount
        {
            get { return _count; }
            set
            {
                _count = value;
                Notify();
            }
        }

        private string[] _imageList;
        public string[] ImageList
        {
            get { return _imageList; }
            set
            {
                _imageList = value;
                Notify();
            }
        }

        private int _progressValue;
        public int ProgressValue
        {
            get { return _progressValue; }
            set
            {
                _progressValue = value;
                OnPropertyChanged("ProgressValue");
            }
        }

        private int _shrinkPercent = 25;
        public int ShrinkPercent
        {
            get { return _shrinkPercent; }
            set
            {
                _shrinkPercent = value;
                Notify();
            }
        }
        #endregion

        // Set field and raise PropertyChanged event for the given property name
        protected void SetFieldAndNotify<T>(ref T field, T value, [CallerMemberName] string propertyName = "")
        {
            if (!EqualityComparer<T>.Default.Equals(field, value))
            {
                field = value;
                Notify(propertyName);
            }
        }
        public void OnProgressChanged(object sender, ProgressChangedEventArgs args)
        {
            _synchronizationContext.Send(delegate { ProgressValue = args.ProgressPercentage; }, null);
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string name)
        {
            var handler = Interlocked.CompareExchange(ref PropertyChanged, null, null);
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(name));
            }
        }
        // Raise PropertyChanged event for the given property name
        protected void Notify([CallerMemberName] string propertyName = "")
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void Browse()
        {
            FolderBrowserDialog fldDlg = new FolderBrowserDialog();

            fldDlg.ShowDialog();

            FolderPath = fldDlg.SelectedPath;

            // Add combo box to use different file types ?
            var list = System.IO.Directory.GetFiles(FolderPath, "*.jpg");

            ImageList = list;
            ImageCount = list.Count();

        }

        public void Reducto()
        {
            int progressPercentage = 0;
            for (int i = 0; i <= 100; i++)
            {
                this.OnProgressChanged(this, new ProgressChangedEventArgs(progressPercentage, null));
                ++progressPercentage;
                Thread.Sleep(50);
            }
        }

        
    }
}
