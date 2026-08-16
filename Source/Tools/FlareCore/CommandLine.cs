// CopyRight FlareHorz Team. All Rights Reserved.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

namespace FlareCore;

/**
 * Static variable marked with this attribute will be set via command-line arguments.
 */
[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = true)]
public sealed class CommandLineAttribute : Attribute
{
	public CommandLineAttribute(string? in_prefix = null)
	{
		prefix = in_prefix;
		if (prefix != null)
		{
			Debug.Assert(prefix.StartsWith('-'));
			Debug.Assert(!prefix.Contains('='));
		}
	}

	/** Prefix for the option, with a leading '-' and trailing '?' character if a value is expected. */
	public string? prefix { get; set; } = null;
	
	/** Specifies a fixed value for this argument. Specifying an alternate value is not permitted. */
	public string? value { get; set; } = null;
	
	public string? description { get; set; } = null;
}

/**
 * Class that handles command line arguments by setting needed values for fields with <see cref="CommandLineAttribute"/> attribute.
 */
public class CommandLineParser
{
	public CommandLineParser(string[] InArguments)
	{
		arguments = InArguments.ToList();
	}
	
	/** Process arguments for all properties/fields with <see cref="CommandLineAttribute"/> attribute. */
	public void ProcessCmdArguments()
	{
		List<FieldInfo> fields = GetCommandLineFields();
		foreach (FieldInfo Field in fields)
		{
			ProcessCommandLineField(Field);
		}
	}
	
	/** Show all available command line flags. */
	public void ShowHelp()
	{
		Console.WriteLine("  Showing help...");
		
		int longest_prefix = 0;
		foreach (FieldInfo field in GetCommandLineFields())
		{
			List<CommandLineAttribute> attributes = field.GetCustomAttributes<CommandLineAttribute>().ToList();
			foreach (CommandLineAttribute cmd in attributes)
			{
				if (cmd.prefix != null && cmd.description != null)
				{
					longest_prefix = cmd.prefix.Length > longest_prefix ? cmd.prefix.Length : longest_prefix;
				}
			}
		}
		
		foreach (FieldInfo field in GetCommandLineFields())
		{
			List<CommandLineAttribute> attributes = field.GetCustomAttributes<CommandLineAttribute>().ToList();
			foreach (CommandLineAttribute cmd in attributes)
			{
				if (cmd.prefix != null && cmd.description != null)
				{
					Console.WriteLine($"  {cmd.prefix.PadRight(longest_prefix)} :  {cmd.description}");
				}
			}
		}
	}
	
	/** Get all fields marked with <see cref="CommandLineAttribute"/> attributes that are defined in application. */
	private static List<FieldInfo> GetCommandLineFields()
	{
		return Application.GetAllTypes()
			// Instance fields are invalid, but we want to make an assert to tell user what's wrong
			.SelectMany(t => t.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance))
			.Where(f => f.IsDefined(typeof(CommandLineAttribute), false))
			.ToList();
	}
	
	/** Attempts to process a field marked with the <see cref="CommandLineAttribute"/> attribute. */
	private void ProcessCommandLineField(FieldInfo field)
	{
		Debug.Assert(field.IsStatic, $"Missing 'static' keyword for {field.Name} field marked with CommandLineAttribute!");
		
		List<CommandLineAttribute> attributes = field.GetCustomAttributes<CommandLineAttribute>().ToList();
		foreach (CommandLineAttribute cmd in attributes)
		{
			Debug.Assert(cmd.prefix != null);

			if (cmd.prefix.Contains('?')) // param
			{
				string? argument = arguments.Find(x => x.StartsWith(cmd.prefix));
				if (argument != null)
				{
					string? value = argument.Split('?')[1];
					Debug.Assert(value != null, $"Invalid value passed to {cmd.prefix} argument!");

					if (IsPassed(argument))
					{
						field.SetValue(null, value);

						Logger.Get().LogTrace($"Param {cmd.prefix} was set to {value}");
					}
				}
			}
			else
			{
				CommandLineAttribute? param = attributes.FirstOrDefault(x => x.prefix != null && x.prefix.Contains('?'));
				if (param != null && cmd.value != null) // param alias
				{
					if (IsPassed(cmd.prefix))
					{
						field.SetValue(null, cmd.value);

						Logger.Get().LogTrace($"Param alias {cmd.prefix} was passed");
					}
				}
				else // flag
				{
					if (IsPassed(cmd.prefix))
					{
						field.SetValue(null, true);
						Logger.Get().LogTrace($"Flag {cmd.prefix} was passed");
					}
				}
			}
		}
	}

	/** Determines whatever command-line flag was passed or no. */
	private bool IsPassed(string argument) => arguments.Contains(argument);
	
	/** Command-line arguments that were passed to the application. */
	private readonly List<string> arguments;
}
