#include "TestEntityComponent.hpp"

#include <iostream>
#include <ctime>

bool engine_test::initialize()
{
    srand((u32)time(nullptr));

    return true;
}

void engine_test::run()
{
    do {
        for (u32 i{ 0 }; i < 10000; ++i)
        {
            create_random();
            remove_random();
            num_entities = (u32)entities.size();
        }
        print_results();
    } while (getchar() != 'q');
}

void engine_test::shutdown()
{
}

void engine_test::create_random()
{
    u32 count = rand() % 20;
    if (entities.empty()) { count = 1000; }

    transform::init_info transform_info{};
    game_entity::entity_info entity_info{ &transform_info, };

    while (count > 0)
    {
        ++added;
        game_entity::Entity entity{ game_entity::create(entity_info) };
        assert(entity.is_valid());
        entities.push_back(entity);
        --count;
    }
}

void engine_test::remove_random()
{
    u32 count = rand() % 20;
    if (entities.size() < 1000) { return; }

    while (count > 0)
    {
        const u32 index{ (u32)rand() % (u32)entities.size() };
        const game_entity::Entity entity{ entities[index] };
        assert(entity.is_valid() && id::is_valid(entity.get_id()));
        
        if (entity.is_valid())
        {
            game_entity::remove(entity.get_id());
            entities.erase(entities.begin() + index);
            assert(!game_entity::is_alive(entity.get_id()));
            ++removed;
        }

        --count;
    }
}

void engine_test::print_results()
{
    std::cout << "Entities created: " << added << "\n";
    std::cout << "Entities removed: " << removed << "\n";
}
