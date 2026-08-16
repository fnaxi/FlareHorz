// CopyRight © FlareHorz Team. All Rights Reserved.

using FlareCore;
using NDepend.Path;

namespace FlareBuildTool.Configuration;

public class FlareTool : FlareItem
{
	public FlareTool(IAbsoluteFilePath csproj)
	{
		ms_file = csproj;
		
		name = ms_file.FileNameWithoutExtension;
		path = ms_file.ParentDirectoryPath;
		
		uid = GetUIDFromText(name);
		
		Logger.Get().LogInformation($"Mounted {name} tool with {ms_file.GetRelativePathFrom(Global.engine.tools_path)}");
	}
	
	public override Guid GetMsType() => Guid.Parse("9A19103F-16F7-4668-BE54-9A1E7A4F7556");
	public override string GetDisplayName() => "tool";
}
