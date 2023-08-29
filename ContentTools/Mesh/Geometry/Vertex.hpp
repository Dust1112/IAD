#pragma once

#include "../Utilities/Math.hpp"

namespace iad::tools
{
	struct Vertex
	{
		math::v4 tangent{};
		math::v3 position{};
		math::v3 normal{};
		math::v2 uv{};
	};
}