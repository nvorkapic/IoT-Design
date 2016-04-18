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

using System;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Controls;

namespace OnScreenKeyboard
{
    public class BoolToSolidBrushConverter : IValueConverter
    { 
        /// <summary>
        /// Convert from source-type to target-type
        /// </summary>
        public object Convert(object value, Type targetType, object parameter, string str)
        {
            if ((bool)value)
            {
                return new SolidColorBrush(Windows.UI.Color.FromArgb(128,128,128,128));
            }
            else
            {
                return new SolidColorBrush(Windows.UI.Color.FromArgb(255,128,128,128));
            }
        }

        /// <summary>
        /// Convert-back from target to source.
        /// </summary>
        public object ConvertBack(object value, Type targetType, object parameter, string str)
        {
            return null;
        }
    }
    public class KeyboardViewModel : ViewModel
    {
        #region constructor
        public KeyboardViewModel(OnScreenKeyBoard container)
        {
            this.container = container;
            KeyModel.theKeyboardViewModel = this;
        }
        #endregion constructor

        #region Commanding

        #region BackspaceCommand

        public RelayCommand BackspaceCommand
        {
            get
            {
                if (backspaceCommand == null)
                {
                    backspaceCommand = new RelayCommand((x) => ExecuteBackspaceCommand());
                }
                return backspaceCommand;
            }
        }

        public void ExecuteBackspaceCommand()
        {
            if (container?.TargetBox is TextBox)
            {
                TextBox box = (container.TargetBox as TextBox);
                //the TextBox.SelectionStart property counts \r\n as one character, but indexing at the string level as two..
                //therefore we will remove all \n occurences
                string text = box.Text.Replace("\r", "");
                if (text == null)
                    text = string.Empty;
                //the following indices are with respect to the text variable, but NOT the TextBox.Text property!
                int currentSelectionStart = box.SelectionStart;
                int currentSelectionLength = box.SelectionLength;

                //if text is selected, delete it
                if (currentSelectionLength != 0)
                    box.Text = text.Remove(currentSelectionStart, currentSelectionLength);
                //no text is selected -> remove the character left of the cursor
                else if (currentSelectionStart > 0)
                {
                    box.Text = text.Remove(currentSelectionStart - 1, 1);
                    box.SelectionStart = currentSelectionStart - 1;//move the cursor one position left
                }
            }
            else if (container?.TargetBox is PasswordBox)
            {
                PasswordBox box = (container.TargetBox as PasswordBox);
                if (box.Password.Length > 0)
                box.Password = box.Password.Remove(box.Password.Length-1);
            }
        }

        private RelayCommand backspaceCommand;

        #endregion BackspaceCommand

        #region CapsLockCommand

        public RelayCommand CapsLockCommand
        {
            get
            {
                if (capsLockCommand == null)
                {
                    capsLockCommand = new RelayCommand((x) => ExecuteCapsLockCommand());
                }
                return capsLockCommand;
            }
        }

        #region ExecuteCapsLockCommand
        /// <summary>
        /// Execute the CapsLockCommand, which ocurrs when the user clicks on the CAPS button.
        /// </summary>
        public void ExecuteCapsLockCommand()
        {
            if (IsCapsLock)
            {
                IsCapsLock = false;
            }
            else
            {
                IsCapsLock = true;
            }
        }
        #endregion

        private RelayCommand capsLockCommand;

        #endregion CapsLockCommand

        #region KeyPressedCommand

        public RelayCommand KeyPressedCommand
        {
            get
            {
                if (keyPressedCommand == null)
                {
                    keyPressedCommand = new RelayCommand((x) => ExecuteKeyPressedCommand(x));
                }
                return keyPressedCommand;
            }
        }

        #region ExecuteKeyPressedCommand
        /// <summary>
        /// Execute the KeyPressedCommand.
        /// </summary>
        /// <param name="arg">The KeyModel of the key-button that was pressed</param>
        public void ExecuteKeyPressedCommand(object arg)
        {
            if (container?.TargetBox is TextBox)
            {
                TextBox box = (container.TargetBox as TextBox);
                //the TextBox.SelectionStart property counts \r\n as one character, but indexing at the string level as two..
                //therefore we will remove all \n occurences
                string text = box.Text.Replace("\r", "");
                if (text == null)
                    text = string.Empty;
                //the following indices are with respect to the text variable, but NOT the TextBox.Text property!
                int currentSelectionStart = box.SelectionStart;
                int currentSelectionLength = box.SelectionLength;

                //if text is selected, delete it
                if (currentSelectionLength != 0)
                    text = text.Remove(currentSelectionStart, currentSelectionLength);

                //insert the new character
                box.Text = text.Insert(currentSelectionStart, (string)arg);
                box.SelectionLength = 0;
                box.SelectionStart = currentSelectionStart + 1;//move the cursor to behind the new character(s in case of return)
            }
            else if (container?.TargetBox is PasswordBox)
            {
                PasswordBox box = (container.TargetBox as PasswordBox);
                box.Password = box.Password += (string)arg;
            }

            //Return to un-shift mode if currently in shift mode
            if (IsShiftLock)
                IsShiftLock = false;
        }
        #endregion

