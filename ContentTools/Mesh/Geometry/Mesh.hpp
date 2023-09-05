#pragma once

#include "Vertex.hpp"
#include "VertexStatic.hpp"

namespace iad::tools
{
	struct Mesh
	{
		// Initial data
		utl::vector<math::v3> positions;
		utl::vector<math::v3> normals;
		utl::vector<math::v4> tangents;
		utl::vector<utl::vector<math::v2>> uv_sets;

		utl::vector<u32> raw_indices;

		// Intermediate data
		utl::vector<Vertex> vertices;
		utl::vector<u32> indices;

		// Output data
		std::string name;
		utl::vector<packed_vertex::VertexStatic> packed_vertices_static;
		f32 lod_threshold{ -1.f };
		u32 lod_id{ u32_invalid_id };
	};
}