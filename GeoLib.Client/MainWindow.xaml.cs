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
    /// </summary>.
    [CallbackBehavior(UseSynchronizationContext = false)]
    public partial class MainWindow : Window, IUpdateZipCallback
    {
        public MainWindow()
        {
            InitializeComponent();
            _Proxy = new GeoClient(new InstanceContext(this),"tcpEp");
            _Proxy2 = new StatefulGeoClient();
            _SyncContext = SynchronizationContext.Current;

        }

        GeoClient _Proxy = null;
        StatefulGeoClient _Proxy2 = null;
        private SynchronizationContext _SyncContext = null;
        private void btnGetInfo_Click(object sender, RoutedEventArgs e)
        {
            if (txtZipCode.Text != "")
            {
                GeoClient proxy = new GeoClient(new InstanceContext(this),"tcpEp");

                try
                {
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
                catch (FaultException<ExceptionDetail> ex)
                {
                    MessageBox.Show("Exception thrown by service.\n\rException type: " +
                                    "FaultException<ExceptionDetail>" + "\n\r" +
                                    "Message: " + ex.Detail.Message + "\n\r" + "Proxy state: " + proxy.State);
                }
                catch (FaultException<ApplicationException> ex)
                {
                    MessageBox.Show("Exception thrown by service.\n\rException type: " +
                                    "FaultException<ApplicationException>" + "\n\r" +
                                    "Reason: " + ex.Message + "\n\r" +
                                    "Message: " + ex.Detail.Message + "\n\r" + "Proxy state: " + proxy.State);
                }
                catch (FaultException<NotFoundData> ex)
                {
                    MessageBox.Show("Exception thrown by service.\n\rException type: " +
                                    "FaultException<NotFoundData>" + "\n\r" +
                                    "Reason: " + ex.Message + "\n\r" +
                                    "Message: " + ex.Detail.Message + "\n\r" + "Proxy state: " + proxy.State);
                }
                catch (FaultException ex)
                {
                    MessageBox.Show("Faultxception thrown by service.\n\rException type: " +
                                  "FaultException" + "\n\r" +
                                  "Message: " + ex.Message + "\n\r" + "Proxy state: " + proxy.State);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Exception thrown by service.\n\rException type: " + ex.GetType().Name + "\n\r" +
                                    "Message: " + ex.Message + "\n\r" + "Proxy state: " + proxy.State);
                }
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

                ////  proxy.Close();

            }

        }

        private void btn_MakeCall_Click(object sender, RoutedEventArgs e)
        {
            ChannelFactory<IMessageService> factory = new ChannelFactory<IMessageService>("");

            IMessageService proxy = factory.CreateChannel();

            proxy.ShowMsg(txtMessage.Text);

            factory.Close();

        }

        private void btnPush_Click(object sender, RoutedEventArgs e)
        {
            if (txtZipCode2.Text != "")
            {
                _Proxy2.PushZip(txtZipCode2.Text);
            }
        }
        private void btnGetInfo2_Click(object sender, RoutedEventArgs e)
        {

            //GeoClient proxy = new GeoClient("tcpEp");

            ZipCodeData data = _Proxy2.GetZipInfo();

            if (data != null)
            {
                lblCity2.Content = data.City;
                lblState2.Content = data.State;
            }
            else
            {
                lblCity2.Content = "N/A";
                lblState.Content = "N/A";
            }

            // proxy.Close();

        }

        private void btnInRange_Click(object sender, RoutedEventArgs e)
        {
            if (txtRange.Text != "")
            {
                var data = _Proxy2.GetZips(int.Parse(txtRange.Text));
                if (data != null)
                    lstZips2.ItemsSource = data;
            }
        }

        private void btnOneWay_Click(object sender, RoutedEventArgs e)
        {
            GeoClient proxy = new GeoClient(new InstanceContext(this),"tcpEp");

            proxy.OneWayExample();

            MessageBox.Show("Oneway example. Back at client");

            proxy.Close();

            MessageBox.Show("Proxy close");
        }

        private async void btnUpdateBatch_Click(object sender, RoutedEventArgs e)
        {
            List<ZipCityData> cityZipList = new List<ZipCityData>()
            {
                new ZipCityData() { ZipCode = "07035", City = "Bedrock" },
                new ZipCityData() { ZipCode = "33033", City = "End of the World" },
                new ZipCityData() { ZipCode = "90210", City = "Alderan" },
                new ZipCityData() { ZipCode = "07094", City = "Storybrooke" }
            };

            lstZips2.Items.Clear();
           await Task.Run(()=>
            {

                try
                {
                    GeoClient proxy = new GeoClient(new InstanceContext(this), "tcpEp");

                    proxy.UpdateZipCity(cityZipList);

                    proxy.Close();

                    MessageBox.Show("Updated.");
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message);
                }
            });
           
        }

        private async void btnPutBack_Click(object sender, RoutedEventArgs e)
        {
            List<ZipCityData> cityZipList = new List<ZipCityData>()
            {
                new ZipCityData() { ZipCode = "07035", City = "Lincoln Park" },
                new ZipCityData() { ZipCode = "33033", City = "Homestead" },
                new ZipCityData() { ZipCode = "90210", City = "Beverly Hills" },
                new ZipCityData() { ZipCode = "07094", City = "Secaucus" }
            };

            lstZips2.Items.Clear();
            await Task.Run(() =>
            {

                try
                {
                    GeoClient proxy = new GeoClient(new InstanceContext(this), "tcpEp");

                    proxy.UpdateZipCity(cityZipList);

                    proxy.Close();

                    MessageBox.Show("Updated.");
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message);
                }
            });

        }

        public void ZipUpdated(ZipCityData zipCityData)
        {
           // MessageBox.Show($"updated zipcode {zipCityData.ZipCode} with city {zipCityData.City}.");
            SendOrPostCallback updateUI = new SendOrPostCallback(arg =>
            {
                lstZips2.Items.Add(zipCityData);
            });
            _SyncContext.Send(updateUI, null);
        }
    }

}
