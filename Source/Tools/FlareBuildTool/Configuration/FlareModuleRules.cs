// CopyRight © FlareHorz Team. All Rights Reserved.

namespace FlareBuildTool.Configuration;

public class FlareModuleRules
{
	public List<string> exposed_dependencies = new();
	public List<string> hidden_dependencies = new();
	
	public List<string> exposed_include_dirs = new();
	public List<string> hidden_include_dirs = new();

	public List<string> exposed_lib_dirs = new();
	public List<string> hidden_lib_dirs = new();
	
	public List<string> exposed_defines = new();
	public List<string> hidden_defines = new();
}
