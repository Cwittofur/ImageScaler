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
using System.Windows.Navigation;
using System.Windows.Shapes;
using ImageScaler.Resources;
using ImageScaler.Helpers;

namespace ImageScaler
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        ScalerViewModel view = new ScalerViewModel();
        public MainWindow()
        {
            InitializeComponent();
            DataContext = view;
        }

        public void OnBrowse()
        {
            
        }

        private void btnBrowse2_Click(object sender, RoutedEventArgs e)
        {
            view.Browse();
        }


        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {            
            ProgressWindow progressWindow = new ProgressWindow(new ImageProcessor(view.ShrinkPercent, view.ImageList, view.FolderPath));
            //ProgressWindow progressWindow = new ProgressWindow(new EventLogExporter());
            progressWindow.ShowDialog();
        }
    }
}
