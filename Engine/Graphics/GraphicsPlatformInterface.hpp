#pragma once

#include "../Common/CommonHeaders.hpp"
#include "Renderer.hpp"

namespace iad::graphics
{
    struct platform_interface
    {
        bool (*initialize)(void);
        void (*shutdown)(void);
    };
}
