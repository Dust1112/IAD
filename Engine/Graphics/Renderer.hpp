#pragma once

#include "GraphicsPlatform.hpp"
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

    bool initialize(graphics_platform platform);
    void shutdown();
    void render();
}
