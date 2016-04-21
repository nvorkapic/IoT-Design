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

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace TestInterface
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class StatusBar : Page
    {
        public StatusBar()
        {
            this.InitializeComponent();
        }

        private void btnBACK_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(MainPage), null);
        }

        private void btnSetNrOfRuns_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(statusBarSetting), null);
        }

        private void onLoadCatchCurrUsage(object sender, RoutedEventArgs e)
        {
            btnCurrUsage.Content = "Current Usage Calls: " + (Application.Current as TestInterface.App).currentNrofServiceCalls + " out of " + (Application.Current as TestInterface.App).MaxNrBfrMaintenance;
        }

        private void onLoadList(object sender, RoutedEventArgs e)
        {
            ListViewTest.ItemsSource = (Application.Current as TestInterface.App).ReportForMain;
        }
    }
}
