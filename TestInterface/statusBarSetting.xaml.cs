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

        public statusBarSetting()
        {
            this.InitializeComponent();
            keyboard.RegisterTarget(textBoxNote);
            keyboard.RegisterTarget(MaxServiceNr);
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
    }
}
