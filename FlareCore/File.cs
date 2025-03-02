// CopyRight FlareHorz Engine Development Team. All Rights Reserved.

namespace FlareCore;

/// <summary>
/// Provides methods for creating, deleting, and manipulating files.
/// </summary>
[FlareCoreAPI]
public class FFile : FFlareObject
{
	/// <summary>
	/// Sets default values.
	/// </summary>
	public FFile()
	{
	}
	
	/// <summary>
	/// Name of this file.
	/// </summary>
	public string FileName;

	/// <summary>
	/// Path to this file.
	/// </summary>
	public string FilePath;
	
	/// <summary>
	/// Source of this premake file.
	/// </summary>
	private StreamWriter FileSource;
	
	/// <summary>
	/// Is this file deleted. True when you didn't created or loaded a file.
	/// </summary>
	private bool bDeletedOrUnloaded = true;
	
	/// <summary>
	/// Get parent path.
	/// </summary>
	public static string GetParent(string InPath, int HowManyTimes = 1)
	{
		FAssert.Check(HowManyTimes >= 1);
		
		// Get parent directory
		DirectoryInfo? Info = new DirectoryInfo(InPath);
		while (HowManyTimes != 0)
		{
			Info = Info.Parent;

			// If Info is null, the parent doesn't exist.
			FAssert.Checkf(Info != null, "Can't get parent path!");

			HowManyTimes--;
		}
		
		return Info?.FullName ?? string.Empty;
	}
	
	/// <summary>
	/// Close this file.
	/// </summary>
	public void Close()
	{
		FileSource.Close();
		bDeletedOrUnloaded = true;
	}

	/// <summary>
	/// Delete this file.
	/// </summary>
	public void Delete()
	{
		FAssert.Verifyf(bDeletedOrUnloaded, "File is already deleted or not loaded yet!");
		
		File.Delete(FilePath);
		bDeletedOrUnloaded = true;
	}
	
	/// <summary>
	/// Create file in specified location with specified name.
	/// File path should be with file name.
	/// </summary>
	public void CreateFile(string InFilePath, string InFileName)
	{
		// Should have these values to create file
		FAssert.Check(InFileName != "" && InFilePath != "");
		
		// Set file properties
		FileName = InFileName;
		FilePath = InFilePath;
		
		// Create file
		FAssert.Checkf(!File.Exists(InFilePath), "File is already exist!");
		
		FileSource = new StreamWriter(InFilePath);
		bDeletedOrUnloaded = false;
	}
	
	/// <summary>
	/// Load file by location.
	/// </summary>
	public void LoadFile(string InFilePath)
	{
		// Should have these values to create file
		FAssert.Check(InFilePath != "");
		
		// Set file properties
		FileName = InFilePath.Split('\\').Last();
		FilePath = InFilePath;
		
		// Load file
		FAssert.Checkf(File.Exists(InFilePath), "File doesn't exist!");
		
		FileSource = new StreamWriter(InFilePath, append: true); // Does not delete old text in the file, you can manually clear it if you need that
		bDeletedOrUnloaded = false;
	}
	
	/// <summary>
	/// Clear whole text that file have.
	/// <remarks>IMPORTANT: Be sure to update file path if you changed it</remarks>
	/// </summary>
	public void ClearFile(string Text)
	{
		FAssert.Checkf(!bDeletedOrUnloaded, "File is not loaded or deleted!");
		
		// Close our file source
		FileSource.Close();
		
		// Create temp one to delete everything
		StreamWriter TempSource = new StreamWriter(FilePath);
		TempSource.Close();
		
		// Turn back to our file source
		FileSource = new StreamWriter(FilePath, append: true);
	}
	
	/// <summary>
	/// Write something to source of that file.
	/// </summary>
	public void WriteSrc(string Text)
	{
		FAssert.Checkf(!bDeletedOrUnloaded, "File is not loaded or deleted!");
		FileSource.WriteLine(Text);
	}
	public void WriteSrcInline(string Text)
	{
		FAssert.Checkf(!bDeletedOrUnloaded, "File is not loaded or deleted!");
		FileSource.Write(Text);
	}

	/// <summary>
	/// Find all directories in path.
	/// </summary>
	public static string[] GetDirectories(string InPath)
	{
		return Directory.GetDirectories(InPath);
	}
	
	/// <summary>
	/// Check is file exists.
	/// </summary>
	public static bool IsFileExists(string InFilePath)
	{
		return File.Exists(InFilePath);
	}
	
	/// <summary>
	/// Check is directory exists.
	/// </summary>
	public static bool IsDirectoryExists(string InDirectoryPath)
	{
		return Directory.Exists(InDirectoryPath);
	}

	/// <summary>
	/// Create directory.
	/// </summary>
	public static void CreateDirectory(string InDirectoryPath)
	{
		if (!FFile.IsDirectoryExists(InDirectoryPath))
		{
			Directory.CreateDirectory(InDirectoryPath);
		}
	}
	
	/// <summary>
	/// Get current directory.
	/// </summary>
	public static string GetCurrentDirectory()
	{
		return Directory.GetCurrentDirectory();
	}
	
	/// <summary>
	/// Combine two different paths.
	/// </summary>
	public static string Combine(string InPath1, string InPath2)
	{
		return Path.Combine(InPath1, InPath2);
	}
	public static string Combine(string[] InPaths)
	{
		return Path.Combine(InPaths);
	}
}
