#ifdef _WIN64

#ifndef WIN32_LEAN_AND_MEAN
#define WIN32_LEAN_AND_MEAN
#endif

#include <Windows.h>
#include <crtdbg.h>

#ifndef USE_WITH_EDITOR

extern bool EngineInitialise();
extern void EngineUpdate();
extern void EngineShutdown();

int WINAPI WinMain(HINSTANCE, HINSTANCE, LPSTR, int)
{
#if _DEBUG
 _CrtSetDbgFlag(_CRTDBG_ALLOC_MEM_DF | _CRTDBG_LEAK_CHECK_DF);
#endif

    if (EngineInitialise())
    {
        MSG msg{};
        bool is_running{ true };
        while (is_running)
        {
            EngineUpdate();
            while (PeekMessage(&msg, NULL, 0, 0, PM_REMOVE))
            {
                TranslateMessage(&msg);
                DispatchMessage(&msg);
                is_running &= (msg.message != WM_QUIT);
            }
        }
    }

    EngineShutdown();
    return 0;
}
#endif

#endif