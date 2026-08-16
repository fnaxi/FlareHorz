// CopyRight © FlareHorz Team. All Rights Reserved.

using System.Security.Cryptography;
using System.Text;
using FlareCore;
using NDepend.Path;

namespace FlareBuildTool.Configuration;

public abstract class FlareItem
{
	/** The name that uniquely identifies the item. */
	public string name = null!;
	
	/** The path to the root of this item. */
	public IAbsoluteDirectoryPath path = null!;
	
	/** A MSBuild file describing this item (.vcxproj, .csproj, etc.). */
	public IAbsoluteFilePath ms_file= null!;
	
	/** Global unique identifier representing this project in the solution file. */
	public Guid uid;

	/** Returns a type of this project (C++ project, C# project, etc.) */
	public abstract Guid GetMsType();

	/** Returns a display name of the item in lowercase ("module", "tool", etc.) */
	public abstract string GetDisplayName();

	protected static Guid GetUIDFromText(string path)
	{
		byte[] input_bytes = Encoding.UTF8.GetBytes(path);
		
		return new Guid( MD5.HashData(input_bytes) );
	}
}
