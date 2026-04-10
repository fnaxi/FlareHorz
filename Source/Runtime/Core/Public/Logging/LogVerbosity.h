// CopyRight FlareHorz Team. All Rights Reserved.

#pragma once

#include "CoreTypes.h"

/** 
 * Enum that defines the verbosity levels of the logging system.
 */
enum class ELogVerbosity : uint8
{
	/** Disables logging for a specific log category. */
	NoLogging		= 0,

	/** Always prints a fatal error to log file and crashes (even if logging is disabled). */
	Fatal			= 1,

	/** Prints an error to log file. */
	Error			= 2,

	/** Prints a warning to log file. */
	Warning			= 3,

	/** Prints a message to a log file. */
	Log				= 4,

	/** Prints a verbose message to a log file (if Verbose logging is enabled for the given category, usually used for detailed logging). */
	Verbose			= 5,

	/**
	 * Prints a verbose message to a log file (if VeryVerbose logging is enabled,
	 * usually used for detailed logging that would otherwise spam output)
	 */
	VeryVerbose		= 6,

	/** Enables all logging verbosity for a category. */
	All				= VeryVerbose
};

/**
 * Converts verbosity to a string.
 * @param Verbosity Verbosity enum.
 * @returns String representation of the verbosity enum.
 */
CORE_API const TCHAR* LogVerbosityToString(ELogVerbosity Verbosity);
