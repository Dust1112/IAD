using System;
using IADEditor.GameProject;
using System.ComponentModel;
using System.IO;
using System.Windows;

namespace IADEditor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        // TODO: Change this every time the repo is cloned on another device
        // TODO: To be changed when version 1.0 released
        public static string IADPath { get; private set; } = @"D:\repos\IAD";

        public MainWindow()
        {
            InitializeComponent();
            Loaded += OnMainWindowLoaded;
            Closing += OnMainWindowClosing;
        }

        private void OnMainWindowLoaded(object sender, RoutedEventArgs e)
        {
            Loaded -= OnMainWindowLoaded;
            GetEnginePath();
            OpenProjectBrowserDialogue();
        }

        private void GetEnginePath()
        {
            string? iadPath = Environment.GetEnvironmentVariable("IAD_ENGINE", EnvironmentVariableTarget.User);
            if (string.IsNullOrEmpty(iadPath) ||
                !Directory.Exists(Path.Combine(iadPath, @"Engine\EngineAPI")))
            {
                EnginePathDialog dialog = new EnginePathDialog();
                if (dialog.ShowDialog() == true)
                {
                    IADPath = dialog.IADPath;
                    Environment.SetEnvironmentVariable("IAD_ENGINE", IADPath.ToUpper(), EnvironmentVariableTarget.User);
                }
                else
                {
                    Application.Current.Shutdown();
                }
            }
            else
            {
                IADPath = iadPath;
            }
        }

        private void OpenProjectBrowserDialogue()
        {
            var projectBrowser = new ProjectBrowserDialog();
            if (projectBrowser.ShowDialog() == false || projectBrowser.DataContext == null)
            {
                Application.Current.Shutdown();
            }
            else
            {
                Project.Current?.Unload();
                DataContext = projectBrowser.DataContext;
            }
        }

        private void OnMainWindowClosing(object? sender, CancelEventArgs e)
        {
            Closing -= OnMainWindowClosing;
            Project.Current?.Unload();
        }
    }
}
