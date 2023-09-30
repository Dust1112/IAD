#include "TestRenderer.hpp"

#include <thread>

#include "../../Engine/Platform/PlatformTypes.hpp"
#include "../../Engine/Platform/Platform.hpp"
#include "../../Engine/Graphics/Renderer.hpp"

iad::graphics::RenderSurface _surfaces[4];

void create_render_surface(iad::graphics::RenderSurface& surface, iad::platform::WindowInitInfo info)
{
    surface.window = iad::platform::CreateAndInitWindow(&info);
}

void destroy_render_surface(iad::graphics::RenderSurface& surface)
{
    iad::platform::RemoveWindow(surface.window.GetId());
}

LRESULT WinProc(HWND hwnd, UINT msg, WPARAM wparam, LPARAM lparam)
{
    bool isAllClosed{ true };
    switch (msg)
    {
    case WM_DESTROY:
        for (u32 i{ 0 }; i < _countof(_surfaces); ++i)
        {
            if (!_surfaces[i].window.IsClosed())
            {
                isAllClosed = false;
            }
        }

        if (isAllClosed)
        {
            PostQuitMessage(0);
            return 0;
        }

        break;
        
    case WM_SYSCHAR:
        if (wparam == VK_RETURN && (HIWORD(lparam) & KF_ALTDOWN))
        {
            iad::platform::Window window{ iad::platform::window_id{ (iad::id::id_type)GetWindowLongPtr(hwnd, GWLP_USERDATA) } };
            window.SetFullscreen(!window.IsFullscreen());
            return 0;
        }
        break;
    }

    return DefWindowProc(hwnd, msg, wparam, lparam);
}

bool engine_test::initialize()
{
    bool result{ iad::graphics::initialize(iad::graphics::graphics_platform::direct_3d12) };
    if (!result) { return result; }
    
    iad::platform::WindowInitInfo info[]
    {
        { &WinProc, nullptr, L"Test Window 1", 100, 100, 400, 800 },
        { &WinProc, nullptr, L"Test Window 2", 150, 150, 800, 400 },
        { &WinProc, nullptr, L"Test Window 3", 200, 200, 400, 800 },
        { &WinProc, nullptr, L"Test Window 4", 250, 250, 800, 400 }
    };
    static_assert(_countof(info) == _countof(_surfaces));
    
    for (u32 i{ 0 }; i < _countof(_surfaces); ++i)
    {
        create_render_surface(_surfaces[i], info[i]);
    }

    return result;
}

void engine_test::run()
{
    std::this_thread::sleep_for(std::chrono::milliseconds(10));
}

void engine_test::shutdown()
{
    for (u32 i{ 0 }; i < _countof(_surfaces); ++i)
    {
        destroy_render_surface(_surfaces[i]);
    }

    iad::graphics::shutdown();
}


