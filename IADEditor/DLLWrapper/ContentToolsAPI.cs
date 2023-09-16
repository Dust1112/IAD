using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using IADEditor.Content;
using IADEditor.DLLWrapper.Structs;
using IADEditor.Utilities;
using IADEditor.Utilities.Enums;

namespace IADEditor.DLLWrapper;

public static class ContentToolsAPI
{
    private const string _toolsDll = "ContentTools.dll";

    [DllImport(_toolsDll)]
    private static extern void CreatePrimitiveMesh([In, Out] SceneData data, PrimitiveInitInfo info);

    public static void CreatePrimitiveMesh(Geometry geometry, PrimitiveInitInfo info)
    {
        Debug.Assert(geometry != null);
        using var sceneData = new SceneData();

        try
        {
            CreatePrimitiveMesh(sceneData, info);
            Debug.Assert(sceneData.Data != IntPtr.Zero && sceneData.DataSize > 0);
            var data = new byte[sceneData.DataSize];
            Marshal.Copy(sceneData.Data, data, 0, sceneData.DataSize);
            geometry.FromRawData(data);
        }
        catch (Exception e)
        {
           Logger.Log(MessageType.Error, $"Failed to create {info.Type} primitive mesh.");
           Debug.WriteLine(e.Message);
        }
    }
}