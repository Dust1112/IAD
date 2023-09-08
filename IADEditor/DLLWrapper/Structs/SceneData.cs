using System;
using System.Runtime.InteropServices;

namespace IADEditor.DLLWrapper.Structs;

[StructLayout(LayoutKind.Sequential)]
public class SceneData : IDisposable
{
    public IntPtr Data;
    public int DataSize;
    public GeometryImportSettings ImportSettings = new GeometryImportSettings();

    public void Dispose()
    {
        Marshal.FreeCoTaskMem(Data);
        GC.SuppressFinalize(this);
    }

    ~SceneData()
    {
        Dispose();
    }
}