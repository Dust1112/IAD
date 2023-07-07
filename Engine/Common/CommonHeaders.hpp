#pragma once

#pragma warning(disable: 4530) // Disable exception worning

// C/C++
#include <cstdint>
#include <assert.h>
#include <typeinfo>
#include <memory>
#include <unordered_map>

#if defined(_WIN64)
#include <DirectXMath.h>
#endif

// Common Headers
#include "PrimitiveTypes.hpp"
#include "../Utilities/Utilities.h"
#include "../Utilities/MathTypes.hpp"