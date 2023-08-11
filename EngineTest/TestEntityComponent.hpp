#pragma once

#include "Test.hpp"
#include "../Engine/Components/Entity.hpp"
#include "../Engine/Components/Transform.hpp"

using namespace iad;

class EngineTest : public Test
{
public:
	// Inherited via Test
	bool initialize() override;
	void run() override;
	void shutdown() override;

private:
	void create_random();
	void remove_random();

	void print_results();

private:
	utl::vector<game_entity::Entity> entities;

	u32 added{ 0 };
	u32 removed{ 0 };
	u32 num_entities{ 0 };
};