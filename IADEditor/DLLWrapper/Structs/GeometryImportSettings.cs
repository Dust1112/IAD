using System.Runtime.InteropServices;

namespace IADEditor.DLLWrapper.Structs;

[StructLayout(LayoutKind.Sequential)]
public class GeometryImportSettings
{
    public float SmoothingAngle = 178f;
    public byte CalculateNormals = 0;
    public byte CalculateTangents = 1;
    public byte ReverseHandedness = 0;
    public byte ImportEmbededTextures = 1;
    public byte ImportAnimations = 1;
}