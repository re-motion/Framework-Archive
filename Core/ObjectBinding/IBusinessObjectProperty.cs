using System;
using Rubicon.NullableValueTypes;

namespace Rubicon.ObjectBinding
{

public interface IBusinessObjectProperty
{
  bool IsList { get; }
  Type PropertyType { get; }
  string Identifier { get; }
  string DisplayName { get; }
  NaBoolean IsRequired { get; }
 
  /// <param name="obj"> The object to evaluate this property for, or <see langword="null"/>. </param>
  bool IsAccessible (IBusinessObject obj);

  /// <param name="obj"> The object to evaluate this property for, or <see langword="null"/>. </param>
  bool IsReadOnly (IBusinessObject obj);
}

public interface IBusinessObjectStringProperty: IBusinessObjectProperty
{
  NaInt32 MaxLength { get; }
}

public interface IBusinessObjectNumericProperty: IBusinessObjectProperty
{
  bool AllowNegative { get; }
}

public interface IBusinessObjectDoubleProperty: IBusinessObjectNumericProperty
{
}

public interface IBusinessObjectInt32Property: IBusinessObjectNumericProperty
{
}

public interface IBusinessObjectDateProperty: IBusinessObjectProperty
{
}

public interface IBusinessObjectDateTimeProperty: IBusinessObjectProperty
{
}

public interface IBusinessObjectReferenceProperty: IBusinessObjectProperty
{
}

public interface IBusinessObjectBooleanProperty: IBusinessObjectProperty
{
}

public interface IBusinessObjectEnumerationProperty: IBusinessObjectProperty
{
  IEnumerationValueInfo[] GetAllValues ();
  IEnumerationValueInfo[] GetEnabledValues ();
}

public interface IEnumerationValueInfo
{
  int Value { get; }
  string Identifier { get; }
  string DisplayName { get; }
  bool IsEnabled { get; }
}

public class EnumerationValueInfo: IEnumerationValueInfo
{
  private int _value;
  private string _identifier;
  private string _displayName;
  private bool _isEnabled; 

  public EnumerationValueInfo (int value, string identifier, string displayName, bool isEnabled)
  {
    _value = value;
    _identifier = identifier;
    _displayName = displayName;
    _isEnabled = isEnabled;
  }

  public int Value
  {
    get { return _value; }
  }

  public string Identifier
  {
    get { return _identifier; }
  }

  public string DisplayName
  {
    get { return _displayName; }
  }

  public bool IsEnabled
  {
    get { return _isEnabled; }
  }
}

}
