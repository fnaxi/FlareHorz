// CopyRight FlareHorz Team. All Rights Reserved.

using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using FlareCore.Logger;

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
	public static string GetAPINameFromStackTrace(bool bLVMethods)
	{
		StackTrace StackTrace = new StackTrace();
		StackFrame StackFrame = StackTrace.GetFrame(bLVMethods ? 3 : 2); // The method that called Log()
		MethodBase Method = StackFrame.GetMethod();
		Type DeclaringType = Method.DeclaringType;
		if (DeclaringType == null)
		{
			FLog.FatalUninitializedLog("Can't get API name because method that called log function is not valid!");
		}
		
		// Try to get the API from the calling class
		API[] APITypeAttributes = (API[])Attribute.GetCustomAttributes(DeclaringType, typeof(API));
		if (APITypeAttributes.Length == 0)
		{
			// No attribute is found
			return "NoAPI";
		}
		
		return APITypeAttributes[0].GetName();
	}
}
