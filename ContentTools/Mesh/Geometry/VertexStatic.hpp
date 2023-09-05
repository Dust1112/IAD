#pragma once

#include "../../ToolsCommon.hpp"

namespace iad::tools
{
	namespace packed_vertex
	{
		struct VertexStatic
		{
			math::v3 position;
			u8 reverted[3];
			// bit 0; tangent handedness * (tangent.z sign)
			// bit 1; normal.z sign (0 means -1, 1 means +1)
			u8 t_sign;
			u16 normal[2];
			u16 tangent[2];
			math::v2 uv;
		};
	}
}