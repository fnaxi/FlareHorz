// CopyRight FlareHorz Engine Development Team. All Rights Reserved.

namespace FlareCore;

/// <summary>
/// Attribute for indicating current API. Like in API macro in C++ codespace.
/// It only needed for easier code reading and making code style to know what API you're coding.
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Enum | AttributeTargets.Interface |
                AttributeTargets.Field | AttributeTargets.Method | AttributeTargets.Property)]
public class API : Attribute
{
	/// <summary>
	/// Sets default values.
	/// </summary>
	public API()
	{ }
	
	/// <summary>
	/// Get API string. Should be overrided.
	/// </summary>
	public virtual string GetName()
	{
		return "UnknownAPI";
	}
}

/// <summary>
/// Flare Build Tool API.
/// </summary>
public class FlareCoreAPI : API
{
	/// <summary>
	/// Sets default values.
	/// </summary>
	public FlareCoreAPI()
	{ }
	
	/// <summary>
	/// Get API string. Should be overrided.
	/// </summary>
	public override string GetName()
	{
		return "FlareCoreAPI";
	}
}
