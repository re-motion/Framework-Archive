using System;
using System.Text;
using System.Globalization;

namespace Rubicon
{

/// <summary>
/// Provides utility functions that make common string handling easier.
/// </summary>
public sealed class StringUtility
{
  // static members

  public static bool AreEqual (string strA, string strB)
  {
    return AreEqual (strA, strB, false);
  }

  public static bool AreEqual (string strA, string strB, bool ignoreCase)
  {
    return 0 == String.Compare (strA, strB, ignoreCase, CultureInfo.InvariantCulture);
  }

  public static string NullToEmpty (string str)
  {
    if (str == null)
      return string.Empty;
    return str;
  }

  public static bool IsNullOrEmpty (string str)
  {
    if (str == null || str.Length == 0)
      return true;
    return false;
  }

  public static string ConcatWithSeperator (string[] strings, string seperator)
  {
    StringBuilder sb = new StringBuilder ();
    foreach (string s in strings)
    {
      if (sb.Length > 0)
        sb.Append (seperator);
      sb.Append (s);
    }
    return sb.ToString();
  }

  public static string ConcatWithSeperator (object[] objects, string seperator)
  {
    return ConcatWithSeperator (ObjectArrayToStringArray (objects), seperator);
  }

  public static string[] ObjectArrayToStringArray (object[] objects)
  {
    if (objects == null)
      return null;

    string[] strings = new string[objects.Length];
    for (int i = 0; i < objects.Length; ++i)
      strings[i] = objects[i].ToString();
    return strings;
  }

  // construction and disposal
	private StringUtility()
	{
	}
}

}
