﻿// CopyRight FlareHorz Team. All Rights Reserved.

using System;
using System.Collections.Generic;
using FlareCore.Logger;

namespace FlareCore;

/// <summary>
/// Default class for all classes in FlareHorz C# codebase.
/// </summary>
[FlareCoreAPI]
public class FFlareObject
{
	/// <summary>
	/// Sets default values. Constructor is private to prevent direct instantiation
	/// </summary>
	protected FFlareObject() { }

	/// <summary>
	/// Cleanup everything.
	/// </summary>
	~FFlareObject()
	{
		OnObjectDestroyed();
	}

	/// <summary>
	/// Calls when object of that class is created.
	/// </summary>
	protected virtual void OnObjectCreated()
	{ }

	/// <summary>
	/// Calls when object is being destroyed.
	/// </summary>
	protected virtual void OnObjectDestroyed()
	{ }
	
	/// <summary>
	/// Method to perform initialization.
	/// </summary>
	public void Initialize(string Name)
	{
		FlareName = Name;
		ObjectNames.Add(Name);
		OnObjectCreated();
	}
	
	/// <summary>
	/// Create object of set type. Can be used only with FFlareObject derived classes.
	/// </summary>
	protected static T CreateObject<T>(string Name) where T : FFlareObject, new()
	{
		T Instance = new T();
		Instance.Initialize(Name);
		return Instance; 
	}

	/// <summary>
	/// Get name of this object.
	/// </summary>
	protected string GetName()
	{
		return FlareName;
	}
	
	/// <summary>
	/// Quote the text.
	/// </summary>
	protected string Quote(string InText)
	{
		return "\"" + InText + "\"";
	}
	
	/// <summary>
	/// Name of that object.
	/// </summary>
	private string FlareName;
	
	/// <summary>
	/// All objects of type FFlareObject.
	/// </summary>
	private static List<string> ObjectNames = new List<string>();

	/// <summary>
	/// Print all FFlareObject objects.
	/// </summary>
	public static void PrintObjectNames()
	{
		FLog.Debug("== Objects derived from FFlareObject == ");
		for (int i = 0; i < ObjectNames.Count; i++)
		{
			FLog.Debug("        [" + i + "]> " + ObjectNames[i]);
		}
		FLog.Debug("================= END ================= ");
	}
}


/// <summary>
/// Default class for all classes in FlareHorz C# codebase.
/// You should call Destroy() manually.
/// </summary>
[FlareCoreAPI]
public class FStaticFlareObject : FFlareObject, IDisposable
{
	/// <summary>
	/// Calls when object of that class is created.
	/// </summary>
	protected override void OnObjectCreated()
	{ }

	/// <summary>
	/// Calls when object is being destroyed.
	/// </summary>
	protected override void OnObjectDestroyed()
	{ }
	
	/// <summary>
	/// Performs cleanup of resources.
	/// Should be called when the object is no longer needed.
	/// </summary>
	public void Dispose()
	{
		OnObjectDestroyed();
		GC.SuppressFinalize(this);
	}
}
