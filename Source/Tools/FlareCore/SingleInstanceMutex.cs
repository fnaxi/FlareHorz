// CopyRight © FlareHorz Team. All Rights Reserved.

using System;
using System.Threading;
using NDepend.Path;

namespace FlareCore;

public class SingleInstanceMutex : IDisposable
{
	/** The global mutex instance. */
	private Mutex? mutex;

	/**
	 * Constructor. Attempts to acquire the global mutex.
	 * <param name="mutex_name"> Name of the mutex to acquire </param>
	 * <param name="b_wait_mutex"> Allow waiting for the mutex to be acquired. </param>
	 */
	public SingleInstanceMutex(string mutex_name, bool b_wait_mutex)
	{
		// Try to create the mutex, with it initially locked
		mutex = new Mutex(true, mutex_name, out bool b_created_mutex);
		
		if (b_created_mutex)
		{
			Logger.Get().LogTrace($"Created mutex: {mutex_name}");
			return;
		}
		
		// If we didn't create the mutex, we can wait for it or fail immediately
		if (b_wait_mutex)
		{
			Logger.Get().LogInformation("Waiting for mutex to free...");
			try
			{
				mutex.WaitOne();
			}
			catch (AbandonedMutexException)
			{
				Logger.Get().LogInformation("Mutex acquired");
			}
		}
		else
		{
			Logger.Get().LogCritical($"A conflicting instance of {mutex_name} is already running!");
		}
	}
	
	/** Release the mutex and dispose of the object. */
	public void Dispose()
	{
		mutex?.ReleaseMutex();
		mutex?.Dispose();
		mutex = null;
		
		GC.SuppressFinalize(this);
	}
	
	/**
	 * Gets the name of a mutex unique for the given path.
	 *
	 * <param name="name">Base name of the mutex</param>
	 * <param name="unique_path">Path to identify a unique mutex</param>
	 */
	public static string GetUniqueMutexForPath(string name, string unique_path)
	{
		// todo: Use stable hash here
		return $"Global\\{name}_{PathUtils.RemoveStyling(unique_path.ToUpperInvariant())}";
	}
	
	/** <inheritdoc cref="GetUniqueMutexForPath(string,string)"/> */
	public static string GetUniqueMutexForPath(string name, IAbsoluteDirectoryPath unique_path) => GetUniqueMutexForPath(name, unique_path.Get());
}
