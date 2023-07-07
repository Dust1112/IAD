#pragma once

#include "..\Components\ComponentsCommon.hpp"

namespace iad::script
{
    DEFINE_TYPED_ID(script_id);

    class Component final
    {
    public:
        constexpr explicit Component(script_id id) : _id { id } { }
        constexpr Component() : _id { id::invalid_id } { }
        [[nodiscard]] constexpr script_id get_id() const { return _id; }
        [[nodiscard]] constexpr bool is_valid() const { return id::is_valid(_id); }
        
    private:
        script_id _id;
    };
}
