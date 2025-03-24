// CopyRight FlareHorz Team. All Rights Reserved.

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
	protected void Initialize(string Name)
	{
		FlareName = Name;
		OnObjectCreated();
	}
	
	/// <summary>
	/// Create object of set type. Can be used only with FFlareObject derived classes.
	/// </summary>
	public static T CreateObject<T>(string Name) where T : FFlareObject, new()
	{
		T Instance = new T();
		Instance.Initialize(Name);
		ObjectNames.Add(Name);
		return Instance; 
	}

	/// <summary>
	/// Get name of this object.
	/// </summary>
	public string GetName()
	{
		return FlareName;
	}
	
	/// <summary>
	/// Name of that object.
	/// </summary>
	private string FlareName;
	
	/// <summary>
	/// All objects of type FFlareObject.
	/// </summary>
	public static List<string> ObjectNames = new List<string>();

	/// <summary>
	/// Print all FFlareObject objects.
	/// </summary>
	public static void PrintObjectNames()
	{
		FLog.Debug("Objects:");
		for (int i = 0; i < ObjectNames.Count; i++)
		{
			FLog.Debug("[" + i + "]> " + ObjectNames[i]);
		}
	}

	/// <summary>
	/// Check is there object with same name.
	/// </summary>
	public static bool IsThereObjectWithSameName(string InName)
	{
		return ObjectNames.Find(s => s == InName) == InName;
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
