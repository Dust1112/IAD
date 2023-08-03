#include "Common.hpp"
#include "../Engine/Common/CommonHeaders.hpp"
#include "../Engine/Components/Script.hpp"

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