using IADEditor.Components;
using IADEditor.GameProject;
using IADEditor.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;

namespace IADEditor.Editors
{
    /// <summary>
    /// Interaction logic for GameEntityView.xaml
    /// </summary>
    public partial class GameEntityView : UserControl
    {
        private Action _undoAction;
        private string _propertyName;

        public static GameEntityView Instance { get; private set; }

        public GameEntityView()
        {
            InitializeComponent();
            DataContext = null;
            Instance = this;
            DataContextChanged += (_, __) =>
            {
                if (DataContext != null)
                {
                    (DataContext as MSEntity)!.PropertyChanged += (s, e) => _propertyName = e.PropertyName!;
                }
            };
        }

        private void OnName_TextBox_GotKeyboardFocus(object sender, System.Windows.Input.KeyboardFocusChangedEventArgs e)
        {
            _propertyName = string.Empty;
            _undoAction = GetRenameAction();
        }

        private void OnName_TextBox_LostKeyboardFocus(object sender, System.Windows.Input.KeyboardFocusChangedEventArgs e)
        {
            if (_propertyName == nameof(MSEntity.Name) && _undoAction != null)
            {
                Action redoAction = GetRenameAction();
                Project.UndoRedo.Add(new UndoRedoAction(_undoAction, redoAction, "Rename game entity"));
                _propertyName = null!;
            }
            _undoAction = null!;
        }

        private void OnIsEnabled_CheckBox_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            Action undoAction = GetIsEnabledAction();
            MSEntity? vm = DataContext as MSEntity;
            vm!.IsEnabled = (sender as CheckBox)?.IsChecked == true;

            Action redoAction = GetIsEnabledAction();
            Project.UndoRedo.Add(new UndoRedoAction(undoAction, redoAction,
                vm.IsEnabled == true ? "Enable game entity" : "Disable game entity"));
        }

        private Action GetRenameAction()
        {
            MSEntity? vm = DataContext as MSEntity;
            List<(GameEntity entity, string Name)>? selection = vm?.SelectedEntities.Select(entity => (entity, entity.Name)).ToList();
            return new Action(() =>
            {
                selection?.ForEach(item => item.entity.Name = item.Name);
                (DataContext as MSEntity)?.Refresh();
            });
        }

        private Action GetIsEnabledAction()
        {
            MSEntity? vm = DataContext as MSEntity;
            List<(GameEntity entity, bool IsEnabled)>? selection = vm?.SelectedEntities.Select(entity => (entity, entity.IsEnabled)).ToList();
            return new Action(() =>
            {
                selection?.ForEach(item => item.entity.IsEnabled = item.IsEnabled);
                (DataContext as MSEntity)?.Refresh();
            });
        }
    }
}
