using System;
using System.Collections;
using Rubicon.Utilities;

namespace Rubicon.ObjectBinding
{

/// <summary>
///   A collection of business object properties that result in each other.
/// </summary>
public class BusinessObjectPropertyPath
{
  private const string c_multipleValuesFormatString = "{0}, ... [{1}]";

  private IBusinessObjectProperty[] _properties; 
 
  public BusinessObjectPropertyPath (IBusinessObjectProperty[] properties)
  {
    ArgumentUtility.CheckNotNullOrEmpty ("properties", properties);
    for (int i = 0; i < properties.Length - 1; ++i)
    {
      if (! (properties[i] is IBusinessObjectReferenceProperty))
        throw new ArgumentException ("Each property in a property path except the last one must be a reference property.", "properties");
    }
    _properties = properties;
  }

  public IBusinessObjectProperty[] Properties 
  { 
    get { return _properties; }
  }

  public IBusinessObjectProperty LastProperty 
  { 
    get { return _properties[_properties.Length - 1]; }
  }

  public object GetValue (IBusinessObject obj)
  {
    object value = obj;
    foreach (IBusinessObjectProperty property in _properties)
    {
      obj = value as IBusinessObject;
      if (obj == null)
        throw new Exception ("Cannot get value"); // TODO: throw useful exception
      value = obj.GetProperty (property);
      if (value == null)
        return null;
    }
    return value;
  }

  public object SetValue (IBusinessObject obj)
  {
    // TODO: implement
    throw new NotImplementedException();
  }

  public string GetStringValue (IBusinessObject obj)
  {
    object value = GetValue (obj);
    if (value == null)
    {
      return string.Empty;
    }
    else if (value is IList)
    {
      IList list = (IList) value;
      if (list[0] == null)
        return null;
      return string.Format (c_multipleValuesFormatString, list[0], list.Count);
    }
    else
    {
      return value.ToString();
    }        
  }
}

}
