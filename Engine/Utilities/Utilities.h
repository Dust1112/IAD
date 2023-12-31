#pragma once

#define USE_STL_VECTOR 1
#define USE_STL_DEQUE 1
#include <algorithm>

#if USE_STL_VECTOR
#include <vector>
namespace iad::utl
{
	template<typename T>
	using vector = typename std::vector<T>;

	template<typename T>
	void erase_unordered(std::vector<T>& vector, size_t index)
	{
		if (vector.size() > 1)
		{
			std::iter_swap(vector.begin() + index, vector.end() - 1);
			vector.pop_back();
		}
		else
		{
			vector.clear();
		}
	}
}
#endif

#if USE_STL_DEQUE
#include <deque>
namespace iad::utl
{
	template<typename T>
	using deque = typename std::deque<T>;
}
#endif