#pragma once

#include "../Test.hpp"

class renderer_test : public test
{
public:
    bool initialize() override;
    void run() override;
    void shutdown() override;
};
