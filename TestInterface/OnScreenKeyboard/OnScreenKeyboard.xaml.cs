/**
    Copyright(c) Microsoft Open Technologies, Inc.All rights reserved.
   The MIT License(MIT)
    Permission is hereby granted, free of charge, to any person obtaining a copy
    of this software and associated documentation files(the "Software"), to deal
    in the Software without restriction, including without limitation the rights
    to use, copy, modify, merge, publish, distribute, sublicense, and / or sell
    copies of the Software, and to permit persons to whom the Software is
    furnished to do so, subject to the following conditions :
    The above copyright notice and this permission notice shall be included in
    all copies or substantial portions of the Software.
    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
    IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
    FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.IN NO EVENT SHALL THE
    AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
    LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
    OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
    THE SOFTWARE.
**/

using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace OnScreenKeyboard
{
    public partial class OnScreenKeyBoard : UserControl
    {
        private object _host;
        
        public Control TargetBox { get; private set; }
        

        public void RegisterTarget(TextBox control)
        {
            control.GotFocus += delegate { TargetBox = control; };
            control.LostFocus += delegate { TargetBox = null; };
        }
        public void RegisterTarget(PasswordBox control)
        {
            control.GotFocus += delegate { TargetBox = control; };
            control.LostFocus += delegate { TargetBox = null; };
        }

        public void RegisterHost(object host)
        {
            if (host != null)
            {
                _host = host;
            }
        }

        public object GetHost()
        {
            return _host;
        }

        
        private void Target_GotFocus(object sender, RoutedEventArgs e)
        {
            var t = sender as TextBox;
            if (t.FocusState == FocusState.Pointer)
            {
                //this.IsEnabled = true;
                //turn on the lights
            }
        }

        public OnScreenKeyBoard()
        {
            DataContext = new KeyboardViewModel(this);
            InitializeComponent();
        }

    }
}

