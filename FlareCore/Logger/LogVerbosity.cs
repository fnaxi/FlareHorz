// CopyRight FlareHorz Engine Development Team. All Rights Reserved.

namespace FlareCore.Logger;

/// <summary>
/// Verbosity of the log.
/// </summary>
[FlareCoreAPI]
public enum ELogVerbosity
{
	/// <summary>
	/// Not used.
	/// </summary>
	NoLogging = 0,
	
	/// <summary>
	/// Prints an error to console (and log file).
	/// </summary>
	Error = 1,
	
	/// <summary>
	/// Prints a warning to console (and log file).
	/// </summary>
	Warning = 2,
	
	/// <summary>
	/// Prints a message to console (and log file).
	/// </summary>
	Info = 3,
	
	/// <summary>
	/// Prints a debug message to console (and log file).
	/// Works only in Debug configuration.
	/// </summary>
	Debug = 4,
	
	/// <summary>
	/// Enable all logging verbosities.
	/// </summary>
	All = 5
}
