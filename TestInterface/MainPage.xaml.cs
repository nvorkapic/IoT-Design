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
using TestInterface.Report;
// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace TestInterface
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public string RHumi;
        public string RPres;
        public string RTemp;

        public MainPage()
        {
            this.InitializeComponent();

            
            (Application.Current as TestInterface.App).TempCallbacks += TempCallBack;
            (Application.Current as TestInterface.App).PressureCallbacks += PressureCallBack;
            (Application.Current as TestInterface.App).HumidityCallbacks += HumidityCallBack;

          
        }

        private void HumidityCallBack(float humi)
        {
            btnHumidity.Content=String.Format("Relative\nHumidity:\n{0:f2} % ", humi);
            RHumi = String.Format(" Relative Humidity: {0:f2} % |", humi);
        }

        private void PressureCallBack(float press)
        {
            btnPressure.Content=String.Format("Pressure:\n{0:f2} hPa ", press);
            
            RPres = String.Format(" Pressure: {0:f2} hPa |", press);
        }

        private void TempCallBack(float temp)
        {
            btnTemp.Content = string.Format("Temperature:\n{0:f2} °C", temp);
            RTemp = string.Format(" Temperature: {0:f2} °C |", temp);
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

        private void btnMCounter_Click(object sender, RoutedEventArgs e)
        {
            if ((Application.Current as TestInterface.App).MaxNrBfrMaintenance > (Application.Current as TestInterface.App).currentNrofServiceCalls)
            {
                ++(Application.Current as TestInterface.App).currentNrofServiceCalls;
            }

            if ((Application.Current as TestInterface.App).MaxNrBfrMaintenance == (Application.Current as TestInterface.App).currentNrofServiceCalls)
            {
                
                string Datum = DateTime.Now.Year.ToString("0000") + "-" + DateTime.Now.Month.ToString("00") + "-" + DateTime.Now.Day.ToString("00")+" on "+DateTime.Now.Hour.ToString("00")+":"+DateTime.Now.Minute.ToString("00")+":"+DateTime.Now.Second.ToString("00");
                (Application.Current as TestInterface.App).ReportForMain.Insert(0, new Report.ReportList { DTofServiceCall = Datum, SCHumidity = RHumi, SCPressure = RPres, SCTemperature= RTemp, MaxNr = " | Maintenance After "+(Application.Current as TestInterface.App).currentNrofServiceCalls.ToString()+" Run(s) |", Note=" Note: Automatic Insertion |" });
                (Application.Current as TestInterface.App).currentNrofServiceCalls = 0;

                ListViewTest.ItemsSource = (Application.Current as TestInterface.App).ReportForMain;
                button.Content = "Service Called " + (Application.Current as TestInterface.App).ReportForMain.Count + " time(s)";
            }

            btnMCounter.Content = "Usage Calls:\n" + (Application.Current as TestInterface.App).currentNrofServiceCalls + "\nout of\n" + (Application.Current as TestInterface.App).MaxNrBfrMaintenance;

            int barMaxint = (Application.Current as TestInterface.App).MaxNrBfrMaintenance;
            double barMax = double.Parse(barMaxint.ToString());
            ProgBarforStatus.Maximum=barMax;

            int barCurrentint = (Application.Current as TestInterface.App).currentNrofServiceCalls;
            double barCur = double.Parse(barCurrentint.ToString());
            ProgBarforStatus.Value = barCur;

            
        }

        private void onLoadMCounter(object sender, RoutedEventArgs e)
        {
            btnMCounter.Content = "Usage Calls:\n" + (Application.Current as TestInterface.App).currentNrofServiceCalls + "\nout of\n" + (Application.Current as TestInterface.App).MaxNrBfrMaintenance;
        }

        private void barOnLoad(object sender, RoutedEventArgs e)
        {
            int barMaxint = (Application.Current as TestInterface.App).MaxNrBfrMaintenance;
            double barMax = double.Parse(barMaxint.ToString());
            ProgBarforStatus.Maximum = barMax;

            int barCurrentint = (Application.Current as TestInterface.App).currentNrofServiceCalls;
            double barCur = double.Parse(barCurrentint.ToString());
            ProgBarforStatus.Value = barCur;
        }

        private void onLoadList(object sender, RoutedEventArgs e)
        {
            ListViewTest.ItemsSource = (Application.Current as TestInterface.App).ReportForMain;
        }

        private void button_Loaded(object sender, RoutedEventArgs e)
        {
            button.Content = "Service Called "+(Application.Current as TestInterface.App).ReportForMain.Count+" time(s)";
        }

        private void btnAAS_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(AdditionalApps), null);
        }
    }

}
