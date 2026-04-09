// CopyRight FlareHorz Team. All Rights Reserved.

#pragma once

#include <memory>

/*----------------------------------------------------------------------------
	These typedefs and helper functions are a temporary wrappers over STL smart
	pointers (std::unique_ptr, std::shared_ptr, std::weak_ptr).

	They exist ONLY as a placeholder until we introduce our own custom smart
	pointer implementations and memory management system.

	// TODO: Smart pointers
----------------------------------------------------------------------------*/

template <typename T>
using TUniquePtr = std::unique_ptr<T>;

/**
 * TSharedPtr is a non-intrusive reference-counted authoritative object pointer.
 */
template <typename T>
using TSharedPtr = std::shared_ptr<T>;

template <typename T>
using TWeakPtr = std::weak_ptr<T>;

template <class T, class... TArgs>
TSharedPtr<T> MakeShared(TArgs&&... Args) { return std::make_shared<T>(std::forward<TArgs>(Args)...); }

template <class T, class... TArgs>
TSharedPtr<T> MakeUnique(TArgs&&... Args) { return std::make_unique<T>(std::forward<TArgs>(Args)...); }

// Wraps STL smart pointer's method names to keep consent name style across codebase
#define PTR_Reset reset
#define PTR_Get get
