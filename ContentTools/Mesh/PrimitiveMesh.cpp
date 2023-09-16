#include "PrimitiveMesh.hpp"

#include "PrimitiveInitInfo.hpp"

#include "Geometry/Geometry.hpp"
#include "Geometry/Axis.hpp"

namespace iad::tools
{
    namespace
    {
        using primitive_mesh_creator = void(*)(Scene&, const PrimitiveInitInfo& info);

        void CreatePlane(Scene& scene, const PrimitiveInitInfo& info);
        void CreateCube(Scene& scene, const PrimitiveInitInfo& info);
        void CreateUvSphere(Scene& scene, const PrimitiveInitInfo& info);
        void CreateIcoSphere(Scene& scene, const PrimitiveInitInfo& info);
        void CreateCylinder(Scene& scene, const PrimitiveInitInfo& info);
        void CreateCapsule(Scene& scene, const PrimitiveInitInfo& info);

        primitive_mesh_creator creators[]
        {
            CreatePlane,
            CreateCube,
            CreateUvSphere,
            CreateIcoSphere,
            CreateCylinder,
            CreateCapsule,
        };

        static_assert(_countof(creators) == PrimitiveMeshType::count);
        
        Mesh CreatePlane(const PrimitiveInitInfo& info,
            u32 horizontalIndex = Axis::x, u32 verticalIndex = Axis::z, bool flipWinding = false,
            math::v3 offset = { -0.5f, 0.f, -0.5f }, math::v2 uRange = { 0.f, 1.f },
            math::v2 vRange = { 0.f, 1.f })
        {
            assert(horizontalIndex < 3 && verticalIndex < 3);
            assert(horizontalIndex != verticalIndex);

            const u32 horizontal_count{ math::clamp(info.segments[horizontalIndex], 1u, 10u) };
            const u32 vertical_count{ math::clamp(info.segments[verticalIndex], 1u, 10u) };
            const f32 horizontal_step{ 1.f / horizontal_count };
            const f32 vertical_step{ 1.f / vertical_count };
            const f32 u_step{ (uRange.y - uRange.x) / horizontal_count };
            const f32 v_step{ (vRange.y - vRange.x) / vertical_count };

            Mesh mesh{};
            utl::vector<math::v2> uvs;

            for (u32 j{ 0 }; j <= vertical_count; ++j)
            {
                for (u32 i{ 0 }; i <= horizontal_count; ++i)
                {
                    math::v3 position{ offset };
                    f32* const as_array{ &position.x };
                    as_array[horizontalIndex] += i * horizontal_step;
                    as_array[verticalIndex] += j * vertical_step;
                    mesh.positions.emplace_back(
                        position.x * info.size.x,
                        position.y * info.size.y,
                        position.z * info.size.z
                    );

                    math::v2 uv{ uRange.x, 1.f - vRange.x };
                    uv.x += i * u_step;
                    uv.y -= j * v_step;
                    uvs.emplace_back(uv);
                }
            }

            assert(mesh.positions.size() == (((u64)horizontal_count + 1) * ((u64)vertical_count + 1)));

            const u32 row_length{ horizontal_count + 1 }; // Number of vertices in a row
            for (u32 j{ 0 }; j < vertical_count; ++j)
            {
                u32 k{ 0 };
                for (u32 i{ k }; i < horizontal_count; ++i)
                {
                    const u32 index[4]
                    {
                        i + j * row_length,
                        i + (j + 1) * row_length,
                        (i + 1) + j * row_length,
                        (i + 1) + (j + 1) * row_length,
                    };

                    mesh.raw_indices.emplace_back(index[0]);
                    mesh.raw_indices.emplace_back(index[flipWinding ? 2 : 1]);
                    mesh.raw_indices.emplace_back(index[flipWinding ? 1 : 2]);

                    mesh.raw_indices.emplace_back(index[2]);
                    mesh.raw_indices.emplace_back(index[flipWinding ? 3 : 1]);
                    mesh.raw_indices.emplace_back(index[flipWinding ? 1 : 3]);
                }
                ++k;
            }

            const u32 num_indices{ 3 * 2 * horizontal_count * vertical_count };
            assert(mesh.raw_indices.size() == num_indices);

            mesh.uv_sets.resize(1);
            
            for (u32 i{ 0 }; i < num_indices; ++i)
            {
                mesh.uv_sets[0].emplace_back(uvs[mesh.raw_indices[i]]);
            }

            return mesh;
        }

