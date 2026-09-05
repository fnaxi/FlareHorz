// CopyRight © FlareHorz Team. All Rights Reserved.

using FlareCore;

namespace FlareBuildTool.Configuration;

public class FlareRule
{
	public List<string> exposed = new();
	public List<string> hidden = new();
	public List<string> Get() => Utils.Concat(exposed, hidden);
}

public class FlareModuleRules
{
	public FlareRule dependencies = new();
	public FlareRule include_dirs = new();
	public FlareRule lib_dirs = new();
	public FlareRule defines = new();
}
