using IADEditor.Common;

namespace IADEditor.Content;

public class Mesh : ViewModelBase
{
    private int _vertexSize;
    public int VertexSize
    {
        get => _vertexSize;
        set
        {
            if (_vertexSize != value)
            {
                _vertexSize = value;
                OnPropertyChanged(nameof(VertexSize));
            }
        }
    }
    
    private int _vertexCount;
    public int VertexCount
    {
        get => _vertexCount;
        set
        {
            if (_vertexCount != value)
            {
                _vertexCount = value;
                OnPropertyChanged(nameof(VertexCount));
            }
        }
    }
    
    private int _indexSize;
    public int IndexSize
    {
        get => _indexSize;
        set
        {
            if (_indexSize != value)
            {
                _indexSize = value;
                OnPropertyChanged(nameof(IndexSize));
            }
        }
    }
    
    private int _indexCount;
    public int IndexCount
    {
        get => _indexCount;
        set
        {
            if (_indexCount != value)
            {
                _indexCount = value;
                OnPropertyChanged(nameof(IndexCount));
            }
        }
    }
    
    public byte[] Vertices { get; set; }
    public byte[] Indices { get; set; }
}