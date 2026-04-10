// CopyRight FlareHorz Team. All Rights Reserved.

#pragma once

#include "PreprocessorHelpers.h"

/**
 * Makes a type non-copyable and non-movable by deleting copy/move constructors and assignment/move operators.
 * The macro should be placed in the public section of the type for better compiler diagnostic messages.
 * Example usage:
 *
 *	class CMyClassName
 *	{
 *	public:
 *		FH_NONCOPYABLE(CMyClassName)
 *		CMyClassName() = default;
 *	};
 */
#define FH_NONCOPYABLE(TypeName) \
	TypeName(TypeName&&) = delete; \
	TypeName(const TypeName&) = delete; \
	TypeName& operator=(const TypeName&) = delete; \
	TypeName& operator=(TypeName&&) = delete;

/** Defines boilerplate code for the class so it can be used as a singleton. */
	#define DEFINE_SINGLETON(ApiMacro, ClassName) \
	public: \
		FH_NONCOPYABLE(ClassName) \
		\
		/** Gets the singleton instance. */ \
		static ApiMacro ClassName& Get() \
		{ \
			static ClassName Singleton = ClassName(); \
			return Singleton; \
		} \
	private:

/**
 * Defines all bitwise operators for enum classes, so it can be (mostly) used as a regular flags enum
 */
#define ENUM_CLASS_FLAGS(Enum) \
	inline				Enum&	operator|=(Enum& Lhs, Enum Rhs)	{ return Lhs = (Enum)((__underlying_type(Enum))Lhs | (__underlying_type(Enum))Rhs); } \
	inline				Enum&	operator&=(Enum& Lhs, Enum Rhs)	{ return Lhs = (Enum)((__underlying_type(Enum))Lhs & (__underlying_type(Enum))Rhs); } \
	inline				Enum&	operator^=(Enum& Lhs, Enum Rhs)	{ return Lhs = (Enum)((__underlying_type(Enum))Lhs ^ (__underlying_type(Enum))Rhs); } \
	inline constexpr	Enum	operator| (Enum  Lhs, Enum Rhs)	{ return (Enum)((__underlying_type(Enum))Lhs | (__underlying_type(Enum))Rhs); } \
	inline constexpr	Enum	operator& (Enum  Lhs, Enum Rhs)	{ return (Enum)((__underlying_type(Enum))Lhs & (__underlying_type(Enum))Rhs); } \
	inline constexpr	Enum	operator^ (Enum  Lhs, Enum Rhs)	{ return (Enum)((__underlying_type(Enum))Lhs ^ (__underlying_type(Enum))Rhs); } \
	inline constexpr	bool	operator! (Enum  E)				{ return !(__underlying_type(Enum))E; } \
	inline constexpr	Enum	operator~ (Enum  E)				{ return (Enum)~(__underlying_type(Enum))E; }
