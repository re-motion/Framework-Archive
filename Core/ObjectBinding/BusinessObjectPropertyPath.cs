using System;
using System.Collections;
using System.Text;
using Rubicon.Utilities;

namespace Rubicon.ObjectBinding
{

/// <summary>
///   A collection of business object properties that result in each other.
/// </summary>
public abstract class BusinessObjectPropertyPath
{
  /// <summary>
  ///   Property path formatters can be passed to <see cref="String.Format"/> for full <see cref="IFormattable"/> support.
  /// </summary>
  public class Formatter: IFormattable
  {
    private IBusinessObject _object;
    private BusinessObjectPropertyPath _path;

    public Formatter (IBusinessObject obj, BusinessObjectPropertyPath path)
    {
      _object = obj;
      _path = path;
    }

    public string ToString (string format, IFormatProvider formatProvider)
    {
      return _path.GetString (_object, format);
    }

    public override string ToString()
    {
      return _path.GetString (_object, null);
    }
  }


  private IBusinessObjectProperty[] _properties; 
 
  public static BusinessObjectPropertyPath Parse (
      IBusinessObjectClass objectClass, 
      string propertyPathIdentifier)
  {
    ArgumentUtility.CheckNotNull ("objectClass", objectClass);
    ArgumentUtility.CheckNotNullOrEmpty ("propertyPathIdentifier", propertyPathIdentifier);
    
    char separator = objectClass.BusinessObjectProvider.GetPropertyPathSeparator();

    string[] propertyIdentifiers = propertyPathIdentifier.Split (separator);
    IBusinessObjectProperty[] properties = new IBusinessObjectProperty [propertyIdentifiers.Length];

    int lastProperty = propertyIdentifiers.Length - 1;

    for (int i = 0; i < lastProperty; i++)
    {
      properties[i] = objectClass.GetPropertyDefinition (propertyIdentifiers[i]);      
      if (properties[i] == null)
        throw new ArgumentException ("BusinessObjectClass '" + objectClass.GetType().FullName + "' does not contain a property named '" + propertyIdentifiers[i] + "'.", propertyPathIdentifier);

      IBusinessObjectReferenceProperty referenceProperty = properties[i] as IBusinessObjectReferenceProperty;
      if (referenceProperty == null)
        throw new ArgumentException ("Each property in a property path except the last one must be a reference property.", "properties");

      objectClass = referenceProperty.ReferenceClass;
    }

    properties[lastProperty] = objectClass.GetPropertyDefinition (propertyIdentifiers[lastProperty]);
    if (properties[lastProperty] == null)
      throw new ArgumentException ("BusinessObjectClass '" + objectClass.GetType().FullName + "' does not contain a property named '" + propertyIdentifiers[lastProperty] + "'.", propertyPathIdentifier);

    objectClass.BusinessObjectProvider.CreatePropertyPath (properties);
  }

  public abstract IBusinessObjectProvider BusinessObjectProvider { get; }
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

  /// <summary>
  ///   Gets the value of this property path for the specified object. 
  /// </summary>
  /// <param name="obj"> The object that has the first property in the path. </param>
  /// <param name="throwExceptionIfNotReachable"> 
  ///   If <c>true</c>, an InvalidOperationException is thrown if any but the last property in the path is null. If <c>false</c>,
  ///   <see langword="null"/> is returned instead. </param>
  /// <param name="getFirstListEntry">
  ///   If <c>true</c>, the first value of each list property is processed. If <c>false</c>, evaluation of list properties 
  ///   causes an InvalidOperationException.
  ///   (This does not apply to the last property in the path. If the last property is a list property, the return value is always a list.) </param>
  /// <exception cref="InvalidOperationException"> 
  ///   Thrown if any but the last property in the path is <see langword="null"/>, or is not a single-value reference property. </exception>
  public virtual object GetValue (IBusinessObject obj, bool throwExceptionIfNotReachable, bool getFirstListEntry)
  {
    IBusinessObject obj2 = GetValueWithoutLast (obj, throwExceptionIfNotReachable, getFirstListEntry);
    if (obj2 == null)
      return null;

    return obj2.GetProperty (LastProperty);
  }
  
  public virtual string GetString (IBusinessObject obj, string format)
  {
    IBusinessObject obj2 = GetValueWithoutLast (obj, false, true);
    if (obj2 == null)
      return null;

    return obj2.GetPropertyString (LastProperty, format);
  }
  
  private IBusinessObject GetValueWithoutLast (IBusinessObject obj, bool throwExceptionIfNotReachable, bool getFirstListEntry)
  {
    for (int i = 0; i < (_properties.Length - 1); ++i)
    {
      IBusinessObjectProperty property = _properties[i];
      if (! (property is IBusinessObjectReferenceProperty))
        throw new InvalidOperationException (string.Format ("Element {0} of property path {1} is not reference property.", i, this));
      if (property.IsList)
      {
        if (getFirstListEntry)
        {
          IList list = (IList) obj.GetProperty (property);
          if (list.Count > 0)
            obj = (IBusinessObject) list[0];
          else
            obj = null;
        }
        else
        {
          throw new InvalidOperationException (string.Format ("Element {0} of property path {1} is a not a single-value property.", i, this));
        }
      }
      else
      {
        obj = (IBusinessObject) obj.GetProperty (property);
      }
            
      if (obj == null)
      {
        if (throwExceptionIfNotReachable)
          throw new InvalidOperationException (string.Format ("A null value was detected in element {0} of property path {1}. Cannot evaluate rest of path.", i, this));
        else
          return null;
      }
    }
    return obj;
  }

  public virtual object SetValue (IBusinessObject obj)
  {
    // TODO: implement
    throw new NotImplementedException();
  }

  public string Identifier
  {
    get
    {
      StringBuilder sb = new StringBuilder (100);
      char separator = '\0';
		  for (int i = 0; i < _properties.Length; i++)
		  {
			  if (i == 0)
          separator = _properties[i].BusinessObjectProvider.GetPropertyPathSeparator();
        else
	  			sb.Append (separator);
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
