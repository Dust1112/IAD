#pragma once

namespace iad::tools
{
	struct GeometryImportSettings
	{
		f32 smoothing_angle;
		u8 calculate_normals;
		u8 calculate_tangents;
		u8 reverse_handedness;
		u8 import_embedded_textures;
		u8 import_animations;
	};
}