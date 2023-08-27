#pragma once

#include "../Common/CommonHeaders.hpp"
#include "MathTypes.hpp"

namespace iad::math
{
	template<typename T>
	constexpr T clamp(T value, T min, T max)
	{
		return (value < min) ? min : (value > max) ? max : value;
	}
}