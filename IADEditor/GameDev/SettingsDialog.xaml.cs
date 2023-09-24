using System.Windows;
using System.Windows.Controls;
using IADEditor.Utilities;
using IADEditor.Utilities.Enums;

namespace IADEditor.GameDev;

public partial class SettingsDialog
{
    private bool _textChanged = false;
    private bool _suppressTextChanged = false;

    public SettingsDialog()
    {
        InitializeComponent();
    }

    private void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        var listBox = sender as ListBox;
        var selectedItem = listBox?.SelectedItem as ListBoxItem;

        if (selectedItem != null)
        {
            _suppressTextChanged = true;  // Suppress the TextChanged event.
            DynamicContentPresenter.ContentTemplate = selectedItem.Tag as DataTemplate;
            _suppressTextChanged = false;  // Reset suppression.
        }
    }

    private void OnTextChanged(object sender, TextChangedEventArgs e)
    {
        if (!_suppressTextChanged)
        {
            _textChanged = true;
            SaveButton.IsEnabled = _textChanged;
        }
    }

    private async void OnSave_Button_Click(object sender, RoutedEventArgs e)
    {
        _textChanged = false;
        SaveButton.IsEnabled = false;

        if (DataContext is Settings vm)
        {
            if (ContentHelper.IsValidPath(vm.CompilerText) && ContentHelper.IsValidPath(vm.EditorText))
            {
                await vm.SetEnvironmentVariables();
                Logger.Log(MessageType.Info, "Changes applied successfully.");
            }
            else
            {
                Logger.Log(MessageType.Error, "Executable not found or incorrect path.");
            }
        }
    }
}