/* Copyright (C) 2005 - 2008 rubicon informationstechnologie gmbh
 *
 * This program is free software: you can redistribute it and/or modify it under 
 * the terms of the re:motion license agreement in license.txt. If you did not 
 * receive it, please visit http://www.re-motion.org/licensing.
 * 
 * Unless otherwise provided, this software is distributed on an "AS IS" basis, 
 * WITHOUT WARRANTY OF ANY KIND, either express or implied. 
 */

using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;

namespace Remotion.Utilities
{
  /// <summary>
  /// Provides utility functions that make common string handling easier.
  /// </summary>
  public static class StringUtility
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

    /// <summary>
    ///   Parses a delimiter-separated string into individual elements.
    /// </summary>
    /// <remarks>
    ///   This method handles quotes and escaping. A quoted string may contain commas that
    ///   will not be treated as separators. Commas prefixed with a backslash are treated
    ///   like normal commas, double backslashes are treated as single backslashes.
    /// </remarks>
    /// <param name="value"> The string to be parsed. Must not be <see langword="null"/>. </param>
    /// <param name="delimiter"> The character used for list separation. Default is comma (,). </param>
    public static ParsedItem[] ParseSeparatedList (string value, char delimiter)
    {
      return ParseSeparatedList (value, delimiter, '\"', '\"', '\\', " ", true);
    }

    /// <summary>
    ///   Parses a delimiter-separated string into individual elements.
    /// </summary>
    /// <remarks>
    ///   This method handles quotes and escaping. A quoted string may contain commas that
    ///   will not be treated as separators. Commas prefixed with a backslash are treated
    ///   like normal commas, double backslashes are treated as single backslashes.
    /// </remarks>
    /// <param name="value"> The string to be parsed. Must not be <see langword="null"/>. </param>
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
    /// <param name="value"> The string to be parsed. Must not be <see langword="null"/>. </param>
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
      ArgumentUtility.CheckNotNull ("value", value);

      string specialCharacters = "rnt";
      string specialCharacterResults = "\r\n\t";

      StringBuilder current = new StringBuilder();
      List<ParsedItem> items = new List<ParsedItem>();
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
  		  sb.Append (StringUtility.FormatScalarValue (list[i], formatProvider));
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


    public static string Format (object value, IFormatProvider formatProvider)
    {
      if (value == null)
        return string.Empty;
      else if (value.GetType().IsArray)
        return StringUtility.FormatArrayValue (value, formatProvider);
      else
        return StringUtility.FormatScalarValue (value, formatProvider);
    }

    public static string Format (object value)
    {
      return Format (value, null);
    }

    private static string FormatArrayValue (object value, IFormatProvider formatProvider)
    {
      Type elementType = value.GetType().GetElementType();
      string format = null;
      if (elementType == typeof (float) || elementType == typeof (double))
        format = "R";
      return StringUtility.ConcatWithSeparator ((IList) value, ",", format, formatProvider);
    }

    private static string FormatScalarValue (object value, IFormatProvider formatProvider)
    {
      if (value is string)
        return (string) value;
      IFormattable formattable = value as IFormattable;
      if (formattable != null)
      {
        string format = null;
        if (value is float || value is double)
          format = "R";
        return formattable.ToString (format, formatProvider);
      }
      return value.ToString();
    }

    /// <summary>
    ///   Parses the specified type from a string.
    /// </summary>
    /// <param name="type"> The type that should be created from the string. This type must have 
    ///   a public <see langword="static"/> <b>Parse</b> method with either no arguments or a single 
    ///   <see cref="IFormatProvider"/>argument. 
    ///   If <paramref name="type"/> is an array type, the values must be comma-separated. (Escaping is 
    ///   handled as for <see cref="ParseSeparatedList"/>.) </param>
    /// <param name="value"> The string value to be parsed. </param>
    /// <param name="formatProvider"> The format provider to be passed to the type's <b>Parse</b> method (if present). </param>
    /// <returns> An instance of the specified type. </returns>
    /// <exception cref="ParseException"> The <b>Parse</b> method was not found, or failed. </exception>
    public static object Parse (Type type, string value, IFormatProvider formatProvider)
    {
      ArgumentUtility.CheckNotNull ("type", type);

      Type underlyingType = Nullable.GetUnderlyingType (type) ?? type;
      bool isNullableType = underlyingType != type;

      if (underlyingType == typeof (string))
        return value;
      if (underlyingType.IsArray)
        return StringUtility.ParseArrayValue (type, value, formatProvider);
      if (underlyingType == typeof (DBNull))
        return DBNull.Value;
      if (isNullableType && string.IsNullOrEmpty (value))
        return null;
      if (underlyingType.IsEnum)
        return ParseEnumValue(underlyingType, value);
      if (underlyingType == typeof (Guid))
        return new Guid (value);
      else
        return StringUtility.ParseScalarValue (underlyingType, value, formatProvider);
    }

    private static object ParseArrayValue (Type type, string value, IFormatProvider formatProvider)
    {
      Type elementType = type.GetElementType();
      if (elementType.IsArray)
        throw new ParseException ("Cannot parse an array of arrays.");
      ParsedItem[] items = StringUtility.ParseSeparatedList (value, ',');
      Array results = Array.CreateInstance (elementType, items.Length);
      for (int i = 0; i < items.Length; ++i)
      {
        try
        {
          results.SetValue (StringUtility.Parse (elementType, items[i].Value, formatProvider), i);
        }
        catch (ParseException e)
        {
          throw new ParseException (e.Message + " (Error accured at array index " + i.ToString() + ").", e);
        }
      }
      return results;
    }

    private static object ParseScalarValue (Type type, string value, IFormatProvider formatProvider)
    {
      MethodInfo parseMethod = StringUtility.GetParseMethod (type, true);

      object[] args;
      if (parseMethod.GetParameters().Length == 2)
        args = new object[] { value, formatProvider };
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

    private static object ParseEnumValue (Type underlyingType, string value)
    {
      try
      {
        return Enum.Parse (underlyingType, value, false);
      }
      catch (ArgumentException)
      {
        throw new ParseException (string.Format ("{0} is not a valid value for {1}.", value, underlyingType.Name));
      }
    }

    public static bool CanParse (Type type)
    {
      ArgumentUtility.CheckNotNull ("type", type);

      type = Nullable.GetUnderlyingType (type) ?? type;

      if (type == typeof (string))
        return true;
      if (type == typeof (DBNull))
        return true;
      if (type == typeof (Guid))
        return true;
      if (type.IsEnum)
        return true;
      if (type.IsArray)
      {
        Type elementType = type.GetElementType();
        if (elementType.IsArray)
          return false;
        return StringUtility.CanParse (elementType);
      }
      MethodInfo parseMethod = StringUtility.GetParseMethod (type, false);
      return parseMethod != null;
    }

    private static MethodInfo GetParseMethod  (Type type, bool throwIfNotFound)
    {
      MethodInfo parseMethod = StringUtility.GetParseMethodFromCache (type);
      if (parseMethod == null && ! StringUtility.HasTypeInCache (type))
      {
        parseMethod = StringUtility.GetParseMethodWithFormatProviderFromType (type);
        if (parseMethod == null)
          parseMethod = StringUtility.GetParseMethodFromType (type);
        StringUtility.AddParseMethodToCache (type, parseMethod);
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
