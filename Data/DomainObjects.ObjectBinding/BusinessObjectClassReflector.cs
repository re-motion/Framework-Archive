using System;
using System.Collections;
using System.Reflection;
using System.ComponentModel;

using Rubicon.Utilities;
using Rubicon.ObjectBinding;

namespace Rubicon.Data.DomainObjects.ObjectBinding
{
public class BusinessObjectClassReflector
{
  private Type _businessObjectClassType;
  private ReflectionPropertyFactory _propertyFactory;

	public BusinessObjectClassReflector (Type businessObjectClassType, ReflectionPropertyFactory propertyFactory)
	{
    ArgumentUtility.CheckNotNull ("businessObjectClassType", businessObjectClassType);
    ArgumentUtility.CheckNotNull ("propertyFactory", propertyFactory);

    _businessObjectClassType = businessObjectClassType;
    _propertyFactory = propertyFactory;
  }

  public Type BusinessObjectClassType
  {
    get { return _businessObjectClassType; }
  }

  public string Identifier
  {
    get { return _businessObjectClassType.FullName; }
  }

  public IBusinessObjectProperty GetPropertyDefinition (string propertyIdentifier)
  {
    PropertyInfo propertyInfo = _businessObjectClassType.GetProperty (propertyIdentifier);
    return propertyInfo == null ? null : _propertyFactory.CreateProperty (propertyInfo);
  }

  public IBusinessObjectProperty[] GetPropertyDefinitions()
  {
    PropertyInfo[] propertyInfos = _businessObjectClassType.GetProperties ();
    if (propertyInfos == null)
      return new IBusinessObjectProperty[0];

    ArrayList properties = new ArrayList();
    foreach (PropertyInfo propertyInfo in propertyInfos)
    {
      if (UsePropertyInBusinessObject (propertyInfo))
        properties.Add (_propertyFactory.CreateProperty (propertyInfo));
    }

    return (IBusinessObjectProperty[]) properties.ToArray (typeof (IBusinessObjectProperty));
  }

  private bool UsePropertyInBusinessObject (PropertyInfo propertyInfo)
  {
    EditorBrowsableAttribute[] editorBrowsableAttributes = (EditorBrowsableAttribute[]) propertyInfo.GetCustomAttributes (
        typeof (EditorBrowsableAttribute), true);

    if (editorBrowsableAttributes.Length == 1)
    {
      EditorBrowsableAttribute editorBrowsableAttribute = editorBrowsableAttributes[0];
      if (editorBrowsableAttribute.State == EditorBrowsableState.Never)
        return false;
    }

    //  Prevents the display of the indexers declared in BusinessObject.
    //  Adding "EditorBrowsable (EditorBrowsableState.Never)" to BusinessObject 
    //  might not be the best solution until the final way of hiding properties is established
    if (propertyInfo.Name == "Item")
      return false;

    return true;
  }
}
}
