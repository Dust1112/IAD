#pragma once

#include "../../ToolsCommon.hpp"
#include "GeometryImportSettings.hpp"
#include "Scene.hpp"

namespace iad::tools
{
	struct SceneData 
	{
		u8* buffer;
		u32 buffer_size;
		GeometryImportSettings settings;
	};

	void ProcessScene(Scene& scene, const GeometryImportSettings settings);
	void PackData(const Scene& scene, SceneData& data);
}