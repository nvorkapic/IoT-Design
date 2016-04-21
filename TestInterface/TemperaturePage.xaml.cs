using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using TestInterface.TemperatureControl;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml.Shapes;
using WinRTXamlToolkit.Controls.DataVisualization.Charting;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace TestInterface
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class TemperaturePage : Page
    {
        private Queue<TempControl> TempNdTime = new Queue<TempControl>();
        
        public TemperaturePage()
        {
            this.InitializeComponent();
            TempChart.LegendItems.Clear();

            (Application.Current as TestInterface.App).TempCallbacks += TempCallback;

        }

        private void TempCallback(float temp)
        {
            btnCurrentTemp.Content = string.Format("Temperature: {0:f2} °C", temp);

           
            if (TempNdTime.Count >= 15)
            {
                TempNdTime.Dequeue();

            }
            TempNdTime.Enqueue(new TempControl { Temperature = double.Parse(temp.ToString()), DTReading =DateTime.Now.Hour.ToString("00") + DateTime.Now.Minute.ToString("00") + DateTime.Now.Second.ToString("00") });

            (TempChart.Series[0] as LineSeries).ItemsSource = TempNdTime.ToList();
        }

        private void btnBACK_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(MainPage), null);
        }


        //private void buttonLoadTemp_Click(object sender, RoutedEventArgs e)
        //{
        //    TempControl Init = new TempControl();
        //    List<TempControl> DataList = new List<TempControl>();
        //    DataList = Init.CreateTemp();

        //    double canvasHeight = TempCanvas.Height;
        //    double canvasWidth = TempCanvas.Width;

        //    double MaxTemp = 100;
        //    double MinTemp = 0;
        //    foreach (var item in DataList)
        //    {
        //        MaxTemp = item.Temperature > MaxTemp ? item.Temperature : MaxTemp;
        //        MinTemp = item.Temperature < MinTemp ? item.Temperature : MinTemp;
        //    }

        //    double MaxDate = 31;

        //    var coordinates = new List<Tuple<double, double>>();

        //    foreach (var dataitem in DataList)
        //    {
        //        double x = (canvasWidth * (dataitem.DTReading / MaxDate));
        //        double y = canvasHeight * (dataitem.Temperature / MaxTemp);
        //        coordinates.Add(new Tuple<double, double>(x, y));
        //    }

        //    Color c = new Color() { R = 255, G = 255, B = 255 };

        //    for (int i = 0; i <= 29; i++)
        //    {
        //        var line = new Line() { X1 = coordinates[i].Item1, Y1 = coordinates[i].Item2, X2 = coordinates[i + 1].Item1, Y2 = coordinates[i + 1].Item2, Stroke = new SolidColorBrush(c), StrokeThickness = 2.0 };
        //        TempCanvas.Children.Add(line);
        //    }
        //}
    }
}
