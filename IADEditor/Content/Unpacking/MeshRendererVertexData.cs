using System.Windows.Media;
using System.Windows.Media.Media3D;
using IADEditor.Common;

namespace IADEditor.Content.Unpacking;

// NOTE: the purpose of this class is to enable viewing 3D geometry in WPF while
// I don't have a graphic renderer in the game engine. When I have a renderer,
// this class and the WPF viewer will become obsolete.
public class MeshRendererVertexData : ViewModelBase
{
    private Brush _specular = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#ff111111"));
    public Brush Specular
    {
        get => _specular;
        set
        {
            if (_specular != value)
            {
                _specular = value;
                OnPropertyChanged(nameof(Specular));
            }
        }
    }

    private Brush _diffuse = Brushes.White;
    public Brush Diffuse
    {
        get => _diffuse;
        set
        {
            if (_diffuse != value)
            {
                _diffuse = value;
                OnPropertyChanged(nameof(Diffuse));
            }
        }
    }
    
    public Point3DCollection Positions { get; } = new Point3DCollection();
    public Vector3DCollection Normals { get; } = new Vector3DCollection();
    public PointCollection UVs { get; } = new PointCollection();
    public Int32Collection Indices { get; } = new Int32Collection();
}