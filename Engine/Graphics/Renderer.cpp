﻿#include "Renderer.hpp"
#include "GraphicsPlatformInterface.hpp"
#include "Direct3D12/D3D12Interface.hpp"

namespace iad::graphics
{
    namespace 
    {
        platform_interface gfx{};

        bool set_platform_interface(graphics_platform platform)
        {
            switch (platform)
            {
            case graphics_platform::direct_3d12:
                iad::graphics::d3d12::get_platform_interface(gfx);
                break;
            default:
                break;
            }
        }
    }

    bool initialize(graphics_platform platform)
    {
        return set_platform_interface(platform);   
    }

    void shutdown()
    {
        gfx.shutdown();
    }
}
