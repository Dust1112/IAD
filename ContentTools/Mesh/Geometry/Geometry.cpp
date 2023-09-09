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

		u64 GetMeshSize(const Mesh& mesh)
		{
			const u64 num_vertices{ mesh.vertices.size() };
			const u64 vertex_buffer_size{ sizeof(packed_vertex::VertexStatic) * num_vertices };
			const u64 index_size{ (num_vertices < (1 << 16)) ? sizeof(u16) : sizeof(u32) };
			const u64 index_buffer_size{ index_size * mesh.indices.size() };

			constexpr u64 su32{ sizeof(u32) };
			const u64 size
			{
				su32 + mesh.name.size() +				// Mesh name length and room for mesh name string
				su32 +									// Mesh ID	
				su32 +									// Vertex size
				su32 +									// Number of vertices
				su32 +									// Index size (16 bit or 32 bit)
				su32 +									// Number of indices
				sizeof(f32) +							// LOD threshold
				vertex_buffer_size +					// Room for vertices
				index_buffer_size						// Room for indices
			};

			return size;
		}

		u64 GetSceneSize(const Scene& scene)
		{
			constexpr u64 su32{ sizeof(u32) };
			u64 size
			{
				su32 +						// Name length
				scene.name.size() +			// Room for scene name string
				su32						// Number of LODs
			};

			for (auto& lod : scene.lod_groups)
			{
				u64 lod_size
				{
					su32 + lod.name.size() +		// LOD name length and room for LPD name string
					su32							// Number of meshes in this LOD
				};

				for (auto& mesh : lod.meshes)
				{
					lod_size += GetMeshSize(mesh);
				}
				size += lod_size;
			}

			return size;
		}

		void PackMeshData(const Mesh& mesh, u8* const buffer, u64& at)
		{
			constexpr u64 su32{ sizeof(u32) };
			u32 s{ 0 };

			// Mesh name
			s = (u32)mesh.name.size();
			memcpy(&buffer[at], &s, su32); at += su32;
			memcpy(&buffer[at], mesh.name.c_str(), s); at += s;

			// Lod ID
			s = mesh.lod_id;
			memcpy(&buffer[at], &s, su32); at += su32;

			// Vertex size
			constexpr u32 vertex_size{ sizeof(packed_vertex::VertexStatic) };
			s = vertex_size;
			memcpy(&buffer[at], &s, su32); at += su32;

			// Number of vertices
			const u32 num_vertices{ (u32)mesh.vertices.size() };
			s = num_vertices;
			memcpy(&buffer[at], &s, su32); at += su32;

			// Index size (16 bit or 32 bit)
			const u32 index_size { (num_vertices < (1 << 16)) ? sizeof(u16) : sizeof(u32) };
			s = index_size;
			memcpy(&buffer[at], &s, su32); at += su32;

			// Number of indices
			const u32 num_indices{ (u32)mesh.indices.size() };
			s = num_indices;
			memcpy(&buffer[at], &s, su32); at += su32;

			// LOD threshold
			memcpy(&buffer[at], &mesh.lod_threshold, sizeof(f32)); at += sizeof(f32);

			// Vertex data
			s = vertex_size * num_vertices;
			memcpy(&buffer[at], mesh.packed_vertices_static.data(), s); at += s;

			// Index data
			s = index_size * num_indices;
			void* data{ (void*)mesh.indices.data() };
			utl::vector<u16> indices;

			if (index_size == sizeof(u16))
			{
				indices.reserve(num_indices);
				for (u32 i{ 0 }; i < num_indices; ++i)
				{
					indices[i] = (u16)mesh.indices[i];
				}
				data = (void*)indices.data();
			}
			memcpy(&buffer[at], data, s); at += s;
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
	constexpr u64 su32{ sizeof(u32) };

	// Calculate scene size
	const u64 scene_size{ GetSceneSize(scene) };

	data.buffer_size = (u32)scene_size;
	data.buffer = (u8*)CoTaskMemAlloc(scene_size);
	assert(data.buffer);

	u8* const buffer{ data.buffer };
	u64 at{ 0 };
	u64 s{ 0 };

	// Scene name
	s = (u32)scene.name.size();
	memcpy(&buffer[at], &s, su32); at += su32;
	memcpy(&buffer[at], scene.name.c_str(), s); at += s;

	// Number of LOD's
	s = (u32)scene.lod_groups.size();
	memcpy(&buffer[at], &s, su32); at += su32;
	for (auto& lod : scene.lod_groups)
	{
		// Lod name
		s = (u32)lod.name.size();
		memcpy(&buffer[at], &s, su32); at += su32;
		memcpy(&buffer[at], lod.name.c_str(), s); at += s;

		// Number of meshes in this LOD
		s = (u32)lod.meshes.size();
		memcpy(&buffer[at], &s, su32); at += su32;
		for (auto& mesh : lod.meshes)
		{
			PackMeshData(mesh, buffer, at);
		}
	}

	assert(scene_size == at);
}
