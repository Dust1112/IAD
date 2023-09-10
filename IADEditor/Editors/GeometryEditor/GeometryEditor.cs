using System.Diagnostics;
using IADEditor.Common;
using IADEditor.Content.Unpacking;

namespace IADEditor.Editors.GeometryEditor;

public class GeometryEditor : ViewModelBase, IAssetEditor
{
    public Content.Asset Asset => Geometry;

    private Content.Geometry _geometry; // My geometry class
    public Content.Geometry Geometry
    {
        get => _geometry;
        set
        {
            if (_geometry != value)
            {
                _geometry = value;
                OnPropertyChanged(nameof(Geometry));
            }
        }
    }
    
    private MeshRenderer _meshRenderer;
    public MeshRenderer MeshRenderer
    {
        get => _meshRenderer;
        set
        {
            if (_meshRenderer != value)
            {
                _meshRenderer = value;
                OnPropertyChanged(nameof(MeshRenderer));
            }
        }
    }
    
    public void SetAsset(Content.Asset asset)
    {
        Debug.Assert(asset is Content.Geometry);
        if (asset is Content.Geometry geometry)
        {
            Geometry = geometry;
            MeshRenderer = new MeshRenderer(Geometry.GetLODGroup().LODs[0], MeshRenderer);
        }
    }
}