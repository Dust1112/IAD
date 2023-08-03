#pragma once

#include "Platform.hpp"
#include "Window.hpp"

namespace iad::platform
{
    struct WindowInitInfo;
    
    Window CreateAndInitWindow(const WindowInitInfo* const init_info = nullptr);
    void RemoveWindow(window_id id);
}
