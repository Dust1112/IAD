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

#ifndef DISABLE_COPY
#define DISABLE_COPY(T)                                     \
    explicit T(const T&) = delete;                          \
    T& operator=(const T&) = delete;
#endif

#ifndef DISABLE_MOVE
#define DISABLE_MOVE(T)                                     \
    explicit T(T&&) = delete;                               \
    T& operator=(T&&) = delete;
#endif

#ifndef DISABLE_COPY_AND_MOVE
#define DISABLE_COPY_AND_MOVE(T) DISABLE_COPY(T) DISABLE_MOVE(T)
#endif

// Common Headers
#include "../Utilities/Math.hpp"
#include "../Utilities/Utilities.h"
#include "../Utilities/MathTypes.hpp"
#include "PrimitiveTypes.hpp"
#include "Id.hpp"
#include "Macros.hpp"