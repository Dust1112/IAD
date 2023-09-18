using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using IADEditor.Content.Enums;
using IADEditor.Utilities;

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
}