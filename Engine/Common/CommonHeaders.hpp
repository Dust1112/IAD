#pragma once

#pragma warning(disable: 4530) // Disable exception warning

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
#include "../Utilities/Math.hpp"
#include "../Utilities/Utilities.h"
#include "../Utilities/MathTypes.hpp"
#include "PrimitiveTypes.hpp"
#include "Id.hpp"
#include "Macros.hpp"