#pragma once

#include "../../ToolsCommon.hpp"
#include "Vertex.hpp"

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
	};
}