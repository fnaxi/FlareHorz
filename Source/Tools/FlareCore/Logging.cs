// CopyRight © FlareHorz Team. All Rights Reserved.

using System;
using System.IO;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Logging.Console;

namespace FlareCore;

public static class TextWriterExtensions
{
	public static void WriteWithColor(this TextWriter text_writer, string message, ConsoleColor? foreground, ConsoleColor? background = ConsoleColor.DarkGray)
	{
		string? background_color = background.HasValue ? GetBackgroundColorEscapeCode(background.Value) : null;
		string? foreground_color = foreground.HasValue ? GetForegroundColorEscapeCode(foreground.Value) : null;

		if (background_color != null) text_writer.Write(background_color);
		if (foreground_color != null) text_writer.Write(foreground_color);

		text_writer.WriteLine(message);

		if (foreground_color != null) text_writer.Write(default_foreground_color);
		if (background_color != null) text_writer.Write(default_foreground_color);
	}

	private static string GetForegroundColorEscapeCode(ConsoleColor color) =>
		color switch
		{
			ConsoleColor.Black		=> "\x1B[30m",
			ConsoleColor.DarkRed		=> "\x1B[31m",
			ConsoleColor.DarkGreen	=> "\x1B[32m",
			ConsoleColor.DarkYellow	=> "\x1B[33m",
			ConsoleColor.DarkBlue	=> "\x1B[34m",
			ConsoleColor.DarkMagenta	=> "\x1B[35m",
			ConsoleColor.DarkCyan	=> "\x1B[36m",
			ConsoleColor.Gray		=> "\x1B[37m",
			ConsoleColor.Red			=> "\x1B[1m\x1B[31m",
			ConsoleColor.Green		=> "\x1B[1m\x1B[32m",
			ConsoleColor.Yellow		=> "\x1B[1m\x1B[33m",
			ConsoleColor.Blue		=> "\x1B[1m\x1B[34m",
			ConsoleColor.Magenta		=> "\x1B[1m\x1B[35m",
			ConsoleColor.Cyan		=> "\x1B[1m\x1B[36m",
			ConsoleColor.White		=> "\x1B[1m\x1B[37m",
			_							=> default_foreground_color
		};

	private static string GetBackgroundColorEscapeCode(ConsoleColor color) => 
		color switch
		{
			ConsoleColor.Black		=> "\x1B[40m",
			ConsoleColor.DarkRed		=> "\x1B[41m",
			ConsoleColor.DarkGreen	=> "\x1B[42m",
			ConsoleColor.DarkYellow	=> "\x1B[43m",
			ConsoleColor.DarkBlue	=> "\x1B[44m",
			ConsoleColor.DarkMagenta	=> "\x1B[45m",
			ConsoleColor.DarkCyan	=> "\x1B[46m",
			ConsoleColor.Gray		=> "\x1B[47m",
			_							=> default_background_color
		};
	
	private const string default_foreground_color = "\x1B[39m\x1B[22m";
	private const string default_background_color = "\x1B[49m";
}

public class FlareConsoleFormatter() : ConsoleFormatter("Flare")
{
	public override void Write<TState>(in LogEntry<TState> log_entry, IExternalScopeProvider? scope_provider, TextWriter text_writer)
	{
		string message = log_entry.Formatter(log_entry.State, log_entry.Exception);
		
		if (!Utils.IsTextValid(message) && log_entry.Exception == null) return;
		if (log_entry.LogLevel < ParseLogVerbosityFromString(max_log_verbosity)) return;
		
		string timestamp = DateTime.Now.ToString("HH:mm:ss.f");
		
		string log_level = log_entry.LogLevel switch
		{
			LogLevel.Critical	=> "FATAL",
			LogLevel.Error		=> "ERROR",
			LogLevel.Warning	=> "WARN ",
			LogLevel.Debug		=> "DEBUG",
			LogLevel.Trace		=> "TRACE",
			_					=> "LOG  "
		};
		
		ConsoleColor color = log_entry.LogLevel switch
		{
			LogLevel.Critical	=> ConsoleColor.Red,
			LogLevel.Error		=> ConsoleColor.Red,
			LogLevel.Warning	=> ConsoleColor.Yellow,
			LogLevel.Debug		=> ConsoleColor.Blue,
			LogLevel.Trace		=> ConsoleColor.DarkBlue,
			_					=> ConsoleColor.Cyan
		};
		
		string category = log_entry.Category;

		text_writer.WriteWithColor($"[{timestamp}] {log_level} > {message}", color);
		if (log_entry.Exception != null)
		{
			text_writer.WriteLine(log_entry.Exception.ToString());
		}
	}
	
	/**
	 * <remarks> To capture CommandLineParser debug/trace logs you should manually specify it here. </remarks>
	 */
	[CommandLine("-Log?", description = "Specifies maximum allowed log verbosity to use. " +
										"Possible values are: None/Disabled, Fatal, Error, Warn/Warning, Info/Information, Debug, Trace and All.")]
	private static string max_log_verbosity = "Info";

	private static LogLevel ParseLogVerbosityFromString(string log_verbosity)
	{
		return log_verbosity switch
		{
			"All" or "Trace"		=> LogLevel.Trace,
			"Debug"					=> LogLevel.Debug,
			"Info" or "Information"	=> LogLevel.Information,
			"Warn" or "Warning"		=> LogLevel.Warning,
			"Error"					=> LogLevel.Error,
			"Fatal"					=> LogLevel.Critical,
			"None" or "Disabled"	=> LogLevel.None,
			
			_						=> LogLevel.Debug
		};
	}
}

public class Logger
{
	private Logger()
	{
		ILoggerFactory factory = LoggerFactory.Create(builder =>
		{
			builder.AddConsole(options => options.FormatterName = "Flare")
				.AddConsoleFormatter<FlareConsoleFormatter, ConsoleFormatterOptions>();
			
			builder.SetMinimumLevel(LogLevel.Trace); // max verbosity is handled manually in FlareConsoleFormatter.Write()
		});

		m_instance = factory.CreateLogger(Global.solution_name);
	}

	public static Logger Get() => singleton.Value;

	public void LogCritical(string message, params object[] args)
	{
		m_instance.LogCritical(message, args);
		Environment.Exit(1);
	}

	public void LogError(string message, params object[] args)
		=> m_instance.LogError(message, args);
	
	public void LogWarning(string message, params object[] args)
		=> m_instance.LogWarning(message, args);
	
	public void LogInformation(string message, params object[] args)
		=> m_instance.LogInformation(message, args);
	
	public void LogTrace(string message, params object[] args)
		=> m_instance.LogTrace(message, args);

	public void LogDebug(string message, params object[] args)
		=> m_instance.LogDebug(message, args);
	
	private	static readonly Lazy<Logger> singleton = new(() => new Logger());

	private readonly ILogger m_instance;
}
