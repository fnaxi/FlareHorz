// CopyRight FlareHorz Team. All Rights Reserved.


#include "Logging/Logger.h"

#include <cstdarg>

void CLogger::Log(const SLogCategoryBase& InCategory, const ELogVerbosity InVerbosity, const TCHAR* Format, ...)
{
	if (InVerbosity > InCategory.GetVerbosity()) return;
	
	const TCHAR* Color = NoneColor;
	switch (InVerbosity)
	{
	case ELogVerbosity::Error:
		Color = RedColor;
		break;
	case ELogVerbosity::Warning:
		Color = YellowColor;
		break;
	case ELogVerbosity::Verbose: case ELogVerbosity::VeryVerbose:
		Color = CyanColor;
		break;
	default: break;
	}
	
	va_list Args;
	va_start(Args, Format);
	
	Printf(FH_TEXT("%s"), Color);
	Printf(FH_TEXT("%-13s%-26s"), LogVerbosityToString(InVerbosity), *InCategory.GetName());
	Printf_V(Format, Args);
	Printf(FH_TEXT("%s"), ResetColor);

	Printf(FH_TEXT("\n"));
	
	va_end(Args);
}

int32 CLogger::Printf(const TCHAR* Format, ...)
{
	va_list Args;
	va_start(Args, Format);

	const int32 Result = Printf_V(Format, Args);

	va_end(Args);
	return Result;
}

int32 CLogger::Printf_V(const TCHAR* Format, va_list Args)
{
	return vprintf(Format, Args); // NOLINT(clang-diagnostic-format-nonliteral)
}
