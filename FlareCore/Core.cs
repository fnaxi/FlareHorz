// CopyRight FlareHorz Engine Development Team. All Rights Reserved.

namespace FlareCore;

/// <summary>
/// Attribute for indicating current API in logger. Like API macro in C++ codespace.
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Enum | AttributeTargets.Interface)]
public class API : Attribute
{
	/// <summary>
	/// Get API string.
	/// </summary>
	public virtual string GetName()
	{
		return "NoAPI";
	}
}
