#include "D3D12Core.hpp"

namespace
{
    ID3D12Device8* main_device;
}

bool iad::graphics::d3d12::core::initialize()
{
    // Determine which adapter (i.e. graphics card) to use
    // Determine what is the maximum feature level that is supported
    // Create a ID3D12Device (this is a virtual adapter)
}

void iad::graphics::d3d12::core::shutdown()
{
    
}

