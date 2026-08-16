// CopyRight FlareHorz Team. All Rights Reserved.

using System.Diagnostics;
using System.IO;
using System.Text;
using NDepend.Path;

namespace FlareCore;

public static class PathExtensions
{
	/*----------------------------------------------------------------------------
		IAbsoluteFilePath / IRelativeFilePath
	----------------------------------------------------------------------------*/
	
	/**
	 * Creates a new StreamWriter instance for this file.
	 * <param name="b_append"> True to append data to the file; false to overwrite the file. </param>
	 */
	public static StreamWriter CreateWriter(this IAbsoluteFilePath file_path, bool b_append = false)
	{
		return new StreamWriter(file_path.FileInfo.FullName, b_append, Encoding.UTF8);
	}
	
	public static string Get(this IAbsoluteFilePath file_path)
	{
		string? path = file_path.ToString();
		Debug.Assert(path != null);
		return path;
	}
	
	public static string Get(this IRelativeFilePath file_path)
	{
		string? path = file_path.ToString();
		Debug.Assert(path != null);
		return path;
	}
	
	/*----------------------------------------------------------------------------
		IAbsoluteDirectoryPath / IRelativeDirectoryPath
	----------------------------------------------------------------------------*/

	public static string Get(this IAbsoluteDirectoryPath dir_path)
	{
		string? path = dir_path.ToString();
		Debug.Assert(path != null);
		return path;
	}
	
	public static string Get(this IRelativeDirectoryPath dir_path)
	{
		string? path = dir_path.ToString();
		Debug.Assert(path != null);
		return path;
	}
}
