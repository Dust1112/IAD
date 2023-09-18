using IADEditor.Common;

namespace IADEditor.Content;

public class GeometryImportSettings : ViewModelBase
{
    private bool _calculateNormals;
    public bool CalculateNormals
    {
        get => _calculateNormals;
        set
        {
            if (_calculateNormals != value)
            {
                _calculateNormals = value;
                OnPropertyChanged(nameof(CalculateNormals));
            }
        }
    }
    
    private bool _calculateTangents;
    public bool CalculateTangents
    {
        get => _calculateTangents;
        set
        {
            if (_calculateTangents != value)
            {
                _calculateTangents = value;
                OnPropertyChanged(nameof(CalculateTangents));
            }
        }
    }
    
    private float _smoothingAngle;
    public float SmoothingAngle
    {
        get => _smoothingAngle;
        set
        {
            if (_smoothingAngle != value)
            {
                _smoothingAngle = value;
                OnPropertyChanged(nameof(SmoothingAngle));
            }
        }
    }
    
    private bool _reverseHandedness;
    public bool ReverseHandedness
    {
        get => _reverseHandedness;
        set
        {
            if (_reverseHandedness != value)
            {
                _reverseHandedness = value;
                OnPropertyChanged(nameof(ReverseHandedness));
            }
        }
    }
    
    private bool _importEmbeddedTextures;
    public bool ImportEmbeddedTextures
    {
        get => _importEmbeddedTextures;
        set
        {
            if (_importEmbeddedTextures != value)
            {
                _importEmbeddedTextures = value;
                OnPropertyChanged(nameof(ImportEmbeddedTextures));
            }
        }
    }
    
    private bool _importAnimations;
    public bool ImportAnimations
    {
        get => _importAnimations;
        set
        {
            if (_importAnimations != value)
            {
                _importAnimations = value;
                OnPropertyChanged(nameof(ImportAnimations));
            }
        }
    }

    public GeometryImportSettings()
    {
        CalculateNormals = false;
        CalculateTangents = false;
        SmoothingAngle = 178f;
        ReverseHandedness = false;
        ImportEmbeddedTextures = false;
        ImportAnimations = false;
    }
}