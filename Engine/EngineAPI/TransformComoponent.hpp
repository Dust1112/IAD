#pragma once

#include "../Components/ComponentsCommon.hpp"

namespace iad::transform
{
	DEFINE_TYPED_ID(transform_id);

	class Component final
	{
	public:
		constexpr explicit Component(transform_id id) : _id{ id } {}
		constexpr Component() : _id{ id::invalid_id } {}
		constexpr transform_id get_id() const { return _id; }
		constexpr bool is_valid() const { return id::is_valid(_id); }

		math::v3 position() const;
		math::v4 rotation() const;
		math::v3 scale() const;
	private:
		transform_id _id;
	};
}