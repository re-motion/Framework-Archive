using System;
using System.Text;
using System.Globalization;
using System.Collections;

namespace Rubicon.Utilities
{

/// <summary>
/// Provides utility functions that make common string handling easier.
/// </summary>
public sealed class StringUtility
{
  // static members

  /// <summary>
  /// Compares two strings using the invariant culture.
  /// </summary>
  public static bool AreEqual (string strA, string strB)
  {
    return AreEqual (strA, strB, false);
  }

  /// <summary>
  /// Compares two strings using the invariant culture.
  /// </summary>
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

  public static string EmptyToNull (string str)
  {
    if (str.Length == 0)
      return null;
    return str;
  }

  public static bool IsNullOrEmpty (string str)
  {
    if (str == null || str.Length == 0)
      return true;
    return false;
  }

//  public static string ConcatWithSeperator (string[] strings, string seperator)
//  {
//    StringBuilder sb = new StringBuilder ();
//    foreach (string s in strings)
//    {
//      if (sb.Length > 0)
//        sb.Append (seperator);
//      sb.Append (s);
//    }
//    return sb.ToString();
//  }
//
//  public static string ConcatWithSeperator (object[] objects, string seperator)
//  {
//    return ConcatWithSeperator (ObjectArrayToStringArray (objects), seperator);
//  }

  public static string[] ListToStringArray (IList list)
  {
    if (list == null)
      return null;

    string[] strings = new string[list.Count];
    for (int i = 0; i < list.Count; ++i)
      strings[i] = list[i].ToString();
    return strings;
  }

  // construction and disposal
	private StringUtility()
	{
	}

  public static string ConcatWithSeperator (IList list, string seperator)
	{
		return ConcatWithSeperator (list, seperator, null, null);
	}

	public static string ConcatWithSeperator (IList list, string seperator, string format, IFormatProvider formatProvider)
	{
		if (list == null)
			throw new ArgumentNullException ("list");

    if (list.Count == 0)
			return string.Empty;

		StringBuilder sb = new StringBuilder ();
		for (int i = 0; i < list.Count; ++i)
		{
			if (i > 0)
				sb.Append (seperator);

			object obj = list[i];
			IFormattable formatable = obj as IFormattable;
			if (formatable != null)
				sb.Append (formatable.ToString (format, formatProvider));
			else
				sb.Append (obj.ToString());
		}
		return sb.ToString();
	}

	public static string ConcatWithSeperator (string[] strings, string seperator)
	{
		if (strings == null)
			throw new ArgumentNullException ("strings");
		if (strings.Length == 0)
			return string.Empty;

		StringBuilder sb = new StringBuilder ();
		for (int i = 0; i < strings.Length; ++i)
		{
			if (i > 0)
				sb.Append (seperator);
			sb.Append (strings[i]);
		}
		return sb.ToString();
	}

	public static string Concat (Array array)
	{
		StringBuilder sb = new StringBuilder();
		for (int i = 0; i < array.Length; ++i)
			sb.Append (array.GetValue (i));
		return sb.ToString();
	}

}

}
