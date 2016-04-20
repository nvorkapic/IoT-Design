using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using TestInterface.TemperatureControl;
using System.Collections.ObjectModel;
using System.ServiceModel.Channels;
using static System.Collections.Specialized.BitVector32;
using System.Threading.Tasks;
using System.Threading;
// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace TestInterface
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        
        
        public MainPage()
        {
            this.InitializeComponent();


            (Application.Current as TestInterface.App).TempCallbacks += TempCallBack ;
        }

        private void TempCallBack(float temp)
        {
            btnTemp.Content = string.Format("Temperature: {0:f2} °C", temp);
        }

        private void btnTemp_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(TemperaturePage), null);
        }

        private void btnPressure_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(PressurePage), null);
            
        }

        private void btnHumidity_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(HumidityPage), null);
        }

        private void btnStatus_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(StatusBar), null); 
        }
    }

}
