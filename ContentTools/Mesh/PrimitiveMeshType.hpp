#pragma once

#include <PrimitiveTypes.hpp>

namespace iad::tools
{
    enum PrimitiveMeshType : u32
    {
        plane,
        cube,
        uv_sphere,
        ico_sphere,
        cylinder,
        capsule,

        count
    };
}