// CopyRight FlareHorz Team. All Rights Reserved.


#include "Containers/FlareString.h"

SIZE_T CStringHelpers::StrLen(const TCHAR* Str)
{
	return std::wcslen(Str);
}

CString::CString()
{
	Data = TArray<UElementType>( { NULL_TERMINATOR } );
}

CString::CString(const UElementType* Str)
{
	const SIZE_T Lenght = CStringHelpers::StrLen(Str);
	const UArraySizeType Num = static_cast<UArraySizeType>(Lenght);

	Data.SetNum(Num + 1);
	
	for (UArraySizeType i = 0; i < Num; ++i)
	{
		Data[i] = Str[i];
	}
	Data[Num] = NULL_TERMINATOR;
}

UArraySizeType CString::Length() const
{
	return Data.Num() > 0 ? Data.Num() - 1 : 0;
}

const CString::UElementType* CString::operator*() const
{
	return CStr();
}

CString::UElementType& CString::operator[](const UArraySizeType Index)
{
	return Data[Index];
}

const CString::UElementType& CString::operator[](const UArraySizeType Index) const
{
	return Data[Index];
}

bool CString::operator==(const CString& Other) const
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

