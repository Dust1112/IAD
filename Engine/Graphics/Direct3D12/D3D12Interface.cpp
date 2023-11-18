#include "D3D12Interface.hpp"

#include "D3D12Core.hpp"
#include "../../Common/CommonHeaders.hpp"
#include "../../Graphics/GraphicsPlatformInterface.hpp"

void iad::graphics::d3d12::get_platform_interface(platform_interface& pi)
{
    pi.initialize = core::initialize;
    pi.shutdown = core::shutdown;
    pi.render = core::render;
}
