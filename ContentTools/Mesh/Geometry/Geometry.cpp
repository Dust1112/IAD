#include "Geometry.hpp"

namespace iad::tools
{
	namespace
	{
		using namespace math;
		using namespace DirectX;

		void RecalculateNormals(Mesh& mesh)
		{
			const u32 num_indices{ (u32)mesh.raw_indices.size() };
			mesh.normals.reserve(num_indices);

			for (u32 i{ 0 }; i < num_indices; ++i)
			{
				const u32 i0{ mesh.raw_indices[i] };
				const u32 i1{ mesh.raw_indices[++i] };
				const u32 i2{ mesh.raw_indices[++i] };

				XMVECTOR v0{ XMLoadFloat3(&mesh.positions[i0]) };
				XMVECTOR v1{ XMLoadFloat3(&mesh.positions[i1]) };
				XMVECTOR v2{ XMLoadFloat3(&mesh.positions[i2]) };

				XMVECTOR e0{ v1 - v0 };
				XMVECTOR e1{ v2 - v0 };
				XMVECTOR n{ XMVector3Normalize(XMVector3Cross(e0, e1)) };

				XMStoreFloat3(&mesh.normals[i], n);
				mesh.normals[i - 1] = mesh.normals[i];
				mesh.normals[i - 2] = mesh.normals[i];
			}
		}

		void ProcessNormals(Mesh& mesh, f32 smoothingAngle)
		{
			const f32 cos_alpha{ XMScalarCos(pi - smoothingAngle * pi / 180.f) };
			const bool is_hard_edge{ XMScalarNearEqual(smoothingAngle, 180.f, epsilon) };
			const bool is_soft_edge{ XMScalarNearEqual(smoothingAngle, 0.f, epsilon) };
			const u32 num_indices{ (u32)mesh.raw_indices.size() };
			const u32 num_vertices{ (u32)mesh.positions.size() };

			assert(num_indices && num_vertices);

			mesh.indices.resize(num_indices);

			utl::vector<utl::vector<u32>> index_ref(num_vertices);
			for (u32 i{ 0 }; i < num_indices; ++i)
			{
				index_ref[mesh.raw_indices[i]].emplace_back(i);
			}

			for (u32 i{ 0 }; i < num_vertices; ++i)
			{
				auto& refs{ index_ref[i] };
				u32 num_refs{ (u32)refs.size() };
				for (u32 j{ 0 }; j < num_refs; ++j)
				{
					mesh.indices[refs[j]] = (u32)mesh.vertices.size();
					Vertex& vert{ mesh.vertices.emplace_back() };
					vert.position = mesh.positions[mesh.raw_indices[refs[j]]];

					XMVECTOR n1{ XMLoadFloat3(&mesh.normals[refs[j]]) };

					if (!is_hard_edge)
					{
						for (u32 k{ j + 1 }; k < num_refs; ++k)
						{
							f32 cos_theta{ 0.f }; // This value represents the cosine of the andgle between normals.
							XMVECTOR n2{ XMLoadFloat3(&mesh.normals[refs[k]]) };

							if (!is_soft_edge)
							{
								XMStoreFloat(&cos_theta, XMVector3Dot(n1, n2) * XMVector3ReciprocalLength(n1));
							}

							if (is_soft_edge || cos_theta >= cos_alpha)
							{
								n1 += n2;

								mesh.indices[refs[k]] = mesh.indices[refs[j]];
								refs.erase(refs.begin() + k);
								--num_refs;
								--k;
							}
						}
					}
					XMStoreFloat3(&vert.normal, XMVector3Normalize(n1));
				}
			}
		}

		void ProcessUvs(Mesh& mesh)
		{
			utl::vector<Vertex> old_vertices;
			old_vertices.swap(mesh.vertices);
			utl::vector<u32> old_indices(mesh.indices.size());
			old_indices.swap(mesh.indices);

			const u32 num_vertices{ (u32)old_vertices.size() };
			const u32 num_indices{ (u32)old_indices.size() };
			assert(num_vertices && num_indices);

			utl::vector<utl::vector<u32>> index_ref(num_vertices);
			for (u32 i{ 0 }; i < num_indices; ++i)
			{
				index_ref[old_indices[i]].emplace_back(i);
			}

			for (u32 i{ 0 }; i < num_indices; ++i)
			{
				auto& refs{ index_ref[i] };
				u32 num_refs{ (u32)refs.size() };
				for (u32 j{ 0 }; j < num_refs; ++j)
				{
					mesh.indices[refs[j]] = (u32)mesh.vertices.size();
					Vertex& vert{ old_vertices[old_indices[refs[j]]] };
					vert.uv = mesh.uv_sets[0][refs[j]];
					mesh.vertices.emplace_back(vert);

					for (u32 k{ j + 1 }; k < num_refs; ++k)
					{
						math::v2& uv1{ mesh.uv_sets[0][refs[k]] };
						if (XMScalarNearEqual(vert.uv.x, uv1.x, epsilon) &&
							XMScalarNearEqual(vert.uv.y, uv1.y, epsilon))
						{
							mesh.indices[refs[k]] = mesh.indices[refs[j]];
							refs.erase(refs.begin() + k);
							--num_refs;
							--k;
						}
					}
				}
			}
		}

		void PackVerticesStatic(Mesh& mesh)
		{
			const u32 num_vertices{ (u32)mesh.vertices.size() };
			assert(num_vertices);

			mesh.packed_vertices_static.reserve(num_vertices);

			for (u32 i{ 0 }; i < num_vertices; ++i)
			{
				Vertex& vert{ mesh.vertices[i] };
				const u8 signs{ (u32)((vert.normal.z > 0.f) << 1) };
				const u16 normal_x{ (u16)PackFloat<16>(vert.normal.x, -1.f, 1.f) };
				const u16 normal_y{ (u16)PackFloat<16>(vert.normal.y, -1.f, 1.f) };

				// TODO: pack tangents in sign and in x/y components

				mesh.packed_vertices_static.emplace_back(
					packed_vertex::VertexStatic
					{
						vert.position,
						{ 0, 0, 0 },
						signs,
						{ normal_x, normal_y },
						{},
						vert.uv
					}
				);
			}
		}

		void ProcessVertices(Mesh& mesh, const GeometryImportSettings settings)
		{
			assert((mesh.raw_indices.size() % 3) == 0);

			if (settings.calculate_normals || mesh.normals.empty())
			{
				RecalculateNormals(mesh);
			}

			ProcessNormals(mesh, settings.smoothing_angle);

			if (!mesh.uv_sets.empty())
			{
				ProcessUvs(mesh);
			}

			PackVerticesStatic(mesh);
		}
	}
}

void iad::tools::ProcessScene(Scene& scene, const GeometryImportSettings settings)
{
	for (auto& lod : scene.lod_groups)
	{
		for (auto& mesh : lod.meshes)
		{
			ProcessVertices(mesh, settings);
		}
	}
}

void iad::tools::PackData(const Scene& scene, SceneData& data)
{
}
