// CopyRight © FlareHorz Team. All Rights Reserved.

namespace FlareCore;

public static class Utils
{
	/** Inverts IsNullOrEmpty() for cleaner checks. */
	public static bool IsTextValid(string? text)
	{
		return !string.IsNullOrEmpty(text);
	}
	
	public static List<T> Concat<T>(List<T> list1, List<T> list2)
	{
		return list1.Concat(list2).ToList();
	}
}
