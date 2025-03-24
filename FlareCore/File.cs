// CopyRight FlareHorz Engine Development Team. All Rights Reserved.

namespace FlareCore;

/// <summary>
/// Provides methods for creating, deleting, and manipulating files.
/// </summary>
[FlareCoreAPI]
public class FFile : FFlareObject
{
	/// <summary>
	/// Name of this file.
	/// </summary>
	public string FileName { get; private set; }

	/// <summary>
	/// Path to this file.
	/// </summary>
	public string FilePath { get; private set; }
	
	/// <summary>
	/// Source of this premake file.
	/// </summary>
	private StreamWriter FileSource;
	
	/// <summary>
	/// Is this file deleted. True when you didn't create or loaded a file.
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
		
		FileSource = new StreamWriter(InFilePath, append: true); // Use ClearFile
		bDeletedOrUnloaded = false;
	}
	
	/// <summary>
	/// Clear whole text that file have.
	/// </summary>
	public void ClearFile()
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
	/// Check is file exists.
	/// </summary>
	public bool IsFileExists()
	{
		return File.Exists(FilePath);
	}
	
	/// <summary>
	/// Write something to source of that file.
	/// </summary>
	public void WriteSrc(string Text, bool bInline = false)
	{
		FAssert.Checkf(!bDeletedOrUnloaded, "File is unloaded or deleted!");
		switch (bInline)
		{
			case true: 
				FileSource.Write(Text); break;
			case false:
				FileSource.WriteLine(Text); break;
		}
	}
}
