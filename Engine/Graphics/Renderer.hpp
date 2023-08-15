#pragma once

#include "../Common/CommonHeaders.hpp"
#include "../Platform/Window.hpp"

namespace iad::graphics
{
    class Surface
    {
        
    };

    struct RenderSurface
    {
        platform::Window window{};
        Surface surface;
    };
}
