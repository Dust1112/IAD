#include "TestWindow.hpp"

#include <thread>


bool TestWindow::initialize()
{
    for (u32 i{ 0 }; i < _countof(_windows); ++i)
    {
        _windows[i] = iad::platform::CreateAndInitWindow();
    }

    return true;
}

void TestWindow::run()
{
    std::this_thread::sleep_for(std::chrono::milliseconds(10));
}

void TestWindow::shutdown()
{
    for (u32 i{ 0 }; i < _countof(_windows); ++i)
    {
        iad::platform::RemoveWindow(_windows[i].GetId());
    }
    
}
