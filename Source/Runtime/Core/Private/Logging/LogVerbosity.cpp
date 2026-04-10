// CopyRight FlareHorz Team. All Rights Reserved.


#include "Logging/LogVerbosity.h"

const TCHAR* LogVerbosityToString(const ELogVerbosity Verbosity)
{
	switch (Verbosity)
	{
		case ELogVerbosity::Fatal:			return TEXT("Fatal");
		case ELogVerbosity::Error:			return TEXT("Error");
		case ELogVerbosity::Warning:		return TEXT("Warning");
		case ELogVerbosity::Log:			return TEXT("Log");
		case ELogVerbosity::Verbose:		return TEXT("Verbose");
		case ELogVerbosity::VeryVerbose:	return TEXT("VeryVerbose");
		default:							return TEXT("Unknown");
	}
}

