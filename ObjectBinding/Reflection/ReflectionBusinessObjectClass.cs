using System;
using System.Reflection;
using Rubicon.Utilities;

namespace Rubicon.ObjectBinding.Reflection
{

public class ReflectionBusinessObjectClass: IBusinessObjectClass
{
  Type _type;

  public ReflectionBusinessObjectClass (Type type)
  {
    ArgumentUtility.CheckNotNull ("type", type);
    _type = type;
  }

  public IBusinessObjectProperty GetPropertyDefinition (string propertyIdentifier)
  {
    PropertyInfo propertyInfo = _type.GetProperty (propertyIdentifier);
    return (propertyInfo == null) ? null : ReflectionBusinessObjectProperty.Create (propertyInfo);
  }

  public IBusinessObjectProperty[] GetPropertyDefinitions()
  {
    PropertyInfo[] propertyInfos = _type.GetProperties ();
    if (propertyInfos == null)
      return new IBusinessObjectProperty[0];

    IBusinessObjectProperty[] properties = new IBusinessObjectProperty[propertyInfos.Length];
    for (int i = 0; i < propertyInfos.Length; ++i)
      properties[i] = ReflectionBusinessObjectProperty.Create (propertyInfos[i]);

    return properties;
  }

  public IBusinessObjectProvider BusinessObjectProvider 
  {
    get { return ReflectionBusinessObjectProvider.Instance; }
  }
}

}
