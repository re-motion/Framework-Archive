using System;

namespace Rubicon.ObjectBinding
{

/// <summary>
///   Provides implementations for <see cref="IBusinessObjectEnumerationProperty"/> methods that can be used by boolean properties.
/// </summary>
public sealed class BooleanToEnumPropertyConverter
{
  private static readonly IEnumerationValueInfo s_enumInfoTrue = new EnumerationValueInfo (true, "true", "true", true);
  private static readonly IEnumerationValueInfo s_enumInfoFalse = new EnumerationValueInfo (false, "false", "false", true);

  public static IEnumerationValueInfo[] GetValues()
  {
    return new IEnumerationValueInfo[] { s_enumInfoTrue, s_enumInfoFalse };
  }

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

  public static IEnumerationValueInfo GetValueInfoByIdentifier (string identifier)
  {
    if (identifier == null || identifier.Length == 0)
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
