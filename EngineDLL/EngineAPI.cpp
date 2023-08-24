#include "Common.hpp"
#include "../Engine/Common/CommonHeaders.hpp"
#include "../Engine/Components/Script.hpp"
#include "../Engine/Graphics/Renderer.hpp"
#include "../Engine/Platform/PlatformTypes.hpp"
#include "../Engine/Platform/Platform.hpp"

#ifndef WIN32_LEAN_AND_MEAN
#define WIN32_LEAN_AND_MEAN
#endif

#include <OleCtl.h>
#include <Windows.h>

namespace
{
    HMODULE game_code_dll{nullptr};
    using _get_script_creator = iad::script::detail::script_creator(*)(size_t);
    _get_script_creator get_script_creator{ nullptr };

    using _get_script_names = LPSAFEARRAY(*)(void);
    _get_script_names get_script_names{ nullptr };

    iad::utl::vector<iad::graphics::RenderSurface> surfaces;
}

EDITOR_INTERFACE u32 LoadGameCodeDll(const char* dll_path)
{
    if (game_code_dll) { return FALSE; }
    game_code_dll = LoadLibraryA(dll_path);
    assert(game_code_dll);

    get_script_names = (_get_script_names)GetProcAddress(game_code_dll, "get_script_names");
    get_script_creator = (_get_script_creator)GetProcAddress(game_code_dll, "get_script_creator");
    
    return (game_code_dll && get_script_names && get_script_creator) ? TRUE : FALSE;
}

EDITOR_INTERFACE u32 UnloadGameCodeDll()
{
    if (!game_code_dll) { return FALSE; }
    assert(game_code_dll);

    int result{ FreeLibrary(game_code_dll) };
    assert(result);
    game_code_dll = nullptr;

    return TRUE;
}

EDITOR_INTERFACE iad::script::detail::script_creator GetScriptCreator(const char* name)
{
    return (game_code_dll && get_script_creator)
        ? get_script_creator(iad::script::detail::string_hash()(name))
        : nullptr;
}

EDITOR_INTERFACE LPSAFEARRAY GetScriptNames()
{
    return (game_code_dll && get_script_names) ? get_script_names() : nullptr;
}

EDITOR_INTERFACE u32 CreateRenderSurface(HWND host, s32 width, s32 height)
{
    assert(host);
    iad::platform::WindowInitInfo info{ nullptr, host, nullptr, 0, 0, width, height };
    iad::graphics::RenderSurface surface{ iad::platform::CreateAndInitWindow(&info), {} };
    assert(surface.window.IsValid());

    surfaces.emplace_back(surface);

    return (u32)surfaces.size() - 1;
}

EDITOR_INTERFACE void RemoveRenderSurface(u32 id)
{
    assert(id < surfaces.size());
    iad::platform::RemoveWindow(surfaces[id].window.GetId());
}

EDITOR_INTERFACE HWND GetWindowHandle(u32 id)
{
    assert(id < surfaces.size());
    return (HWND)surfaces[id].window.Handle();
}

EDITOR_INTERFACE void ResizeRenderSurface(u32 id)
{
    assert(id < surfaces.size());
    surfaces[id].window.Resize(0, 0);
}