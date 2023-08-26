#pragma once

#include "../../ToolsCommon.hpp"
#include "GeometryImportSettings.hpp"

namespace iad::tools
{
	struct SceneData 
	{
		u8* buffer;
		u32 buffer_size;
		geometry_import_settings settings;
	};
}