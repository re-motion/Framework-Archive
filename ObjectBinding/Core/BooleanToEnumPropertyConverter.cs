using System;
using Rubicon.Utilities;

namespace Rubicon.ObjectBinding
{

/// <summary>
///   Provides implementations for <see cref="IBusinessObjectEnumerationProperty"/> methods that can be used by 
///   implementations of <see cref="IBusinessObjectBooleanProperty"/>.
/// </summary>
public sealed class BooleanToEnumPropertyConverter
{
  private static readonly IEnumerationValueInfo s_enumInfoTrue = new EnumerationValueInfo (true, "true", "true", true);
  private static readonly IEnumerationValueInfo s_enumInfoFalse = new EnumerationValueInfo (false, "false", "false", true);

  /// <summary>
  ///   Returns the <see cref="IEnumerationValueInfo"/> objects for <see langword="true"/> and <see langword="false"/>.
  /// </summary>
  /// <returns> An array of <see cref="IEnumerationValueInfo"/> objects. </returns>
  public static IEnumerationValueInfo[] GetValues()
  {
    return new IEnumerationValueInfo[] { s_enumInfoTrue, s_enumInfoFalse };
  }

  /// <summary>
  ///   Returns an <see cref="IEnumerationValueInfo"/> if <paramref name="value"/> is <see langword="true"/> or
  ///   <see langword="false"/> and <see langword="null"/> if <paramref name="value"/> is <see langword="null"/>.
  /// </summary>
  /// <param name="value">
  ///   Can be any object taht equals to <see langword="true"/> or <see langword="false"/> and <see langword="null"/>.
  /// </param>
  /// <returns> An <see cref="IEnumerationValueInfo"/> or <see langword="null"/>. </returns>
  public static IEnumerationValueInfo GetValueInfoByValue (object value)
  {
    if (value == null)
      return null;
    else if (value.Equals (true))
      return s_enumInfoTrue;
    else if (value.Equals (false))
      return s_enumInfoFalse;
    else 
      throw new ArgumentOutOfRangeException ("value");
  }

  /// <summary>
  ///   Returns an <see cref="IEnumerationValueInfo"/> matching the <c>true</c> or <c>false</c> strings or 
  ///   <see langword="null"/> for an empty or null string.
  /// </summary>
  /// <param name="identifier"> Can be <c>true</c>, <c>false</c>, or an empty or null string. </param>
  /// <returns> An <see cref="IEnumerationValueInfo"/> or <see langword="null"/>. </returns>
  public static IEnumerationValueInfo GetValueInfoByIdentifier (string identifier)
  {
    if (StringUtility.IsNullOrEmpty (identifier))
      return null;
    else if (identifier == "true")
      return s_enumInfoTrue;
    else if (identifier == "false")
      return s_enumInfoFalse;
    else 
      throw new ArgumentOutOfRangeException ("value");
  }
}

}
