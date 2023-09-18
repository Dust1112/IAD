using System.Runtime.InteropServices;

namespace IADEditor.DLLWrapper.Structs;

[StructLayout(LayoutKind.Sequential)]
public class GeometryImportSettings
{
    public float SmoothingAngle = 178f;
    public byte CalculateNormals = 0;
    public byte CalculateTangents = 1;
    public byte ReverseHandedness = 0;
    public byte ImportEmbeddedTextures = 1;
    public byte ImportAnimations = 1;

    public void FromContentSettings(Content.Geometry geometry)
    {
        var settings = geometry.ImportSettings;
        SmoothingAngle = settings.SmoothingAngle;
        CalculateNormals = ToByte(settings.CalculateNormals);
        CalculateTangents = ToByte(settings.CalculateTangents);
        ReverseHandedness = ToByte(settings.ReverseHandedness);
        ImportEmbeddedTextures = ToByte(settings.ImportEmbeddedTextures);
        ImportAnimations = ToByte(settings.ImportAnimations);
    }

    private byte ToByte(bool value) => value ? (byte)1 : (byte)0;
}