#include "Common.hpp"
#include "../Engine/Common/CommonHeaders.hpp"
#include "../Engine/Common/Id.hpp"
#include "../Engine/Components/Entity.hpp"
#include "../Engine/Components/Transform.hpp"

using namespace iad;

namespace
{
    struct TransformComponent
    {
        f32 position[3];
        f32 rotation[3];
        f32 scale[3];

        transform::init_info ToInitInfo()
        {
            transform::init_info info{};
            memcpy(&info.position[0], &position[0], sizeof(f32) * _countof(position));
            memcpy(&info.scale[0], &scale[0], sizeof(f32) * _countof(scale));
            DirectX::XMFLOAT3A rot{ &rotation[0] };
            DirectX::XMVECTOR quat{ DirectX::XMQuaternionRotationRollPitchYawFromVector(DirectX::XMLoadFloat3A(&rot)) };
            DirectX::XMFLOAT4A rot_quat{};
            DirectX::XMStoreFloat4A(&rot_quat, quat);
            memcpy(&info.rotation[0], &rot_quat.x, sizeof(f32) * _countof(info.rotation));

            return info;
        }
    };

    struct GameEntityDescriptor
    {
        TransformComponent transform;
    };

    game_entity::Entity GetEntityFromId(id::id_type id)
    {
        return game_entity::Entity{ game_entity::entity_id{ id } };
    }
} // anonymous namespace

EDITOR_INTERFACE id::id_type CreateGameEntity(GameEntityDescriptor* e)
{
    assert(e);
    GameEntityDescriptor& descriptor{ *e };
    transform::init_info transformInfo{ descriptor.transform.ToInitInfo() };
    game_entity::entity_info entity_info{ &transformInfo };

    return game_entity::create(entity_info).get_id();
}

EDITOR_INTERFACE void RemoveGameEntity(id::id_type id)
{
    assert(id::is_valid(id));
    game_entity::remove(game_entity::entity_id{ id });
}