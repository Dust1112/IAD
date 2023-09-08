using System.Numerics;
using System.Runtime.InteropServices;
using IADEditor.Content.Enums;

namespace IADEditor.DLLWrapper.Structs;

[StructLayout(LayoutKind.Sequential)]
public class PrimitiveInitInfo
{
    public PrimitiveMeshType Type;
    public int SegmentX = 1;
    public int SegmentY = 1;
    public int SegmentZ = 1;
    public Vector3 Size = new Vector3(1f);
    public int LOD = 0;
}