        Mesh CreateUvSphere(const PrimitiveInitInfo& info)
        {
            const u32 phi_count{ math::clamp(info.segments[Axis::x], 3u, 256u) };
            const u32 theta_count{ math::clamp(info.segments[Axis::y], 2u, 256u) };
            const f32 theta_step{ math::pi / theta_count };
            const f32 phi_step{ math::two_pi / phi_count };
            const u32 num_vertices{ 2 + phi_count * (theta_count - 1) };
            const u32 num_indices{ 2 * 3 * phi_count + 2 * 3 * phi_count * (theta_count - 2) };

            Mesh mesh{};
            mesh.name = "uv_sphere";
            mesh.positions.resize(num_vertices);

            // Add the top vertex
            u32 c{ 0 };
            mesh.positions[c++] = { 0.f, info.size.y, 0.f };

            for (u32 j{ 1 }; j <= (theta_count - 1); ++j)
            {
                const f32 theta{ j * theta_step };
                for (u32 i{ 0 }; i < phi_count; ++i)
                {
                    const f32 phi{ i * phi_step };
                    mesh.positions[c++] =
                    {
                        info.size.x * DirectX::XMScalarSin(theta) * DirectX::XMScalarCos(phi),
                        info.size.y * DirectX::XMScalarCos(theta),
                        -info.size.z * DirectX::XMScalarSin(theta) * DirectX::XMScalarSin(phi),
                    };
                }
            }

            // Add the bottom vertex
            mesh.positions[c++] = { 0.f, -info.size.y, 0.f };
            assert(c == num_vertices);

            c = 0;
            mesh.raw_indices.resize(num_indices);

            // Indices for the top cap, connecting the north pole to the first ring
            for (u32 i{ 0 }; i < phi_count - 1; ++i)
            {
                mesh.raw_indices[c++] = 0;
                mesh.raw_indices[c++] = i + 1;
                mesh.raw_indices[c++] = i + 2;
            }

            mesh.raw_indices[c++] = 0;
            mesh.raw_indices[c++] = phi_count;
            mesh.raw_indices[c++] = 1;

            // Indices for the section between the top and bottom rings
            for (u32 j{ 0 }; j < (theta_count - 2); ++j)
            {
                for (u32 i{ 0 }; i < (phi_count - 1); ++i)
                {
                    const u32 index[4]
                    {
                        1 + i + j * phi_count,
                        1 + i + (j + 1) * phi_count,
                        1 + (i + 1) + (j + 1) * phi_count,
                        1 + (i + 1) + j * phi_count
                    };

                    mesh.raw_indices[c++] = index[0];
                    mesh.raw_indices[c++] = index[1];
                    mesh.raw_indices[c++] = index[2];

                    mesh.raw_indices[c++] = index[0];
                    mesh.raw_indices[c++] = index[2];
                    mesh.raw_indices[c++] = index[3];
                }

                const u32 index[4]
                {
                    phi_count + j * phi_count,
                    phi_count + (j + 1) * phi_count,
                    1 + (j + 1) * phi_count,
                    1 + j * phi_count
                };

                mesh.raw_indices[c++] = index[0];
                mesh.raw_indices[c++] = index[1];
                mesh.raw_indices[c++] = index[2];

                mesh.raw_indices[c++] = index[0];
                mesh.raw_indices[c++] = index[2];
                mesh.raw_indices[c++] = index[3];
            }

            // Indices for the bottom cap, connecting the south pole to the last ring
            const u32 south_pole_index{ (u32)mesh.positions.size() - 1 };
            for (u32 i{ 0 }; i < (phi_count - 1); ++i)
            {
                mesh.raw_indices[c++] = south_pole_index;
                mesh.raw_indices[c++] = south_pole_index - phi_count + i + 1;
                mesh.raw_indices[c++] = south_pole_index - phi_count + i;
            }

            mesh.raw_indices[c++] = south_pole_index;
            mesh.raw_indices[c++] = south_pole_index - phi_count;
            mesh.raw_indices[c++] = south_pole_index - 1;

            mesh.uv_sets.resize(1);
            mesh.uv_sets[0].resize(mesh.raw_indices.size());

            return mesh;
        }

        void CreatePlane(Scene& scene, const PrimitiveInitInfo& info)
        {
            LodGroup lod{};
            lod.name = "plane";
            lod.meshes.emplace_back(CreatePlane(info));
            scene.lod_groups.emplace_back(lod);
        }

        void CreateCube(Scene& scene, const PrimitiveInitInfo& info)
        {
        }

        void CreateUvSphere(Scene& scene, const PrimitiveInitInfo& info)
        {
            LodGroup lod{};
            lod.name = "uv_sphere";
            lod.meshes.emplace_back(CreateUvSphere(info));
            scene.lod_groups.emplace_back(lod);
        }

        void CreateIcoSphere(Scene& scene, const PrimitiveInitInfo& info)
        {
        }

        void CreateCylinder(Scene& scene, const PrimitiveInitInfo& info)
        {
        }

        void CreateCapsule(Scene& scene, const PrimitiveInitInfo& info)
        {
        }
    }

    EDITOR_INTERFACE void CreatePrimitiveMesh(SceneData* data, PrimitiveInitInfo* info)
    {
        assert(data && info);
        assert(info->type < PrimitiveMeshType::count);

        Scene scene{};
        creators[info->type](scene, *info);

        data->settings.calculate_normals = 1;
        ProcessScene(scene, data->settings);
        PackData(scene, *data);
    }
}
