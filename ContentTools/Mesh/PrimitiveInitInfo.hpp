#pragma once

#include "PrimitiveMeshType.hpp"
#include "../../Engine/Utilities/MathTypes.hpp"

namespace iad::tools
{
	struct PrimitiveInitInfo
	{
		PrimitiveMeshType type;
		u32 segments[3]{ 1, 1, 1 };
		math::v3 size{ 1, 1, 1 };
		u32 lod{ 0 };
	};
}
