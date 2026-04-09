// CopyRight FlareHorz Team. All Rights Reserved.

using System;
using System.Collections.Generic;
using System.Linq;

namespace FlareCore;

public static class CUtils
{
	/** Inverts String.IsNullOrWhiteSpace for cleaner checks. */
	public static bool IsTextValid(string InText)
	{
		return !String.IsNullOrWhiteSpace(InText);
	}
	
	public static string LuaQuote(string InText)
	{
		return "\\" + "\"" + InText + "\\" + "\"";
	}

	public static List<T> Concat<T>(List<T> InList1, List<T> InList2)
	{
		return InList1.Concat(InList2).ToList();
	}
}
