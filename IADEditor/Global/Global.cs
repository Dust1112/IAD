public static class Global
{
    public static readonly string cppCode = @"#include ""{0}.h""

namespace {1}
{{
    REGISTER_SCRIPT({0})
    void {0}::BeginPlay()
    {{
        // Your code here
    }}

    void {0}::Update(float deltaTime)
    {{
        // Your code here
    }}
}}
";

    public static readonly string hCode = @"#pragma once

namespace {1}
{{
    class {0} : public iad::script::EntityScript
    {{
    public:
    constexpr explicit {0}(iad::game_entity::Entity entity)
        : iad::script::EntityScript{{entity}} {{  }}
    
    void BeginPlay() override;
    void Update(float deltaTime) override;

    private:
    }};
}}";
}