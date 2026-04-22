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
	CString()
	{
		Data = TArray<UElementType>( { NULL_TERMINATOR } );
	}
	CString(const UElementType* Str)
	{
		const SIZE_T Lenght = CStringHelpers::StrLen(Str);
		const UArraySizeType Num = static_cast<UArraySizeType>(Lenght);

		Data.SetCount(Num + 1);
	
		for (UArraySizeType i = 0; i < Num; ++i)
		{
			Data[i] = Str[i];
		}
		Data[Num] = NULL_TERMINATOR;
	}
	
	/** Get the length of the string, excluding terminating character. */
	[[nodiscard]] UArraySizeType Length() const
	{
		return Data.Count() > 0 ? Data.Count() - 1 : 0;
	}

	// TODO: Printf()
	
	[[nodiscard]] FORCEINLINE UElementType* Get()
	{
		return Data.GetData();
	}
	[[nodiscard]] FORCEINLINE const UElementType* Get() const
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

	const UElementType* operator*() const
	{
		return Get();
	}
	
	UElementType& operator[](UArraySizeType Index)
	{
		return Data[Index];
	}
	const UElementType& operator[](UArraySizeType Index) const
	{
		return Data[Index];
	}

	bool operator==(const CString& Other) const
	{
		if (Length() != Other.Length())
		{
			return false;
		}

		for (UArraySizeType i = 0; i < Length(); ++i)
		{
			if (Data[i] != Other.Data[i])
			{
				return false;
			}
		}
	
		return true;
	}
};
