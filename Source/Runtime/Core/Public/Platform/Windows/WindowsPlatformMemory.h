// CopyRight FlareHorz Team. All Rights Reserved.

#pragma once

#include <type_traits>

class CWindowsMemoryManager
{
public:
	template <typename T>
	static constexpr CORE_API std::remove_reference_t<T>&& Move(T&& Arg) noexcept
	{
		return static_cast<std::remove_reference_t<T>&&>(Arg);
	}
};

typedef CWindowsMemoryManager CMemoryManager;
