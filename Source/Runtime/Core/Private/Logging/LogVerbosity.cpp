// CopyRight FlareHorz Team. All Rights Reserved.


#include "Logging/LogVerbosity.h"

const TCHAR* LogVerbosityToString(const ELogVerbosity Verbosity)
{
	switch (Verbosity)
	{
		case ELogVerbosity::Fatal:			return FH_TEXT("Fatal");
		case ELogVerbosity::Error:			return FH_TEXT("Error");
		case ELogVerbosity::Warning:		return FH_TEXT("Warning");
		case ELogVerbosity::Log:			return FH_TEXT("Log");
		case ELogVerbosity::Verbose:		return FH_TEXT("Verbose");
		case ELogVerbosity::VeryVerbose:	return FH_TEXT("VeryVerbose");
		default:							return FH_TEXT("Unknown");
	}
}

