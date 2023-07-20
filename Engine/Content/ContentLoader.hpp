#pragma once

#include "../Common/CommonHeaders.hpp"

#if !defined(SHIPPING)
namespace iad::content
{
    bool LoadGame();
    void UnloadGame();
}
#endif