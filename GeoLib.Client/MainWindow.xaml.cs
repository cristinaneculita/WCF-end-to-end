using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Text;
using System.Threading;
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
using GeoLib.Client.Contracts;
using GeoLib.Contracts;
using GeoLib.Proxies;
using Binding = System.ServiceModel.Channels.Binding;


namespace GeoLib.Client
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            _Proxy = new GeoClient("tcpEp");

        }

        GeoClient _Proxy = null;
        private void btnGetInfo_Click(object sender, RoutedEventArgs e)
        {
            if (txtZipCode.Text != "")
            {
                GeoClient proxy = new GeoClient("tcpEp");

                ZipCodeData data = proxy.GetZipInfo(txtZipCode.Text);

                if (data != null)
                {
                    lblCity.Content = data.City;
                    lblState.Content = data.State;
                }
                else
                {
                    lblCity.Content = "N/A";
                    lblState.Content = "N/A";
                }

                proxy.Close();
            }
        }

        private void btnGetZipCodes_Click(object sender, RoutedEventArgs e)
        {
            if (txtState.Text != null)
            {
                //GeoClient proxy = new GeoClient("tcpEp");

                IEnumerable<ZipCodeData> data = _Proxy.GetZips(txtState.Text);
                if (data != null)
                {
                    lstZips.ItemsSource = data;
                }

               // proxy.Close();

            }

        }

        private void btn_MakeCall_Click(object sender, RoutedEventArgs e)
        {
            ChannelFactory<IMessageService> factory = new ChannelFactory<IMessageService>("");

            IMessageService proxy = factory.CreateChannel();

            proxy.ShowMsg(txtMessage.Text);

            factory.Close();

        }

       

    }

}
