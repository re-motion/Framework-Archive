using System;
using System.Text;
using System.Globalization;
using System.Runtime.Serialization;
using System.Reflection;
using System.Collections;
using Rubicon.Collections;

namespace Rubicon.Utilities
{

/// <summary>
/// Provides utility functions that make common string handling easier.
/// </summary>
public sealed class StringUtility
{
  /// <summary>
  ///   The result of <see cref="ParseSeparatedList"/>.
  /// </summary>
  public class ParsedItem
  {
    public ParsedItem (string value, bool isQuoted)
    {
      Value = value;
      IsQuoted = isQuoted;
    }

    /// <summary> The string value at this position. </summary>
    public string Value;

    /// <summary> An indicator that is <c>true</c> if the string at this position was quoted in the input string. </summary>
    public bool IsQuoted;
  }

  private static Hashtable s_parseMethods = new Hashtable();

  public static ParsedItem[] ParseSeparatedList (string value, char delimiter)
  {
    return ParseSeparatedList (value, delimiter, '\"', '\"', '\\', " ", true);
  }

  public static ParsedItem[] ParseSeparatedList (string value)
  {
    return ParseSeparatedList (value, ',', '\"', '\"', '\\', " ", true);
  }

  /// <summary>
  ///   Parses a delimiter-separated string into individual elements.
  /// </summary>
  /// <remarks>
  ///   This method handles quotes and escaping. A quoted string may contain commas that
  ///   will not be treated as separators. Commas prefixed with a backslash are treated
  ///   like normal commas, double backslashes are treated as single backslashes.
  /// </remarks>
  /// <param name="value"> The string to be parsed. </param>
  /// <param name="delimiter"> The character used for list separation. Default is comma (,). </param>
  /// <param name="openingQuote"> The character used as opening quote. Default is double quote (&quot;). </param>
  /// <param name="closingQuote"> The character used as closing quote. Default is double quote (&quot;). </param>
  /// <param name="escapingChar"> The character used to escape quotes and itself. Default is backslash (\). </param>
  /// <param name="whitespaceCharacters"> A string containing all characters to be considered whitespace. 
  ///   Default is space character only. </param>
  /// <param name="interpretSpecialCharacters"> If true, the escaping character can be followed by the letters
  ///   r, n or t (case sensitive) for line feeds, new lines or tab characters, respectively. Default is true. </param>
  public static ParsedItem[] ParseSeparatedList (
      string value, 
      char delimiter, char openingQuote, char closingQuote, char escapingChar, string whitespaceCharacters, 
      bool interpretSpecialCharacters) 
  {
    string specialCharacters = "rnt";
    string specialCharacterResults = "\r\n\t";

    StringBuilder current = new StringBuilder();
    TypedArrayList items = new TypedArrayList (typeof (ParsedItem));
    // ArrayList argsArray = new ArrayList();

    bool isQuoted = false;
    // ArrayList isQuotedArray = new ArrayList();

    int len = value.Length;
    int state = 0; // 0 ... between arguments, 1 ... within argument, 2 ... within quotes

    for (int i = 0; i < len; ++i)
    {
      char c = value[i];
      if (state == 0)
      {
        if (c == openingQuote)
        {
          state = 2;
          isQuoted = true;
        }
        else if (c != delimiter && whitespaceCharacters.IndexOf (c) < 0)
        {
          state = 1;
          current.Append (c);
        }
      }
      else if (state == 1)
      {
        if (c == openingQuote)
        {
          // the string started without quotes, but enters a quoted area now. isQuoted is still false!
          state = 2;
          current.Append (c);
        }
        else if (c == delimiter)
        {
          state = 0;
          if (current.Length > 0)
          {
            items.Add (new ParsedItem (current.ToString(), isQuoted));
            current.Length = 0;
            isQuoted = false;
          }
        }
        else 
        {
          current.Append (c);
        }
      }
      else if (state == 2)
      {
        if (c == closingQuote)
        {
          if (! isQuoted)
            current.Append (c);
          state = 1;
        }
        else if (c == escapingChar)
        {
          if ((i + 1) < len)
          {
            char next = value[i+1];
            if (next == escapingChar || next == openingQuote || next == closingQuote)
            {
              current.Append (next);
              ++i;
            }
            else if (interpretSpecialCharacters && specialCharacters.IndexOf (next) >= 0)
            {
              current.Append (specialCharacterResults[specialCharacters.IndexOf (next)]);
              ++i;
            }
            else
            {
              current.Append ('\\');
            }
          }
          else
          {
            state = 1;
          }
        }
        else 
        {
          current.Append (c);
        }
      }
    }

    if (current.Length > 0)
    {
      items.Add (new ParsedItem (current.ToString(), isQuoted));
    }
    return (ParsedItem[]) items.ToArray();
  }
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
    if (str != null && str.Length == 0)
      return null;
    return str;
  }

  public static bool IsNullOrEmpty (string str)
  {
    if (str == null || str.Length == 0)
      return true;
    return false;
  }

  public static string[] ListToStringArray (IList list)
  {
    if (list == null)
      return null;

    string[] strings = new string[list.Count];
    for (int i = 0; i < list.Count; ++i)
      strings[i] = list[i].ToString();
    return strings;
  }

  public static string ConcatWithSeparator (IList list, string separator)
	{
		return ConcatWithSeparator (list, separator, null, null);
	}

	public static string ConcatWithSeparator (IList list, string separator, string format, IFormatProvider formatProvider)
	{
		if (list == null)
			throw new ArgumentNullException ("list");

    if (list.Count == 0)
			return string.Empty;

		StringBuilder sb = new StringBuilder ();
		for (int i = 0; i < list.Count; ++i)
		{
			if (i > 0)
				sb.Append (separator);

			object obj = list[i];
			IFormattable formatable = obj as IFormattable;
			if (formatable != null)
				sb.Append (formatable.ToString (format, formatProvider));
			else
				sb.Append (obj.ToString());
		}
		return sb.ToString();
	}

