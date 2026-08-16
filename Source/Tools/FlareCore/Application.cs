// CopyRight FlareHorz Team. All Rights Reserved.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

namespace FlareCore;

public abstract class Application
{
	public static Assembly? target_assembly { get; private set; }
	public static Assembly? core_assembly { get; private set; }

	/**
	 * Entry-point of the application.
	 * <param name="arguments"> Command-line arguments the application was run with. </param>
	 * <param name="in_target_assembly"> Pass info about executable assembly to parse metadata such as CmdParam or CmdFlag attributes. </param>
	 */
	public int Run(string[] arguments, Assembly in_target_assembly)
	{
		Initialize(arguments, in_target_assembly);
		if (b_show_help || arguments.Length == 0)
		{
			cmd_parser.ShowHelp();
			return 0;
		}
		
		int exit_result = GuardedMain();
		Logger.Get().LogInformation($"Exited with code {exit_result}");

		Shutdown();
		return exit_result;
	}
	
	public static List<Type> GetAllTypes()
	{
		List<Type> types = [];
		
		Debug.Assert(target_assembly != null);
		types.AddRange( target_assembly.GetTypes().ToList() );
		
		Debug.Assert(core_assembly != null);
		types.AddRange( core_assembly.GetTypes().ToList() );

		return types;
	}
	
	/** Executes console command or an executable. */
	public static int ExecuteConsoleCommand(string command)
	{
		Debug.Assert(Utils.IsTextValid(command));
		
		Logger.Get().LogInformation($"Executing command: {command}");
		ProcessStartInfo process_info = new ProcessStartInfo("cmd.exe", "/c" + command)
		{
			CreateNoWindow = true,
			UseShellExecute = false,
			RedirectStandardError = true,
			RedirectStandardOutput = true
		};

		Process? process = Process.Start(process_info);
		Debug.Assert(process != null, $"Process is null. Can't execute command {command}");
		
		// Log output
		process.OutputDataReceived += (_, e) =>
		{
			if (Utils.IsTextValid(e.Data)) Logger.Get().LogInformation($"Output: {e.Data}");
		};
		process.BeginOutputReadLine();

		// Log any errors
		process.ErrorDataReceived += (_, e) =>
		{
			if (Utils.IsTextValid(e.Data)) Logger.Get().LogInformation($"Error: {e.Data}");
		};
		process.BeginErrorReadLine();
		
		process.WaitForExit();
		int exit_code = process.ExitCode;
		
		process.Close();

		return exit_code;
	}
	
	/**
	 * Core method where the main application logic should be implemented.
	 * <returns> Exit result code. </returns>
	 */
	protected abstract int GuardedMain();
	
	[CommandLine("-Help", description = "Display this help.")]
	[CommandLine("-h")]
	[CommandLine("--help")]
	private static bool b_show_help = false;
	
	private CommandLineParser cmd_parser { get; set; } = null!;

	/** Initializes the application by processing command-line arguments, setting up the necessary paths, and initializing logger. */
	private void Initialize(string[] arguments, Assembly in_target_assembly)
	{
		target_assembly = in_target_assembly;
		Debug.Assert(target_assembly != null, "Target assembly is null!");
		
		core_assembly = Assembly.GetExecutingAssembly();
		Debug.Assert(core_assembly != null, "FlareCore assembly is null!");
		
		// Logger.Get().LogInformation($"Working directory: {Global.engine.root_path.Get()}");
		
		cmd_parser = new CommandLineParser(arguments);
		cmd_parser.ProcessCmdArguments();
	}
	
	/** Shutdown the application. */
	private void Shutdown() { }
}
