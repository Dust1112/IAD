using System.Collections.ObjectModel;
using IADEditor.Common;

namespace IADEditor.Content;

public class LODGroup : ViewModelBase
{
    private string _name;
    public string Name
    {
        get => _name;
        set
        {
            if (_name != value)
            {
                _name = value;
                OnPropertyChanged(nameof(Name));
            }
        }
    }

    public ObservableCollection<MeshLOD> LODs { get; } = new ObservableCollection<MeshLOD>();
}