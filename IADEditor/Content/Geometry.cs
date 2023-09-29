using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using IADEditor.Content.Enums;
using IADEditor.Content.Unpacking;
using IADEditor.Editors.GeometryEditor;
using IADEditor.Utilities;
using IADEditor.Utilities.Enums;

namespace IADEditor.Content;

public class Geometry : Asset
{
    private readonly List<LODGroup?> _lodGroups = new List<LODGroup?>();
    public GeometryImportSettings ImportSettings { get; } = new GeometryImportSettings();
    
    public Geometry() : base(AssetType.Mesh)
    {
    }

    public void FromRawData(byte[] data)
    {
        Debug.Assert(data.Length > 0);
        _lodGroups.Clear();

        using var reader = new BinaryReader(new MemoryStream(data));
        // Skip scene name string as it is not important at this stage
        var s = reader.ReadInt32();
        reader.BaseStream.Position += s;
        
        // Get the number of LODs
        var numLodGroups = reader.ReadInt32();
        Debug.Assert(numLodGroups > 0);

        for (int i = 0; i < numLodGroups; ++i)
        {
            s = reader.ReadInt32();
            string lodGroupName;
            if (s > 0)
            {
                var nameBytes = reader.ReadBytes(s);
                lodGroupName = Encoding.UTF8.GetString(nameBytes);
            }
            else
            {
                lodGroupName = $"lod_{ ContentHelper.GetRandomString() }";
            }
            
            // Get number of meshes in this LOD group
            var numMeshes = reader.ReadInt32();
            Debug.Assert(numMeshes > 0);
            List<MeshLOD> lods = ReadMeshLODs(numMeshes, reader);

            var lodGroup = new LODGroup() { Name = lodGroupName };
            lods.ForEach(l => lodGroup.LODs.Add(l));
            
            _lodGroups.Add(lodGroup);
        }
    }

    private static List<MeshLOD> ReadMeshLODs(int numMeshes, BinaryReader reader)
    {
        var lodIds = new List<int>();
        var lodList = new List<MeshLOD>();

        for (int i = 0; i < numMeshes; ++i)
        {
            ReadMeshes(reader, lodIds, lodList);
        }

        return lodList;
    }

    private static void ReadMeshes(BinaryReader reader, List<int> lodIds, List<MeshLOD> lodList)
    {
        // Get mesh's name
        var s = reader.ReadInt32();
        string meshName;
        if (s > 0)
        {
            var nameBytes = reader.ReadBytes(s);
            meshName = Encoding.UTF8.GetString(nameBytes);
        }
        else
        {
            meshName = $"mesh_{ ContentHelper.GetRandomString() }";
        }

        var mesh = new Mesh();
        
        var lodId = reader.ReadInt32();
        mesh.VertexSize = reader.ReadInt32();
        mesh.VertexCount = reader.ReadInt32();
        mesh.IndexSize = reader.ReadInt32();
        mesh.IndexCount = reader.ReadInt32();

        var lodThreshold = reader.ReadSingle();
        var vertexBufferSize = mesh.VertexSize * mesh.VertexCount;
        var indexBufferSize = mesh.IndexSize * mesh.IndexCount;

        mesh.Vertices = reader.ReadBytes(vertexBufferSize);
        mesh.Indices = reader.ReadBytes(indexBufferSize);

        MeshLOD lod;
        if (ID.IsValid(lodId) && lodIds.Contains(lodId))
        {
            lod = lodList[lodIds.IndexOf(lodId)];
            Debug.Assert(lod != null);
        }
        else
        {
            lodIds.Add(lodId);
            lod = new MeshLOD { Name = meshName, LodThreshold = lodThreshold };
            lodList.Add(lod);
        }
        
        lod.Meshes.Add(mesh);
    }

    public LODGroup? GetLODGroup(int lodGroup = 0)
    {
        Debug.Assert(lodGroup >= 0 && lodGroup < _lodGroups.Count);
        return _lodGroups.Any() ? _lodGroups[lodGroup] : null;
    }

    public override IEnumerable<string> Save(string file)
    {
        Debug.Assert(_lodGroups.Any());
        var savedFiles = new List<string>();
        if (!_lodGroups.Any()) { return savedFiles; }

        var path = Path.GetDirectoryName(file) + Path.DirectorySeparatorChar;
        var fileName = Path.GetFileNameWithoutExtension(file);

        try
        {
            foreach (var lodGroup in _lodGroups)
            {
                Debug.Assert(lodGroup.LODs.Any());
                
                // Use the name of the most detailed LOD for file name
                var meshFileName = ContentHelper.SanitizeFileName(path + fileName + "_" + lodGroup.LODs[0].Name + AssetFileExtension);
                
                // NOTE: I have to make a different id for each new asset file
                Guid = Guid.NewGuid();
                byte[] data = null;
                using (var writer = new BinaryWriter(new MemoryStream()))
                {
                    writer.Write(lodGroup.Name);
                    writer.Write(lodGroup.LODs.Count);
                    var hashes = new List<byte>();
                    foreach (var lod in lodGroup.LODs)
                    {
                        LODToBinary(lod, writer, out var hash);
                        hashes.AddRange(hash);
                    }

                    Hash = ContentHelper.ComputeHash(hashes.ToArray());
                    data = (writer.BaseStream as MemoryStream).ToArray();
                    Icon = GenerateIcon(lodGroup.LODs[0]);
                }
                
                Debug.Assert(data?.Length > 0);

                using (var writer = new BinaryWriter(File.Open(meshFileName, FileMode.Create, FileAccess.Write)))
                {
                    WriteAssetFileHeader(writer);
                    ImportSettings.ToBinary(writer);
                    writer.Write(data.Length);
                    writer.Write(data);
                }
                
                savedFiles.Add(meshFileName);
            }
        }
        catch (Exception e)
        {
            Debug.Write(e.Message);
            Logger.Log(MessageType.Error, $"Failed to save geometry to {file}");
        }

        return savedFiles;
    }

    private byte[] GenerateIcon(MeshLOD lod)
    {
        var width = 90 * 4;

        BitmapSource bmp = null;

        Application.Current.Dispatcher.Invoke(() =>
        {
            bmp = GeometryView.RenderToBitmap(new MeshRenderer(lod, null), width, width);
            bmp = new TransformedBitmap(bmp, new ScaleTransform(0.25, 0.25, 0.5, 0.5));
        });

        using var memStream = new MemoryStream();
        memStream.SetLength(0);

        var encoder = new PngBitmapEncoder();
        encoder.Frames.Add(BitmapFrame.Create(bmp));
        encoder.Save(memStream);

        return memStream.ToArray();
    }

    private void LODToBinary(MeshLOD lod, BinaryWriter writer, out byte[] hash)
    {
        writer.Write(lod.Name);
        writer.Write(lod.LodThreshold);
        writer.Write(lod.Meshes.Count);

        var meshDataBegin = writer.BaseStream.Position;
        
        foreach (var mesh in lod.Meshes)
        {
            writer.Write(mesh.VertexSize);
            writer.Write(mesh.VertexCount);
            writer.Write(mesh.IndexSize);
            writer.Write(mesh.Vertices);
            writer.Write(mesh.Indices);
        }

        var meshDataSize = writer.BaseStream.Position - meshDataBegin;
        Debug.Assert(meshDataSize > 0);

        var buffer = (writer.BaseStream as MemoryStream).ToArray();
        hash = ContentHelper.ComputeHash(buffer, (int)meshDataBegin, (int)meshDataSize);
    }
}