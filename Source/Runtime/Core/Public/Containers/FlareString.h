// CopyRight FlareHorz Team. All Rights Reserved.

#pragma once

#include "Array.h"

// TODO: CName and CText

class CStringHelpers
{
public:
	static SIZE_T StrLen(const TCHAR* Str);
};

/**
 * A dynamically sizeable string.
 *
 * The internal buffer is always null-terminated, allowing safe access as a C-style string via CStr() or operator*().
 *
 * TODO: CString is only partly implemented
 */
class CORE_API CString
{
	enum : uint8 { NULL_TERMINATOR = '\0' };
	using UElementType = TCHAR;
	
public:
	CString();
	
	// ReSharper disable once CppNonExplicitConvertingConstructor
	CString(const UElementType* Str);
	
	/** Get the length of the string, excluding terminating character. */
	[[nodiscard]] UArraySizeType Length() const;

	// TODO: operator+ and Printf()
	
	/** Get pointer to the string. */
	[[nodiscard]] FORCEINLINE UElementType* CStr()
	{
		return Data.GetData();
	}
	[[nodiscard]] FORCEINLINE const UElementType* CStr() const
	{
		return Data.GetData();
	}

private:
	/** Array holding the character data. */
	TArray<UElementType> Data;

public:
	/*----------------------------------------------------------------------------
		Operators
	----------------------------------------------------------------------------*/

	const UElementType* operator*() const;
	
	UElementType& operator[](UArraySizeType Index);
	const UElementType& operator[](UArraySizeType Index) const;

	bool operator==(const CString& Other) const;
};
