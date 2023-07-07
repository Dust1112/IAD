#pragma once

#include "ComponentsCommon.hpp"

namespace iad::transform
{
	struct init_info
	{
		f32 position[3]{};
		f32 rotation[4]{};
		f32 scale[3]{ 1.f, 1.f, 1.f };
	};

	Component create(const init_info& info, game_entity::Entity entity);
	void remove(Component component);
}