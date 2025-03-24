// CopyRight FlareHorz Engine Development Team. All Rights Reserved.

namespace FlareCore;

/// <summary>
/// Attribute for indicating current API. Like in API macro in C++ codespace.
/// It only needed for easier code reading and making code style to know what API you're coding.
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
