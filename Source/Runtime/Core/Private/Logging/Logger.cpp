// CopyRight FlareHorz Team. All Rights Reserved.


#include "Logging/Logger.h"

#include <cstdarg>

DEFINE_LOG_CATEGORY(LogTemp)

void CLogger::Log(const SLogCategoryBase& InCategory, const ELogVerbosity InVerbosity, const TCHAR* Format, ...)
{
	if (InVerbosity > InCategory.GetVerbosity()) return;

	// TODO: Aligned logging

	va_list Args;
	va_start(Args, Format);

	Printf(TEXT("%s: %s: "), *InCategory.GetName(), LogVerbosityToString(InVerbosity));
	Printf_V(Format, Args);
	Printf(TEXT("\n"));
	
	va_end(Args);
}

int32 CLogger::Printf(const TCHAR* Format, ...)
{
	va_list Args;
	va_start(Args, Format);

	int32 Result = Printf_V(Format, Args);

	va_end(Args);
	return Result;
}

int32 CLogger::Printf_V(const TCHAR* Format, va_list Args)
{
	return vwprintf(Format, Args);
}
