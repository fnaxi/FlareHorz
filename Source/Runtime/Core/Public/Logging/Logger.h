// CopyRight FlareHorz Team. All Rights Reserved.

#pragma once

#include "LogCategory.h"

CORE_API DECLARE_LOG_CATEGORY(LogTemp, All)

class CLogger
{
	DEFINE_SINGLETON(CORE_API, CLogger)

	CLogger() = default;

public:
	CORE_API void Log(const SLogCategoryBase& InCategory, ELogVerbosity InVerbosity, const TCHAR* Format, ...);

private:
	static int32 Printf(const TCHAR* Format, ...);
	static int32 Printf_V(const TCHAR* Format, va_list Args);
};

#define FH_LOG(Category, Verbosity, Format, ...) \
	{ \
		CLogger::Get().Log(Category, ELogVerbosity::Verbosity, Format, ##__VA_ARGS__); \
	}
