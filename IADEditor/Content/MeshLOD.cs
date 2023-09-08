using System.Collections.ObjectModel;
using IADEditor.Common;

namespace IADEditor.Content;

public class MeshLOD : ViewModelBase
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
    
    private float _lodThreshold;
    public float LodThreshold
    {
        get => _lodThreshold;
        set
        {
            if (_lodThreshold != value)
            {
                _lodThreshold = value;
                OnPropertyChanged(nameof(LodThreshold));
            }
        }
    }

    public ObservableCollection<Mesh> Meshes { get; } = new ObservableCollection<Mesh>();
}