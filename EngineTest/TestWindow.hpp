#pragma once

#include "Test.hpp"
#include "../Engine/Platform/Platform.hpp"

inline iad::platform::Window _windows[4];

class window_test : test
{
public:
    // Inherited via Test
    bool initialize() override;
    void run() override;
    void shutdown() override;
};
