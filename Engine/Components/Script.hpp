#pragma once

#include "ComponentsCommon.hpp"

namespace iad::script
{
    struct init_info
    {
        detail::script_creator script_creator;
    };

    Component Create(init_info info, game_entity::Entity entity);
    void Remove(Component component);
    void Update(float dt);
}
