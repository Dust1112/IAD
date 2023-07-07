using IADEditor.Components;
using IADEditor.GameProject;
using IADEditor.Utilities;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
namespace IADEditor.Editors
{
    /// <summary>
    /// Interaction logic for ProjectLayoutView.xaml
    /// </summary>
    public partial class ProjectLayoutView : UserControl
    {
        public ProjectLayoutView()
        {
            InitializeComponent();
        }

        private void OnAddGameEntity_Button_Click(object sender, RoutedEventArgs e)
        {
            Button btn = (sender as Button)!;
            Scene vm = (btn!.DataContext as Scene)!;
            vm!.AddGameEntityCommand.Execute(new GameEntity(vm) { Name = "Empty Game Entity" });
        }

        private void OnGameEntities_ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ListBox? listBox = sender as ListBox;
            List<GameEntity>? newSelection = listBox?.SelectedItems.Cast<GameEntity>().ToList();
            List<GameEntity>? previousSelection = newSelection?.Except(e.AddedItems.Cast<GameEntity>()).Concat(e.RemovedItems.Cast<GameEntity>()).ToList();

            Project.UndoRedo.Add(new UndoRedoAction(
                () => // undo action
                {
                    listBox?.UnselectAll();
                    previousSelection?.ForEach(x => (listBox?.ItemContainerGenerator.ContainerFromItem(x) as ListBoxItem)!.IsSelected = true);
                },
                () => //redo action
                {
                    listBox?.UnselectAll();
                    newSelection?.ForEach(x => (listBox?.ItemContainerGenerator.ContainerFromItem(x) as ListBoxItem)!.IsSelected = true);
                },
                "Selection changed"
                ));

            MSGameEntity msEntity = null!;
            if (newSelection!.Any())
            {
                msEntity = new MSGameEntity(newSelection!);
            }
            GameEntityView.Instance.DataContext = msEntity;
        }
    }
}
