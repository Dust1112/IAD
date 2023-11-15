﻿#pragma once

#include "../../Common/CommonHeaders.hpp"
#include "../../Graphics/Renderer.hpp"

#include <dxgi1_6.h>
#include <d3d12.h>
#include <wrl.h>

#pragma comment(lib, "dxgi.lib")
#pragma comment(lib, "d3d12.lib")

// Assert that COM call to D3D succeeded
#ifdef _DEBUG
#ifndef DXCall 
#define DX_CALL(x)                                  \
    if (FAILED(x)) {                                \
        char line_number[32];                       \
        sprintf_s(line_number, "%u", __LINE__);     \
        OutputDebugStringA("Error in ");            \
        OutputDebugStringA(__FILE__);               \
        OutputDebugStringA("\nLine ");              \
        OutputDebugStringA(line_number);            \
        OutputDebugStringA("\n");                   \
        OutputDebugStringA(#x);                     \
        OutputDebugStringA("\n");                   \
        __debugbreak();                             \
    }
#endif
#else
#ifndef DXCall
#define DXCall(x) x
#endif
#endif

#ifdef _DEBUG
// Sets the name of the COM object and outputs a debug string in the IDE's output panel/console
#define NAME_D3D12_OBJECT(obj, name) obj->SetName(name); OutputDebugString(L"::D3D12 Object Created: "); OutputDebugString(name); OutputDebugString(L"\n");
#else
#defineNAME_D3D12_OBJECT(x, name)
#endif
