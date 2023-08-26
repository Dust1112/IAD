#pragma once
#include <string>

#include "../../ToolsCommon.hpp"
#include "LodGroup.hpp"

namespace iad::tools
{
	struct Scene
	{
		std::string name;
		utl::vector<LodGroup> lod_groups;
	};
}