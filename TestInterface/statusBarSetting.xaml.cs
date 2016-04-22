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
using Windows.UI;
using Windows.UI.Xaml.Media.Animation;
using OnScreenKeyboard;
using System.Text.RegularExpressions;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace TestInterface
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class statusBarSetting : Page
    {
        public string RHumi;
        public string RPres;
        public string RTemp;

        public statusBarSetting()
        {
            this.InitializeComponent();
            keyboard.RegisterTarget(textBoxNote);
            keyboard.RegisterTarget(MaxServiceNr);

            (Application.Current as TestInterface.App).TempCallbacks += TempCallBack;
            (Application.Current as TestInterface.App).PressureCallbacks += PressureCallBack;
            (Application.Current as TestInterface.App).HumidityCallbacks += HumidityCallBack;
        }

        private void HumidityCallBack(float humi)
        {
            RHumi = String.Format(" Relative Humidity: {0:f2} % |", humi);
        }

        private void PressureCallBack(float press)
        {
            RPres = String.Format(" Pressure: {0} hPa |", press);
        }

        private void TempCallBack(float temp)
        {
            RTemp = string.Format(" Temperature: {0:f2} °C |", temp);
        }

        private void btnBACK_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(StatusBar), null);
        }

        private void maxNr_GotFocus(object sender, RoutedEventArgs e)
        {
            MaxServiceNr.Text = "";
            
        }

        private void NoteGotFocus(object sender, RoutedEventArgs e)
        {
            textBoxNote.Text = "";
        }

        private void parseCheckMaxNR(object sender, RoutedEventArgs e)
        {
            int broj;
            if(!int.TryParse(MaxServiceNr.Text,out broj))
            {
                MaxServiceNr.Text = "";
                MaxServiceNr.Focus(FocusState.Keyboard);

            }
            else if (broj > 9999 || broj <= 0)
            {
                MaxServiceNr.Text = "";
                MaxServiceNr.Focus(FocusState.Keyboard);
            }
            else
            {
                MaxServiceNr.Text = broj.ToString();
            }

        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            (Application.Current as TestInterface.App).MaxNrBfrMaintenance = int.Parse(MaxServiceNr.Text);
            string Datum = DateTime.Now.Year.ToString("0000") + "-" + DateTime.Now.Month.ToString("00") + "-" + DateTime.Now.Day.ToString("00") + " on " + DateTime.Now.Hour.ToString("00") + ":" + DateTime.Now.Minute.ToString("00") + ":" + DateTime.Now.Second.ToString("00");
            if (textBoxNote.Text=="" || textBoxNote.Text == null || textBoxNote.Text == "Enter your note here.") { textBoxNote.Text = " No Entry on Note. Automatic Insert. Maximum calls changed."; }
            (Application.Current as TestInterface.App).ReportForMain.Insert(0, new Report.ReportList { DTofServiceCall = Datum, SCHumidity = RHumi, SCPressure = RPres, SCTemperature = RTemp, MaxNr = " | Maintenance After " + (Application.Current as TestInterface.App).currentNrofServiceCalls.ToString() + " Run(s) |", Note ="Note: " + textBoxNote.Text });
            (Application.Current as TestInterface.App).currentNrofServiceCalls = 0;
            this.Frame.Navigate(typeof(StatusBar), null);
        }

        private void onLoadMSN(object sender, RoutedEventArgs e)
        {
            MaxServiceNr.Text = (Application.Current as TestInterface.App).MaxNrBfrMaintenance.ToString();
        }
    }
}
