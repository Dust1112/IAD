using System.Windows;
using System.Windows.Controls;
using IADEditor.Content;
using IADEditor.GameDev;

namespace IADEditor.Editors
{
    /// <summary>
    /// Interaction logic for WorldEditorView.xaml
    /// </summary>
    public partial class WorldEditorView : UserControl
    {
        public WorldEditorView()
        {
            InitializeComponent();
            Loaded += OnWorldEditorViewLoaded;
        }

        private void OnWorldEditorViewLoaded(object sender, RoutedEventArgs e)
        {
            Loaded -= OnWorldEditorViewLoaded;
            Focus();
        }

        private void OnNewScript_Button_Click(object sender, RoutedEventArgs e)
        {
            NewScriptDialog dlg = new NewScriptDialog();
            dlg.ShowDialog();
        }

        private void OnCreatePrimitiveMesh_Button_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new PrimitiveMeshDialog();
            dialog.ShowDialog();
        }

        private void OpenSettingsMenu_Button_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new SettingsDialog();
            dialog.ShowDialog();
        }
    }
}
