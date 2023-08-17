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

        utl::vector<WindowInfo> windows;

        // TODO: This will be replaced later
        utl::vector<u32> available_slots;
        u32 AddToWindows(WindowInfo info)
        {
            u32 id{ u32_invalid_id };
            if (available_slots.empty())
            {
                id = (u32)windows.size();
                windows.emplace_back(info);
            }
            else
            {
                id = available_slots.back();
                available_slots.pop_back();
                assert(id != u32_invalid_id);
                windows[id] = info;
            }

            return id;
        }

        void RemoveFromWindows(u32 id)
        {
            assert(id < windows.size());
            available_slots.emplace_back(id);
        }

        WindowInfo& GetFromId(window_id id)
        {
            assert(id < windows.size());
            assert(windows[id].hwnd);
            return windows[id];
        }

        WindowInfo& GetFromHandle(window_handle handle)
        {
            const window_id id{ (id::id_type)GetWindowLongPtr(handle, GWLP_USERDATA) };

            return GetFromId(id);
        }
        
        LRESULT CALLBACK InternalWindowProc(HWND hwnd, UINT msg, WPARAM wparam, LPARAM lparam)
        {
            WindowInfo* info{ nullptr };
            switch (msg)
            {
            case WM_DESTROY:
                GetFromHandle(hwnd).is_closed = true;
                break;
            case WM_EXITSIZEMOVE:
                info = &GetFromHandle(hwnd);
                break;
            case WM_SIZE:
                if (wparam == SIZE_MAXIMIZED)
                {
                    info = &GetFromHandle(hwnd);
                }
                break;
            case WM_SYSCOMMAND:
                if (wparam == SC_RESTORE)
                {
                    info = &GetFromHandle(hwnd);
                }
                break;
            default:
                break;       
            }

            if (info)
            {
                assert(info->hwnd);
                GetClientRect(info->hwnd, info->is_fullscreen ? &info->fullscreen_area : &info->client_area);
            }
            
            LONG_PTR long_ptr{ GetWindowLongPtr(hwnd, 0) };
            return long_ptr
                ? ((window_proc)long_ptr)(hwnd, msg, wparam, lparam)
                : DefWindowProc(hwnd, msg, wparam, lparam);
        }

        void ResizeWindow(const WindowInfo& info, const RECT& area)
        {
            // Adjust the window size for the correct device size
            RECT window_rect{ area };
            AdjustWindowRect(&window_rect, info.style, FALSE);

            const u32 width{ static_cast<u32>(window_rect.right) - static_cast<u32>(window_rect.left) };
            const u32 height{ static_cast<u32>(window_rect.bottom) - static_cast<u32>(window_rect.top) };

            MoveWindow(info.hwnd, info.top_left.x, info.top_left.y, width, height, true);
        }

        void ResizeWindow(window_id id, u32 width, u32 height)
        {
            WindowInfo& info{ GetFromId(id) };

            // NOTE: We also resize while in fullscreen mode to support the case when
            // when the user changes the screen resolution.
            RECT& area{ info.is_fullscreen ? info.fullscreen_area : info.client_area };
            area.bottom = area.top + height;
            area.right = area.left + width;
            
            ResizeWindow(info, area);
        }

        void SetWindowFullScreen(window_id id, bool isFullscreen)
        {
            WindowInfo& info{ GetFromId(id) };
            if (info.is_fullscreen != isFullscreen)
            {
                info.is_fullscreen = isFullscreen;

                if (isFullscreen)
                {
                    // Store the current window dimensions so they can be restored
                    // when switching out of fullscreen state
                    GetClientRect(info.hwnd, &info.client_area);
                    RECT rect;
                    GetWindowRect(info.hwnd, &rect);
                    info.top_left.x = rect.left;
                    info.top_left.y = rect.top;
                    info.style = 0;
                    SetWindowLongPtr(info.hwnd, GWL_STYLE, info.style);

                    ShowWindow(info.hwnd, SW_MAXIMIZE);
                }
                else
                {
                    info.style = WS_VISIBLE | WS_OVERLAPPEDWINDOW;
                    SetWindowLongPtr(info.hwnd, GWL_STYLE, info.style);
                    ResizeWindow(info, info.client_area);
                    ShowWindow(info.hwnd, SW_SHOWNORMAL);
                }
            }
        }

        bool IsWindowFullscreen(window_id id)
        {
            return GetFromId(id).is_fullscreen;
        }

        window_handle GetWindowHandle(window_id id)
        {
            return GetFromId(id).hwnd;
        }

        void SetWindowCaption(window_id id, const wchar_t* caption)
        {
            WindowInfo& info{ GetFromId(id) };
            SetWindowText(info.hwnd, caption);
        }
        
        math::u32v4 GetWindowSize(window_id id)
        {
            WindowInfo& info { GetFromId(id) };
            RECT area{ info.is_fullscreen ? info.fullscreen_area : info.client_area };
            return { (u32)area.left, (u32)area.top, (u32)area.right, (u32)area.right };
        }

        bool IsWindowClosed(window_id id)
        {
            return GetFromId(id).is_closed;
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
        info.client_area.right = (init_info && init_info->width) ? info.client_area.left + init_info->width : info.client_area.right;
        info.client_area.bottom = (init_info && init_info->height) ? info.client_area.top + init_info->height : info.client_area.bottom;
        info.style |= parent ? WS_CHILD : WS_OVERLAPPEDWINDOW;
        
        RECT rect{ info.client_area };

        // Adjust the window size for the correct device size
        AdjustWindowRect(&rect, info.style, FALSE);
        
        const wchar_t* caption{ (init_info && init_info->caption) ? init_info->caption : L"IAD Game" };
        const s32 left{ init_info ? init_info->left : info.top_left.x };
        const s32 top{ init_info ? init_info->top : info.top_left.y };
        const s32 width{ rect.right - rect.left };
        const s32 height{ rect.bottom - rect.top };
        
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
            DEBUG_OP(SetLastError(0));
            window_id id{ AddToWindows(info) };
            SetWindowLongPtr(info.hwnd, GWLP_USERDATA, (LONG_PTR)id);

            // Set in the "extra" bytes of the pointer to the windows callback function
            // which handles messaged for the window
            SetWindowLongPtr(info.hwnd, 0, (LONG_PTR)callback);
            assert(GetLastError() == 0);

            ShowWindow(info.hwnd, SW_SHOWNORMAL);
            UpdateWindow(info.hwnd);

            return Window{ id };
        }
        return{};
    }

    void RemoveWindow(window_id id)
    {
        WindowInfo& info{ GetFromId(id) };
        DestroyWindow(info.hwnd);
        RemoveFromWindows(id);
    }
#elif
#error "Must implement at least one platform."
#endif

    void Window::SetFullscreen(bool isFullscreen) const
    {
        assert(IsValid());
        SetWindowFullScreen(_id, isFullscreen);
    }

    bool Window::IsFullscreen() const
    {
        assert(IsValid());
        return IsWindowFullscreen(_id);
    }

    void* Window::Handle() const
    {
        assert(IsValid());
        return GetWindowHandle(_id);
    }

    void Window::SetCaption(const wchar_t* caption) const
    {
        assert(IsValid());
        SetWindowCaption(_id, caption);
    }


    math::u32v4 Window::Size() const
    {
        assert(IsValid());
        return GetWindowSize(_id);
    }


    void Window::Resize(u32 width, u32 height) const
    {
        assert(IsValid());
        ResizeWindow(_id, width, height);;
    }

    u32 Window::Width() const
    {
        math::u32v4 s{ Size() };

        return s.z - s.x;
    }

    u32 Window::Height() const
    {
        math::u32v4 s{ Size() };

        return s.w - s.y;
    }

    bool Window::IsClosed() const
    {
        assert(IsValid());
        return IsWindowClosed(_id);
    }

}
