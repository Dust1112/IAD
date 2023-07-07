﻿using IADEditor.Utilities.Enums;
using System.Windows;
using System.Windows.Controls;

namespace IADEditor.Utilities
{
    /// <summary>
    /// Interaction logic for LoggerView.xaml
    /// </summary>
    public partial class LoggerView : UserControl
    {
        public LoggerView()
        {
            InitializeComponent();
        }

        private void OnClear_Button_Click(object sender, RoutedEventArgs e)
        {
            Logger.Clear();
        }

        private void OnMessageFilter_Button_Click(object sender, RoutedEventArgs e)
        {
            int filter = 0x0;
            if (toggleInfo.IsChecked == true) { filter |= (int)MessageType.Info; }
            if (toggleWarning.IsChecked == true) { filter |= (int)MessageType.Warning; }
            if (toggleError.IsChecked == true) { filter |= (int)MessageType.Error; }
            Logger.SetMessageFilter(filter);
        }
    }
}
