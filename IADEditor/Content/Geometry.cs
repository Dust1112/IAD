using System.Diagnostics;
using IADEditor.Content.Enums;

namespace IADEditor.Content;

public class Geometry : Asset
{
    public Geometry() : base(AssetType.Mesh)
    {
    }

    public void FromRawData(byte[] data)
    {
        Debug.Assert(data.Length > 0);
        
    }
}