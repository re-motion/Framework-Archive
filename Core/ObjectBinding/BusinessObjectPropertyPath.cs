using System;
using System.Collections;
using System.Text;
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
 
  public static BusinessObjectPropertyPath Parse (
    IBusinessObjectDataSource dataSource, 
    string propertyPathIdentifier)
  {
    ArgumentUtility.CheckNotNull ("dataSource", dataSource);
    ArgumentUtility.CheckNotNullOrEmpty ("propertyPathIdentifier", propertyPathIdentifier);

    string[] propertyIdentifiers = propertyPathIdentifier.Split ('.');
    IBusinessObjectProperty[] properties = 
      //  TODO: BusinessObjectPropertyPath.Parse
      //  new IBusinessObjectProperty [propertyIdentifiers.Length];
      new IBusinessObjectProperty [1];

    if (properties.Length > 0)
    {
      properties[0] = dataSource.BusinessObjectClass.GetPropertyDefinition (propertyIdentifiers[0]); 
      if (properties[0] == null)
        throw new ArgumentException ("BusinessObjectClass '" + dataSource.BusinessObjectClass.GetType().FullName + "' does not contain a property named '" + propertyIdentifiers[0] + "'.", propertyPathIdentifier);
    }
//    IBusinessObjectClass businessObjectClass = 
//      dataSource.BusinessObjectClass as IBusinessObjectClass;
//    IBusinessObject bo;
//    bo.BusinessObjectClass;
//    for (int i; i < propertyIdentifiers.Length - 1; i++)
//    {
//      properties[i] = businessObjectClass.GetPropertyDefinition (propertyIdentifiers[i]); 
//      businessObjectClass.
//      properties[i].
//    }
//
//    IBusinessObjectProperty property = DataSource.BusinessObjectClass.GetPropertyDefinition (_propertyIdentifier); 
//    if (! Control.SupportsProperty (property))
//      throw new ArgumentException ("Property 'Property' does not support the property '" + _propertyIdentifier + "'.");
//    _property = property;
//
//  throw new NotImplementedException ("BusinessObjectPropertyPath.Parse");

    return new BusinessObjectPropertyPath (properties);
  }

  public BusinessObjectPropertyPath (IBusinessObjectProperty[] properties)
  {
    ArgumentUtility.CheckNotNullOrEmpty ("properties", properties);
    for (int i = 0; i < properties.Length - 1; ++i)
    {
      if (! (properties[i] is IBusinessObjectReferenceProperty))
        throw new ArgumentException ("Each property in a property path except the last one must be a reference property.", "properties");
    }

    if (properties[properties.Length - 1] == null)
      throw new ArgumentNullException ("properties[properties.Length - 1]", "A property path must not contain null references.");

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

  public string Identifier
  {
    get
    {
      StringBuilder sb = new StringBuilder (100);
		  for (int i = 0; i < _properties.Length; i++)
		  {
			  if (i > 0)
	  			sb.Append (".");
        sb.Append (_properties[i].Identifier);
      }
      return sb.ToString();
    }
  }

  public override string ToString()
  {
    return Identifier;
  }

}

}
