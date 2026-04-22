// CopyRight FlareHorz Team. All Rights Reserved.

#pragma once

#include "LogCategory.h"

class CLogger
{
	DEFINE_SINGLETON(CORE_API, CLogger)

	CLogger() = default;

public:
	CORE_API void Log(const SLogCategoryBase& InCategory, ELogVerbosity InVerbosity, const TCHAR* Format, ...);

private:
	static int32 Printf(const TCHAR* Format, ...);
	static int32 Printf_V(const TCHAR* Format, va_list Args);

	static constexpr const TCHAR* NoneColor = FH_TEXT("");
	static constexpr const TCHAR* ResetColor = FH_TEXT("\033[0m");
	static constexpr const TCHAR* YellowColor = FH_TEXT("\033[33m");
	static constexpr const TCHAR* RedColor = FH_TEXT("\033[31m");
	static constexpr const TCHAR* CyanColor = FH_TEXT("\033[36m");
};

#define FH_LOG(Category, Verbosity, Format, ...) \
	{ \
		/* TODO: crash on ELogVerbosity::Fatal */ \
		CLogger::Get().Log(Category, ELogVerbosity::Verbosity, Format, ##__VA_ARGS__); \
	}
