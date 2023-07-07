#pragma once

#include "ComponentsCommon.hpp"

namespace iad::script
{
    struct init_info
    {
        detail::script_creator script_creator;
    };

    Component create(init_info info, game_entity::Entity entity);
    void remove(Component component);
}
