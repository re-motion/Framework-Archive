using System;
using Rubicon.Utilities;

namespace Rubicon.ObjectBinding
{

public class BooleanEnumerationValueInfo: IEnumerationValueInfo
{
  bool _value;
  IBusinessObjectBooleanProperty _property;
  
  public BooleanEnumerationValueInfo (bool value, IBusinessObjectBooleanProperty property)
  {
    _value = value;
    _property = property;
  }

  public string DisplayName
  {
    get { return _property.GetDisplayName (_value); }
  }

  public string Identifier
  {
    get { return _value.ToString(); }
  }

  public object Value
  {
    get { return _value; }
  }

  public bool IsEnabled
  {
    get { return true; }
  }
}

/// <summary>
///   Provides implementations for <see cref="IBusinessObjectEnumerationProperty"/> methods that can be used by 
///   implementations of <see cref="IBusinessObjectBooleanProperty"/>.
/// </summary>
public class BooleanToEnumPropertyConverter
{
  private IEnumerationValueInfo _enumInfoTrue;
  private IEnumerationValueInfo _enumInfoFalse;

  public BooleanToEnumPropertyConverter (IBusinessObjectBooleanProperty property)
  {
    _enumInfoTrue = new BooleanEnumerationValueInfo (true, property);
    _enumInfoFalse = new BooleanEnumerationValueInfo (false, property);
  }

  /// <summary>
  ///   Returns the <see cref="IEnumerationValueInfo"/> objects for <see langword="true"/> and <see langword="false"/>.
  /// </summary>
  /// <returns> An array of <see cref="IEnumerationValueInfo"/> objects. </returns>
  public IEnumerationValueInfo[] GetValues()
  {
    return new IEnumerationValueInfo[] { _enumInfoTrue, _enumInfoFalse };
  }

  /// <summary>
  ///   Returns an <see cref="IEnumerationValueInfo"/> if <paramref name="value"/> is <see langword="true"/> or
  ///   <see langword="false"/> and <see langword="null"/> if <paramref name="value"/> is <see langword="null"/>.
  /// </summary>
  /// <param name="value">
  ///   Can be any object that equals to <see langword="true"/> or <see langword="false"/> and <see langword="null"/>.
  /// </param>
  /// <returns> An <see cref="IEnumerationValueInfo"/> or <see langword="null"/>. </returns>
  public IEnumerationValueInfo GetValueInfoByValue (object value)
  {
    if (value == null)
      return null;
    else if (value.Equals (true))
      return _enumInfoTrue;
    else if (value.Equals (false))
      return _enumInfoFalse;
    else 
      throw new ArgumentOutOfRangeException ("value");
  }

  /// <summary>
  ///   Returns an <see cref="IEnumerationValueInfo"/> matching the <c>true</c> or <c>false</c> strings or 
  ///   <see langword="null"/> for an empty or null string.
  /// </summary>
  /// <param name="identifier"> Can be <c>true</c>, <c>false</c>, or an empty or null string. </param>
  /// <returns> An <see cref="IEnumerationValueInfo"/> or <see langword="null"/>. </returns>
  public IEnumerationValueInfo GetValueInfoByIdentifier (string identifier)
  {
    if (StringUtility.IsNullOrEmpty (identifier))
      return null;
    else if (identifier == "true")
      return _enumInfoTrue;
    else if (identifier == "false")
      return _enumInfoFalse;
    else 
      throw new ArgumentOutOfRangeException ("value");
  }
}

}
