#if !defined(SHIPPING)

#include "../Content/ContentLoader.hpp"
#include "../Components/Script.hpp"

#include <thread>

bool EngineInitialise()
{
    bool result{ iad::content::LoadGame() };
    
    return true;
}

void EngineUpdate()
{
    iad::script::Update(10.f);
    std::this_thread::sleep_for(std::chrono::milliseconds(10));    
}

void EngineShutdown()
{
    iad::content::UnloadGame();
}

#endif
