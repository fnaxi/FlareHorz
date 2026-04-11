// CopyRight FlareHorz Team. All Rights Reserved.

#pragma once

#if defined( _WIN64 )
	#define PLATFORM_64BITS		1
#else
	#define PLATFORM_64BITS		0
#endif

/** Standard C function */
#define CDECL					__cdecl

/** Standard calling convention */
#define STDCALL					__stdcall

/** Force code to be inline */
#define FORCEINLINE				__forceinline

/** Force code to NOT be inline */
#define FORCENOINLINE			__declspec(noinline)

/** DLL export and import definitions */
#define DLLEXPORT				__declspec(dllexport)
#define DLLIMPORT				__declspec(dllimport)

#ifndef _UNICODE
	#error FlareHorz does not support ANSI builds!
#endif

/**
 * Windows specific types.
 */
struct SWindowsPlatformTypes
{
	/*----------------------------------------------------------------------------
		Unsigned integer types
	----------------------------------------------------------------------------*/
	
	/** 8-bit unsigned integer */
	typedef unsigned char 		uint8;

	/** 16-bit unsigned integer */
	typedef unsigned short int	uint16;

	/** 32-bit unsigned integer */
	typedef unsigned int		uint32;

	/** 64-bit unsigned integer */
	typedef unsigned long long	uint64;

	/*----------------------------------------------------------------------------
		Signed integer types
	----------------------------------------------------------------------------*/

	/** 8-bit signed integer */
	typedef	signed char			int8;

	/** 16-bit signed integer */
	typedef signed short int	int16;

	/** 32-bit signed integer */
	typedef signed int	 		int32;

	/** 64-bit signed integer */
	typedef signed long long	int64;

	/*----------------------------------------------------------------------------
		Character types
	----------------------------------------------------------------------------*/

	/** An ANSI character. 8-bit fixed-width representation of 7-bit characters. */
	typedef char				ANSICHAR;

	/**
	 * A wide character. In-memory only. ?-bit fixed-width representation of the platform's
	 * natural wide character set. Could be different sizes on different platforms.
	 */
	typedef wchar_t				WIDECHAR;
	
	/** A switchable character. */
	typedef WIDECHAR			TCHAR;
	
	/**
	 * An 8-bit character type. In-memory only. 8-bit representation.
	 * Should really be char8_t but making this the generic option is easier for compilers which don't fully support C++20 yet.
	 */
	enum UTF8CHAR : unsigned char {};

	/**
	 * A 16-bit character type. In-memory only.  16-bit representation.
	 * Should really be char16_t but making this the generic option is easier for compilers which don't fully support C++11 yet (i.e. MSVC).
	 */
	typedef uint16				UTF16CHAR;
	
	/**
	 * A 32-bit character type. In-memory only. 32-bit representation.
	 */
	typedef char32_t			UTF32CHAR;

	/*----------------------------------------------------------------------------
		Size types
	----------------------------------------------------------------------------*/
	
#if PLATFORM_64BITS
	typedef unsigned __int64	SIZE_T;
	typedef __int64				SSIZE_T;
#else
	typedef unsigned long		SIZE_T;
	typedef long				SSIZE_T;
#endif
};

typedef SWindowsPlatformTypes SPlatformTypes;
