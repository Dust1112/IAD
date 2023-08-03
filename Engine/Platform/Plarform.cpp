#include "Platform.hpp"
#include "PlatformTypes.hpp"

namespace iad::platform
{
#ifdef _WIN64
    namespace
    {
        struct WindowInfo
        {
            HWND hwnd{ nullptr };
            RECT client_area{ 0, 0, 1920, 1080 };
            RECT fullscreen_area{};
            POINT top_left{ 0, 0 };
            DWORD style{ WS_VISIBLE };
            bool is_fullscreen{ false };
            bool is_closed{ false };
        };
        
        LRESULT CALLBACK InternalWindowProc(HWND hwnd, UINT msg, WPARAM wparam, LPARAM lparam)
        {
            LONG_PTR long_ptr{ GetWindowLongPtr(hwnd, 0) };
            return long_ptr
                ? ((window_proc)long_ptr)(hwnd, msg, wparam, lparam)
                : DefWindowProc(hwnd, msg, wparam, lparam);
        }
    }

    Window CreateAndInitWindow(const WindowInitInfo* const init_info)
    {
        window_proc callback{ init_info ? init_info->callback : nullptr };
        window_handle parent{ init_info ? init_info->parent : nullptr };

        // Setup windows class
        WNDCLASSEX wc;
        ZeroMemory(&wc, sizeof(wc));
        wc.cbSize = sizeof(WNDCLASSEX);
        wc.style = CS_HREDRAW | CS_VREDRAW;
        wc.lpfnWndProc = InternalWindowProc;
        wc.cbClsExtra = 0;
        wc.cbWndExtra = callback ? sizeof(callback) : 0;
        wc.hInstance = 0;
        wc.hIcon = LoadIcon(NULL, IDI_APPLICATION);
        wc.hCursor = LoadIcon(NULL, IDC_ARROW);
        wc.hbrBackground = CreateSolidBrush(RGB(26, 48, 76));
        wc.lpszMenuName = NULL;
        wc.lpszClassName = L"IADWindow";
        HICON hIconSm = LoadIcon(NULL, IDI_APPLICATION);

        // Register the window class
        RegisterClassEx(&wc);
        
        WindowInfo info{};
        RECT rc{ info.client_area };

        // Adjust the window size for the correct device size
        AdjustWindowRect(&rc, info.style, FALSE);
        
        const wchar_t* caption{ (init_info && init_info->caption) ? init_info->caption : L"IAD Game" };
        const s32 left{ (init_info && init_info->left) ? init_info->left : info.client_area.left };
        const s32 top{ (init_info && init_info->top) ? init_info->top : info.client_area.top };
        const s32 width{ (init_info && init_info->width) ? init_info->width : rc.right - rc.left };
        const s32 height{ (init_info && init_info->height) ? init_info->height : rc.bottom - rc.top };

        info.style |= parent ? WS_CHILD : WS_OVERLAPPEDWINDOW;
        // Create an instance of the window class
        info.hwnd = CreateWindowEx(
            0,
            wc.lpszClassName,
            caption,
            info.style,
            left, top,
            width, height,
            parent,
            NULL,
            NULL,
            NULL
        );

        if (info.hwnd)
        {
            
        }
        return{};
    }
#elif
#error "Must implement at least one platform."
#endif
}