	public static string ConcatWithSeparator (string[] strings, string separator)
	{
		if (strings == null)
			throw new ArgumentNullException ("strings");
		if (strings.Length == 0)
			return string.Empty;

		StringBuilder sb = new StringBuilder ();
		for (int i = 0; i < strings.Length; ++i)
		{
			if (i > 0)
				sb.Append (separator);
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

  /// <summary>
  ///   Parses the specified type from a string.
  /// </summary>
  /// <param name="type"> The type that should be created from the string. This type must have 
  ///   a public static <b>Parse</b> method with either no arguments or a single 
  ///   <see cref="IFormatProvider"/>argument. 
  ///   If <paramref name="type"/> is an array type, the values must be comma-separated. (Escaping is 
  ///   handled as for <see cref="ParseSeparatedList"/>.) </param>
  /// <param name="value"> The string value to be parsed. </param>
  /// <param name="format"> The format provider to be passed to the type's <b>Parse</b> method (if present). </param>
  /// <returns> An instance of the specified type. </returns>
  /// <exception cref="ParseException"> The <b>Parse</b> method was not found, or failed. </exception>
  public static object Parse (Type type, string value, IFormatProvider format)
  {
    if (type == typeof (string))
      return value;

    if (type.IsArray)
      return ParseArrayValue (type, value, format);
    else if (type.IsEnum)
      return Enum.Parse (type, value, false);
    else
      return ParseScalarValue (type, value, format);
  }

  private static object ParseArrayValue (Type type, string value, IFormatProvider format)
  {
    Type elementType = type.GetElementType();
    ParsedItem[] items = ParseSeparatedList (value, ',');
    Array results = Array.CreateInstance (elementType, items.Length);
    for (int i = 0; i < items.Length; ++i)
    {
      try
      {
        results.SetValue (Parse (elementType, items[i].Value, format), i);
      }
      catch (ParseException e)
      {
        throw new ParseException (e.Message + " (Error accured at array index " + i.ToString() + ").", e);
      }
    }
    return results;
  }

  private static object ParseScalarValue (Type type, string value, IFormatProvider format)
  {
    MethodInfo parseMethod = GetParseMethod (type, true);

    object[] args;
    if (parseMethod.GetParameters().Length == 2)
      args = new object[] { value, format };
    else
      args = new object[] { value };

    try
    {
      return parseMethod.Invoke (null, args);
    }
    catch (TargetInvocationException e)
    {
      throw new ParseException (string.Format ("Method '{0}.Parse' failed.", type.Name), e);
    }
  }

  public static bool HasParseMethod (Type type)
  {
    MethodInfo parseMethod = GetParseMethod (type, false);
    return parseMethod != null;
  }

  private static MethodInfo GetParseMethod  (Type type, bool throwIfNotFound)
  {
    MethodInfo parseMethod = GetParseMethodFromCache (type);
    if (parseMethod == null && ! HasTypeInCache (type))
    {
      parseMethod = GetParseMethodWithFormatProviderFromType (type);
      if (parseMethod == null)
        parseMethod = GetParseMethodFromType (type);
      AddParseMethodToCache (type, parseMethod);
    }
    if (throwIfNotFound && parseMethod == null)
      throw new ParseException (string.Format ("Type does not have method 'public static {0} Parse (string s)'.", type.Name));

    return parseMethod;
  }

  private static MethodInfo GetParseMethodWithFormatProviderFromType (Type type)
  {
    ArgumentUtility.CheckNotNull ("type", type);
    MethodInfo parseMethod = type.GetMethod (
        "Parse", 
        BindingFlags.Static | BindingFlags.Public | BindingFlags.FlattenHierarchy, 
        null, 
        new Type[] {typeof (string), typeof (IFormatProvider)}, 
        null);

    if (parseMethod != null && type.IsAssignableFrom (parseMethod.ReturnType))
      return parseMethod;
    else
      return null;
  }

  private static MethodInfo GetParseMethodFromType  (Type type)
  {
    ArgumentUtility.CheckNotNull ("type", type);
    MethodInfo parseMethod = type.GetMethod (
        "Parse", 
        BindingFlags.Static | BindingFlags.Public | BindingFlags.FlattenHierarchy, 
        null, 
        new Type[] {typeof (string)}, 
        null);

    if (parseMethod != null && type.IsAssignableFrom (parseMethod.ReturnType))
      return parseMethod;
    else
      return null;
  }

  private static void AddParseMethodToCache (Type type, MethodInfo parseMethod)
  {
    ArgumentUtility.CheckNotNull ("type", type);

    lock (s_parseMethods.SyncRoot)
    {
      s_parseMethods[type] = parseMethod;
    }
  }

  private static MethodInfo GetParseMethodFromCache (Type type)
  {
    ArgumentUtility.CheckNotNull ("type", type);
    return (MethodInfo) s_parseMethods[type];
  }

  private static bool HasTypeInCache (Type type)
  {
    ArgumentUtility.CheckNotNull ("type", type);
    return s_parseMethods.ContainsKey (type);
  }

  // construction and disposal
	private StringUtility()
	{
	}
}

[Serializable]
public class ParseException: Exception
{
  public ParseException (string message)
    : base (message, null)
  {
  }

  public ParseException (string message, Exception innerException)
    : base (message, innerException)
  {
  }

  public ParseException (SerializationInfo info, StreamingContext context)
    : base (info, context)
  {
  }
}

}