        private RelayCommand keyPressedCommand;

        #endregion KeyPressedCommand

        #region ShiftCommand

        public RelayCommand ShiftCommand
        {
            get
            {
                if (shiftCommand == null)
                {
                    shiftCommand = new RelayCommand((x) => ExecuteShiftCommand());
                }
                return shiftCommand;
            }
        }

        #region ExecuteShiftCommand
        /// <summary>
        /// Execute the ShiftCommand, which ocurrs when the user clicks on the SHIFT key.
        /// </summary>
        public void ExecuteShiftCommand()
        {
            ToggleShiftState();
        }
        #endregion


        private RelayCommand shiftCommand;

        #endregion ShiftCommand

        #endregion Commanding

        #region Methods

        #region NotifyTheIndividualKeys
        /// <summary>
        /// Make the individual key-button view models notify their views that their properties have changed.
        /// </summary>
        public void NotifyTheIndividualKeys(string notificationText)
        {
            if (KB != null && KB.KeyAssignments != null)
            {
                if (notificationText != null)
                {
                    foreach (var keyModel in KB.KeyAssignments)
                    {
                        if (keyModel != null)
                        {
                            keyModel.Notify(notificationText);
                        }
                    }
                }
            }
        }
        #endregion

        #region ToggleShiftState
        /// <summary>
        /// Turn the shift-lock mode off if it's on, and on if it's off.
        /// </summary>
        public void ToggleShiftState()
        {
            if (IsShiftLock)
            {
                IsShiftLock = false;
            }
            else
            {
                IsShiftLock = true;
            }
        }
        #endregion

        #endregion Methods

        #region Properties

        #region IsCapsLock
        /// <summary>
        /// Get/set whether the VirtualKeyboard is currently is Caps-Lock mode.
        /// </summary>
        public bool IsCapsLock
        {
            get
            {
                return isCapsLock;
            }
            set
            {
                if (value != isCapsLock)
                {
                    isCapsLock = value;
                    if (IsShiftLock)
                    {
                        IsShiftLock = false;
                    }
                    else
                    {
                        NotifyTheIndividualKeys("Text");
                    }
                    Notify("IsCapsLock");
                }
            }
        }
        #endregion

        #region IsShiftLock
        /// <summary>
        /// Get/set whether the VirtualKeyboard is currently is Shift-Lock mode.
        /// </summary>
        public bool IsShiftLock
        {
            get
            {
                return shiftKey.IsInShiftState;
            }
            set
            {
                if (value != shiftKey.IsInShiftState)
                {
                    shiftKey.IsInShiftState = value;
                    NotifyTheIndividualKeys("Text");
                    Notify("IsShiftLock");
                }
            }
        }
        #endregion

        #region KB
        /// <summary>
        /// Get/set the specific subclass of KeyAssignmentSet that is currently attached to this keyboard.
        /// </summary>
        public KeyAssignmentSet KB
        {
            get
            {
                if (_currentKeyboardAssignment == null)
                {
                    _currentKeyboardAssignment = KeyAssignmentSet.KeyAssignment;
                }
                return _currentKeyboardAssignment;
            }
        }
        #endregion

        #region The view-model properties for the individual key-buttons of the keyboard

        public KeyModel TabKey
        {
            get { return Tab; }
            set { Tab = value; }
        }

        public KeyModel EnterKey
        {
            get { return Enter; }
            set { Enter = value; }
        }

        public ShiftKeyViewModel ShiftKey
        {
            get { return shiftKey; }
        }

        #endregion The view-model properties for the individual key-buttons of the keyboard

        #region TheKeyModels array
        /// <summary>
        /// Get the array of KeyModels from the KB that comprise the view-models for the keyboard key-buttons
        /// </summary>
        public KeyModel[] TheKeyModels
        {
            get
            {
                if (KB != null)
                {
                    return KB.KeyAssignments;
                }
                return null;
            }
        }
        #endregion

        #endregion Properties

        #region fields

        private bool isCapsLock;

        /// <summary>
        /// The KeyAssignmentSet that is currently attached to this keyboard.
        /// </summary>
        private KeyAssignmentSet _currentKeyboardAssignment;
        // These are view-models for the individual key-buttons of the keyboard which are not provided by the KeyAssignmentSet:
        private KeyModel Tab = new KeyModel('\t', '\t');
        private KeyModel Enter = new KeyModel('\n', '\n');
        private ShiftKeyViewModel shiftKey = new ShiftKeyViewModel();
        private OnScreenKeyBoard container = null;

        #endregion fields
    }
}
