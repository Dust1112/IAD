#pragma once

#include "../../ToolsCommon.hpp"
#include <string>
#include "Mesh.hpp"

namespace iad::tools
{
	struct LodGroup
	{
		std::string name;
		utl::vector<Mesh> meshes;
	};
}