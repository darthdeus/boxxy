using System;
using System.Collections.Generic;
using System.Linq;
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
using Boxxy.Core;

namespace Boxxy.GUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly HttpProxy _proxy;

        public MainWindow()
        {
            InitializeComponent();
            _proxy = new HttpProxy("http://localhost:8080/", "http://requestb.in/zdbqj5zd");
        }

        private void BtnToggleListening_OnClick(object sender, RoutedEventArgs e)
        {
            if (_proxy.IsRunning)
            {
                _proxy.Stop();
                BtnToggleListening.Content = "Start listening";
            }
            else
            {
                _proxy.Start();
                BtnToggleListening.Content = "Stop listening";
            }
        }
    }
}
