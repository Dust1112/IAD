#pragma once

#include <string>

#include "../Components/ComponentsCommon.hpp"
#include "TransformComoponent.hpp"
#include "ScriptComponent.hpp"

namespace iad
{
	namespace game_entity
	{
		DEFINE_TYPED_ID(entity_id)

			class Entity
		{
		public:
			constexpr explicit Entity(entity_id id) : _id{ id } {}
			constexpr Entity() : _id{ id::invalid_id } {}
			constexpr entity_id get_id() const { return _id; }
			constexpr bool is_valid() const { return id::is_valid(_id); }

			transform::Component transform() const;
			script::Component script() const;

		private:
			entity_id _id;
		};
	} // namespace game_entity

	namespace script
	{
		class EntityScript : public game_entity::Entity
		{
		protected:
			constexpr explicit EntityScript(game_entity::Entity entity)
				: game_entity::Entity{ entity } {}

		public:
			virtual ~EntityScript() = default;

		public:
			virtual void BeginPlay() { }
			virtual void Update(float) { }
		};

		namespace detail
		{
			using script_ptr = std::unique_ptr<EntityScript>;
			using script_creator = script_ptr(*)(game_entity::Entity entity);
			using string_hash = std::hash<std::string>;

			u8 RegisterScript(size_t, script_creator);

			template<class ScriptClass>
			script_ptr CreateScript(game_entity::Entity entity)
			{
				assert(entity.is_valid());

				// Create an instance of the script and return a pointer to it
				return std::make_unique<ScriptClass>(entity);
			}
		}

		#define REGISTER_SCRIPT(TYPE)																	\
		class TYPE;																						\
		namespace {																						\
			const u8 _RegCharacterScript##TYPE {														\
				iad::script::detail::RegisterScript(													\
					iad::script::detail::string_hash()(#TYPE),											\
					&iad::script::detail::CreateScript<TYPE> ) };										\
		}
	}
}