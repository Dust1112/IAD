#include "TestWindow.hpp"

#include <thread>

#include "../Engine/Platform/PlatformTypes.hpp"

namespace
{
    LRESULT WinProc(HWND hwnd, UINT msg, WPARAM wparam, LPARAM lparam)
    {
        bool isAllClosed{ true };
        switch (msg)
        {
        case WM_DESTROY:
            for (u32 i{ 0 }; i < _countof(_windows); ++i)
            {
                if (!_windows[i].IsClosed())
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
}

bool window_test::initialize()
{
    iad::platform::WindowInitInfo info[]
    {
        { &WinProc, nullptr, L"Test Window 1", 100, 100, 400, 800 },
        { &WinProc, nullptr, L"Test Window 2", 150, 150, 800, 400 },
        { &WinProc, nullptr, L"Test Window 3", 200, 200, 400, 800 },
        { &WinProc, nullptr, L"Test Window 4", 250, 250, 800, 400 }
    };
    static_assert(_countof(info) == _countof(_windows));
    
    for (u32 i{ 0 }; i < _countof(_windows); ++i)
    {
        _windows[i] = iad::platform::CreateAndInitWindow(&info[i]);
    }

    return true;
}

void window_test::run()
{
    std::this_thread::sleep_for(std::chrono::milliseconds(10));
}

void window_test::shutdown()
{
    for (u32 i{ 0 }; i < _countof(_windows); ++i)
    {
        iad::platform::RemoveWindow(_windows[i].GetId());
    }
}
