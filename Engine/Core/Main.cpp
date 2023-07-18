/* Things to do in order to create a game project (for possible automation):
 * 1) Generate an MSVC solution.project
 * 2) Add files that contain the script
 * 3) Set include and library directories
 * 4) Set force include files (GameEntity.h)
 * 5) Set C++ language level version (C++ 20) and calling convention (fast)
 */

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
  
 }

 return 0;
}
#endif

#endif