using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using TestInterface.PressureControl;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using WinRTXamlToolkit.Controls.DataVisualization.Charting;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace TestInterface
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class PressurePage : Page
    {
        private Queue<PressControl> PressNdTime = new Queue<PressControl>();

        public PressurePage()
        {
            this.InitializeComponent();

            PressChart.LegendItems.Clear();
            (Application.Current as TestInterface.App).PressureCallbacks += PressureCallBack;
        }

        private void PressureCallBack(float press)
        {
            btnCurrentPress.Content = String.Format("Pressure:\n{0:f2} hPa", press);

            if (PressNdTime.Count >= 15)
            {
                PressNdTime.Dequeue();

            }
            PressNdTime.Enqueue(new PressControl { Pressure = double.Parse(press.ToString()), DTReading = DateTime.Now.Hour.ToString("00") + DateTime.Now.Minute.ToString("00") + DateTime.Now.Second.ToString("00") });

            (PressChart.Series[0] as LineSeries).ItemsSource = PressNdTime.ToList();
        }

        private void btnBACK_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(MainPage), null);
        }
    }
}
