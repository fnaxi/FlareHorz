// CopyRight FlareHorz Team. All Rights Reserved.

using System.Diagnostics;
using System.IO;

namespace FlareCore;

public class PathUtils
{
	/** Converts a path to FlareHorz's normalized format by replacing all backslashes with forward slashes. */
	public static string ToFlare(string path) => path.Replace(@"\", "/");
	
	/** Converts a path to the native OS format by replacing all forward slashes with backslashes. */
	public static string ToNative(string path) => path.Replace("/", @"\");
	
	/** Similar to Directory.GetParent() but allows to set levels. */
	public static string GetParent(string path, int levels = 1)
	{
		Debug.Assert(levels >= 1, "Levels must be >= 1");

		DirectoryInfo? Info = new(path);
		for (int i = 0; i < levels; i++)
		{
			Debug.Assert(Info != null, "Invalid path!");
			Info = Info.Parent;
			Debug.Assert(Info != null, "Reached root directory before reaching specified level!");
		}

		return Info.FullName;
	}

	public static string RemoveStyling(string path)
		=> path.Replace('\\', '/').Replace('/', '_').Replace(':', '_').Replace('.', '_');
	
	public static string GetFileNameWithoutDoubleExtension(string path)
		=> Path.GetFileNameWithoutExtension(Path.GetFileNameWithoutExtension(path));
}
