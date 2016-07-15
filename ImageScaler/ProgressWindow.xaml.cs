using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using ImageScaler.Annotations;
using ImageScaler.Interfaces;

namespace ImageScaler
{
    /// <summary>
    /// Interaction logic for ProgressWindow.xaml
    /// </summary>
    public partial class ProgressWindow : Window, INotifyPropertyChanged
    {
        private IProgressOperation _operation;

        public ProgressWindow(IProgressOperation operation)
        {
            this._operation = operation;
            this._operation.ProgressChanged += new EventHandler(_operation_ProgressChanged);
            this._operation.ProgressTotalChanged += new EventHandler(_operation_TotalChanged);
            this._operation.Complete += new EventHandler(_operation_Complete);

            InitializeComponent();

            this.Loaded += new RoutedEventHandler(ProgressWindow_Loaded);
        }

        private void ProgressWindow_Loaded(object sender, RoutedEventArgs e)
        {
            this._operation.Start();
        }

        private void _operation_Complete(object sender, EventArgs e)
        {
            Close();
        }

        private void _operation_ProgressChanged(object sender, EventArgs e)
        {
            OnPropertyChanged("Current");
        }

        private void _operation_TotalChanged(object sender, EventArgs e)
        {
            OnPropertyChanged("Total");
        }

        private void CancelClick(object sender, RoutedEventArgs e)
        {
            this._operation.CancelAsync();
        }

        public int Current
        {
            get { return this._operation.Current; }
        }

        public int Total
        {
            get { return this._operation.Total; }
        }

        #region INotifyPropertyChanged Members

        /// <summary>
        /// Notify property changed
        /// </summary>
        /// <param name="propertyName">Property name</param>
        protected void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion      


    }
}
