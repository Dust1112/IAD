using IADEditor.Utilities.Interfaces;
using System;
using System.Diagnostics;

namespace IADEditor.Utilities
{
    public class UndoRedoAction : IUndoRedo
    {
        private Action _undoAction;
        private Action _redoAction;

        public string Name { get; }

        public UndoRedoAction(string name)
        {
            Name = name;
        }

        public UndoRedoAction(Action undo, Action redo, string name)
            : this(name)
        {
            Debug.Assert(undo != null && redo != null);
            _undoAction = undo;
            _redoAction = redo;
        }

        public UndoRedoAction(string property, object instance, object undoValue, object redoValue, string name) :
            this(
                () => instance.GetType().GetProperty(property)?.SetValue(instance, undoValue),
                () => instance.GetType().GetProperty(property)?.SetValue(instance, redoValue),
                name)
        {

        }

        public void Redo() => _redoAction.Invoke();

        public void Undo() => _undoAction.Invoke();
    }
}
