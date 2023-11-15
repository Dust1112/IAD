#pragma comment(lib, "engine.lib")

#define TEST_ENTITY_COMPONENTS 0
#define TEST_WINDOW 0
#define TEST_RENDERER 1

#if TEST_ENTITY_COMPONENTS
#include "TestEntityComponent.hpp"
entity_test test{};
#elif TEST_WINDOW
#include "TestWindow.hpp"
window_test test{};
#elif TEST_RENDERER
#include "RendererTest/TestRenderer.hpp"
renderer_test test{};
#else
#error One of the tests need to be enabled
#endif

#ifdef _WIN64

#include <crtdbg.h>
#include <Windows.h>

int WINAPI WinMain(HINSTANCE, HINSTANCE, LPSTR, int)
{
#if _DEBUG
	_CrtSetDbgFlag(_CRTDBG_ALLOC_MEM_DF | _CRTDBG_LEAK_CHECK_DF);
#endif
	
	if (test.initialize())
	{
		MSG msg{};
		bool is_running{ true };
		while (is_running)
		{
			while (PeekMessage(&msg, NULL, 0, 0, PM_REMOVE))
			{
				TranslateMessage(&msg);
				DispatchMessage(&msg);
				is_running &= (msg.message != WM_QUIT);
			}

			test.run();
		}
	}

	test.shutdown();
	return 0;
}
#else
int main()
{
#if _DEBUG
	_CrtSetDbgFlag(_CRTDBG_ALLOC_MEM_DF | _CRTDBG_LEAK_CHECK_DF);
#endif
	
	EngineTest test{};

	if (test.initialize())
	{
		test.run();
	}

	test.shutdown();

	return 0;
}
#endif
