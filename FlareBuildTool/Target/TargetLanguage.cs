// CopyRight FlareHorz Team. All Rights Reserved.

namespace FlareBuildTool.Target;

/// <summary>
/// Language on which this target written.
/// </summary>
[FlareBuildToolAPI]
public enum ETargetLanguage
{
	None,
	
	/// <summary>
	/// Use C++ as target language.
	/// </summary>
	CPP,
	
	/// <summary>
	/// Use C# as target language.
	/// </summary>
	CS
}
