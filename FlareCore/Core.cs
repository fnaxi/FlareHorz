// CopyRight FlareHorz Engine Development Team. All Rights Reserved.

using System;

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

/// <summary>
/// Attribute for indicating current method in logger.
/// </summary>
[AttributeUsage(AttributeTargets.Method)]
public class FlareFunc : Attribute
{
	/// <summary>
	/// Sets default values.
	/// </summary>
	public FlareFunc(string InName)
	{
		Name = InName;
	}
	
	/// <summary>
	/// Name of this function.
	/// </summary>
	private string Name;
	
	/// <summary>
	/// Get API string.
	/// </summary>
	public string GetName()
	{
		return Name;
	}
}
