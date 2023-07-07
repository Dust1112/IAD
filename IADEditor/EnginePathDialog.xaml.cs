using System.IO;
using System.Windows;

namespace IADEditor;

public partial class EnginePathDialog : Window
{
    public string IADPath { get; private set; }
    
    public EnginePathDialog()
    {
        InitializeComponent();
        Owner = Application.Current.MainWindow;
    }

    private void On_OkButton_CLick(object sender, RoutedEventArgs e)
    {
        string path = PathTextBox.Text.Trim();
        MessageTextBox.Text = string.Empty;

        if (string.IsNullOrEmpty(path))
        {
            MessageTextBox.Text = "Invalid path.";
        }
        else if (path.IndexOfAny(Path.GetInvalidPathChars()) != -1)
        {
            MessageTextBox.Text = "Invalid character(s) used in path.";
        }
        else if (!Directory.Exists(Path.Combine(path, @"Engine\EngineAPI")))
        {
            MessageTextBox.Text = "Enable to find the engine at the specified location.";
        }

        if (string.IsNullOrEmpty(MessageTextBox.Text))
        {
            if (!Path.EndsInDirectorySeparator(path))
            {
                path += @"\";
                IADPath = path;
                DialogResult = true;
                Close();
            }
        }
    }
}