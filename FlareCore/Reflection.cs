// CopyRight FlareHorz Team. All Rights Reserved.

using System.Diagnostics;
using System.Reflection;

namespace FlareCore;

/// <summary>
/// Reflection of FlareHorz C# code.
/// </summary>
public class FReflection : FFlareObject
{
	/// <summary>
	/// Get the API name from the stack trace by inspecting the calling class.
	/// </summary>
	/// <returns>The API name.</returns>
	public static string GetAPINameFromStackTrace()
	{
		// TODO: Null reference warning for StackFrame, Method and DeclaringType (can't use FAssert here cause FLog is uninitialized)
		
		StackTrace StackTrace = new StackTrace();
		StackFrame? StackFrame = StackTrace.GetFrame(2); // The method that called Log()
		MethodBase? Method = StackFrame.GetMethod();
		Type? DeclaringType = Method.DeclaringType;
		
		// Try to get the API from the calling class
		API? APITypeAttribute = (API)Attribute.GetCustomAttribute(DeclaringType, typeof(API));
		if (APITypeAttribute != null)
		{
			return APITypeAttribute.GetName();
		}
		
		// No attribute is found
		return "NoAPI";
	}
}
