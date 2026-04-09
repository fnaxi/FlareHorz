// CopyRight FlareHorz Team. All Rights Reserved.

using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.IO;

namespace FlareCore;

public static class CPath
{
	/** Converts a path to FlareHorz's normalized format by replacing all backslashes with forward slashes. */
	public static string ToFlare(string InPath)
	{
		return InPath.Replace(@"\", "/");
	}
	
	/** Converts a path to the native OS format by replacing all forward slashes with backslashes. */
	public static string ToNative(string InPath)
	{
		return InPath.Replace("/", @"\");
	}
	
	/** Similar to Directory.GetParent() but allows to set levels. */
	public static string GetParent(string InPath, int Levels = 1)
	{
		Verify(Levels >= 1, "Levels must be >= 1");

		DirectoryInfo Info = new(InPath);
		for (int i = 0; i < Levels; i++)
		{
			Verify(Info != null, "Invalid path!");
			Info = Info.Parent;
			Verify(Info != null, "Reached root directory before reaching specified level!");
		}

		return Info.FullName;
	}

	public static string Combine(string InPath1, string InPath2) { return Path.Combine(InPath1, InPath2); }
	public static string Combine(string InPath1, string InPath2, string InPath3) { return Path.Combine(InPath1, InPath2, InPath3); }
	public static string Combine(string InPath1, string InPath2, string InPath3, string InPath4) { return Path.Combine(InPath1, InPath2, InPath3, InPath4); }
	public static string Combine(List<string> InPaths) { return Path.Combine(InPaths.ToArray()); }
	
	public static string FlareCombine(string InPath1, string InPath2) { return ToFlare(Combine(InPath1, InPath2)); }
	public static string FlareCombine(string InPath1, string InPath2, string InPath3) { return ToFlare(Combine(InPath1, InPath2, InPath3)); }
	public static string FlareCombine(string InPath1, string InPath2, string InPath3, string InPath4) { return ToFlare(Combine(InPath1, InPath2, InPath3, InPath4)); }
	public static string FlareCombine(List<string> InPaths) { return ToFlare(Combine(InPaths)); }

	public static string GetFileNameWithoutDoubleExtension(string InPath)
	{
		return Path.GetFileNameWithoutExtension(Path.GetFileNameWithoutExtension(InPath));
	}
}

/**
 * Provides file creation, deletion, loading, and manipulation utilities.
 */
public class CFileHandle
{
	public CFileHandle()
	{
		Source = null;
	}
	public CFileHandle(string InFilePath)
	{
		Setup(InFilePath);
	}
	
	/** Indicates false when the file is deleted or not loaded. */
	public bool bOpened => Source != null;
	
	/** The name of the file. */
	public string Name { get; private set; }

	/** The path to the file excluding file name. */
	public string Path { get; set; }
	
	/** Set information about a path for the file. */
	public void Setup(string InPath)
	{
		Load(InPath);
		Close();
	}
	
	/** Creates a new file at the specified path. */
	public void Create(string InPath, string InName)
	{
		Name = InName;
		Path = InPath;
		
		Verify(!InPath.EndsWith(InName), "InPath shouldn't end with file name!");
		Verify(!File.Exists(GetFullPath()), "File already exists!");

		Source = new StreamWriter(GetFullPath());
	}
	
	/** Loads an existing file for writing. */
	public void Load(string InPath)
	{
		Verify(IsTextValid(InPath), "File path is not valid!");

		Name = System.IO.Path.GetFileName(InPath);
		Path = System.IO.Path.GetDirectoryName(InPath);
		
		Verify(File.Exists(GetFullPath()), $"File does not exist: {InPath}!");
		Source = new StreamWriter(GetFullPath(), append: true);
	}
	
	/** Load a file if it exists otherwise, create and then load it. */
	public void CreateOrLoad(string InFilePath)
	{
		if (File.Exists(InFilePath))
		{
			Load(InFilePath);
		}
		else
		{
			Create(System.IO.Path.GetDirectoryName(InFilePath), System.IO.Path.GetFileName(InFilePath));
		}
	}
	
	/** Writes text to the file. */
	public void Write(string Text, bool bInline = false)
	{
		Verify(bOpened, "File is not loaded!");
		if (bInline)
		{
			Source.Write(Text);
		}
		else
		{
			Source.WriteLine(Text);
		}
	}
	public void WriteEmptyLine() { Write(""); }
	
	/** Clears all content from the file. */
	public void Clear()
	{
		Verify(bOpened, "File is not loaded!");

		Source.Close();
		File.WriteAllText(GetFullPath(), string.Empty);
		
		Source = new StreamWriter(GetFullPath(), append: true);
	}

	/** Closes the file stream. */
	public void Close()
	{
		if (bOpened)
		{
			Source.Close();
			Source = null;
		}
		else
		{
			CLog.Warning("Tried to close unloaded file!");
		}
	}

	/** Deletes the file from disk. */
	public void Delete()
	{
		Verify(bOpened, "Cannot delete unloaded file!");
		Source.Close();
		
		if (File.Exists(GetFullPath()))
		{
			File.Delete(GetFullPath());
			Close();
		}
		else
		{
			CLog.Error("Can't delete file because it doesn't exist!");
		}
	}
	
	/** Get a full path to the file including file name. */
	[Pure] public string GetFullPath()
	{
		Verify(IsTextValid(Path) && IsTextValid(Name));
		
		return System.IO.Path.Combine(Path, Name);
	}

	// ~IDisposable interface
	public void Dispose() { Close(); }
	// ~IDisposable interface end
	
	private StreamWriter Source;
}
