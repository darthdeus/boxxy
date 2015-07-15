using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
        public ObservableCollection<IncomingHttpRequest> AllRequests { get; set; }
        public ObservableCollection<Header> SelectedHeaders { get; set; }

        public MainWindow()
        {
            AllRequests = new ObservableCollection<IncomingHttpRequest>();
            SelectedHeaders = new ObservableCollection<Header>();
            InitializeComponent();
            _proxy = new HttpProxy("http://localhost:8080/", "http://requestb.in/zdbqj5zd");
            BtnToggleListening_OnClick(null, null);
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
                _proxy.Request += request =>
                {
                    Dispatcher.Invoke(() => LstRequests.Items.Add(request));
                };
                _proxy.Start();
                BtnToggleListening.Content = "Stop listening";
            }
        }

        private void LstRequests_OnSelected(object sender, RoutedEventArgs e)
        {
            var request = (IncomingHttpRequest)LstRequests.SelectedItem;
            LstHeaders.Items.Clear();

            foreach (var header in request.Headers) {
                LstHeaders.Items.Add(header);
                //LstHeaders.Items.Add(string.Format("{0}: {1}", header.Name, header.Value));
            }
        }
    }
}
