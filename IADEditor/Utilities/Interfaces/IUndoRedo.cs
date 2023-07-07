namespace IADEditor.Utilities.Interfaces
{
    public interface IUndoRedo
    {
        string Name { get; }
        void Undo();
        void Redo();
    }
}
