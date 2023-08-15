#if !defined(SHIPPING)

#include "../Content/ContentLoader.hpp"
#include "../Components/Script.hpp"
#include "../Platform/PlatformTypes.hpp"
#include "../Platform/Platform.hpp"
#include "../Graphics/Renderer.hpp"

#include <thread>

iad::graphics::RenderSurface game_window{};
namespace
{
    LRESULT WinProc(HWND hwnd, UINT msg, WPARAM wparam, LPARAM lparam)
    {
        switch (msg)
        {
        case WM_DESTROY:
            if (game_window.window.IsClosed())
            {
                PostQuitMessage(0);
                return 0;
            }

            break;
        
        case WM_SYSCHAR:
            if (wparam == VK_RETURN && (HIWORD(lparam) & KF_ALTDOWN))
            {
                game_window.window.SetFullscreen(!game_window.window.IsFullscreen());
                return 0;
            }
            break;
        }

        return DefWindowProc(hwnd, msg, wparam, lparam);
    }
}

bool EngineInitialise()
{
    if (!iad::content::LoadGame()) { return false; }

    iad::platform::WindowInitInfo info
    {
        // TODO Get the game name from the loaded file
        &WinProc, nullptr, L"IAD Game"
    };


    game_window.window = iad::platform::CreateAndInitWindow(&info);
    if (!game_window.window.IsValid()) { return false; }
    
    return true;
}

void EngineUpdate()
{
    iad::script::Update(10.f);
    std::this_thread::sleep_for(std::chrono::milliseconds(10));    
}

void EngineShutdown()
{
    iad::platform::RemoveWindow(game_window.window.GetId());
    iad::content::UnloadGame();
}

#endif
