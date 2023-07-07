#pragma once

class Test
{
public:
	virtual bool initialize() = 0;
	virtual void run() = 0;
	virtual void shutown() = 0;
};