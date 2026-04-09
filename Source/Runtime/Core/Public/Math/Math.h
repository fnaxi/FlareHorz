// CopyRight FlareHorz Team. All Rights Reserved.

#pragma once

class CMath
{
public:
	template <typename T>
	static const T& Max(const T& A, const T& B)
	{
		return (A < B) ? B : A;
	}
};
