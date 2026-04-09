// CopyRight FlareHorz Team. All Rights Reserved.

#pragma once

#ifndef PLATFORM_WINDOWS
	#define PLATFORM_WINDOWS 0
#endif
#ifndef PLATFORM_MAC
	#define PLATFORM_MAC 0
#endif
#ifndef PLATFORM_LINUX
	#define PLATFORM_LINUX 0
#endif

#if PLATFORM_WINDOWS
	#include "Platform/Windows/WindowsPlatform.h"
#else
	#error Unsupported platform!
#endif

#define PLATFORM_32BITS	(!PLATFORM_64BITS)
#if !PLATFORM_64BITS
	#error FlareHorz only supports 64-bit platforms!
#endif

// If we don't have a platform-specific define for the TEXT macro, define it now
#if !defined(TEXT)
	#define TEXT(x) L ## x
#endif

/*----------------------------------------------------------------------------
	Transfer the platform types to global types
----------------------------------------------------------------------------*/

// Unsigned base types
typedef SPlatformTypes::uint8		uint8;
typedef SPlatformTypes::uint16		uint16;
typedef SPlatformTypes::uint32		uint32;
typedef SPlatformTypes::uint64		uint64;

// Signed base types
typedef	SPlatformTypes::int8		int8;
typedef SPlatformTypes::int16		int16;
typedef SPlatformTypes::int32		int32;
typedef SPlatformTypes::int64		int64;

// Character types
typedef SPlatformTypes::ANSICHAR	ANSICHAR;
typedef SPlatformTypes::WIDECHAR	WIDECHAR;
typedef SPlatformTypes::TCHAR		TCHAR;

typedef SPlatformTypes::UTF8CHAR	UTF8CHAR;
typedef SPlatformTypes::UTF16CHAR	UTF16CHAR;
typedef SPlatformTypes::UTF32CHAR	UTF32CHAR;

// Size types
typedef SPlatformTypes::SIZE_T		SIZE_T;
typedef SPlatformTypes::SSIZE_T		SSIZE_T;

/*----------------------------------------------------------------------------
	Test the global types
----------------------------------------------------------------------------*/
namespace FH::Core::TypeTests
{
	template <typename A, typename B>
	inline constexpr bool TAreTypesEqual_V = false;

	template <typename T>
	inline constexpr bool TAreTypesEqual_V<T, T> = true;

	static_assert(sizeof(TCHAR) == 2, "TCHAR size must be 2 bytes!");

	static_assert(PLATFORM_32BITS || PLATFORM_64BITS, "Type tests pointer size failed!");
	static_assert(PLATFORM_32BITS != PLATFORM_64BITS, "Type tests pointer exclusive failed!");
	
	static_assert(PLATFORM_32BITS || sizeof(void*) == 8, "Pointer size is 64bit, but pointers are short!");
	static_assert(PLATFORM_64BITS || sizeof(void*) == 4, "Pointer size is 32bit, but pointers are long!");

	static_assert(char(-1) < char(0), "Unsigned char type test failed!");

	static_assert((!TAreTypesEqual_V<ANSICHAR, WIDECHAR>), "ANSICHAR and WIDECHAR should be different types!");
	static_assert(TAreTypesEqual_V<TCHAR, ANSICHAR> || TAreTypesEqual_V<TCHAR, WIDECHAR> || TAreTypesEqual_V<TCHAR, UTF8CHAR>, "TCHAR should either be ANSICHAR, WIDECHAR or UTF8CHAR!");
	
	static_assert(TAreTypesEqual_V<WIDECHAR, wchar_t>, "WIDECHAR should be wchar_t");

	static_assert(sizeof(uint8) == 1, "uint8 type size test failed!");
	static_assert(int32(uint8(-1)) == 0xFF, "uint8 type sign test failed!");

	static_assert(sizeof(uint16) == 2, "uint16 type size test failed!");
	static_assert(int32(uint16(-1)) == 0xFFFF, "uint16 type sign test failed!");

	static_assert(sizeof(uint32) == 4, "uint32 type size test failed!");
	static_assert(int64(uint32(-1)) == int64(0xFFFFFFFF), "uint32 type sign test failed!");

	static_assert(sizeof(uint64) == 8, "uint64 type size test failed!");
	static_assert(uint64(-1) > uint64(0), "uint64 type sign test failed!");

	static_assert(sizeof(int8) == 1, "int8 type size test failed!");
	static_assert(int32(int8(-1)) == -1, "int8 type sign test failed!");

	static_assert(sizeof(int16) == 2, "int16 type size test failed!");
	static_assert(int32(int16(-1)) == -1, "int16 type sign test failed!");

	static_assert(sizeof(int32) == 4, "int32 type size test failed!");
	static_assert(int64(int32(-1)) == int64(-1), "int32 type sign test failed!");

	static_assert(sizeof(int64) == 8, "int64 type size test failed!");
	static_assert(int64(-1) < int64(0), "int64 type sign test failed!");

	static_assert(sizeof(ANSICHAR) == 1, "ANSICHAR type size test failed!");
	static_assert(int32(ANSICHAR(-1)) == -1, "ANSICHAR type sign test failed!");

	static_assert(sizeof(WIDECHAR) == 2 || sizeof(WIDECHAR) == 4, "WIDECHAR type size test failed!");

	static_assert(sizeof(UTF8CHAR)  == 1, "UTF8CHAR type size test failed!");
	static_assert(sizeof(UTF16CHAR) == 2, "UTF16CHAR type size test failed!");
	static_assert(sizeof(UTF32CHAR) == 4, "UTF32CHAR type size test failed!");

	static_assert(sizeof(SIZE_T) == sizeof(void *), "SIZE_T type size test failed!");
	static_assert(SIZE_T(-1) > SIZE_T(0), "SIZE_T type sign test failed!");
}
