using System;
using System.Reflection;
using System.Xml.Serialization;
using Rubicon.ObjectBinding;
using Rubicon.NullableValueTypes;
using Rubicon.Utilities;

namespace Rubicon.ObjectBinding.Reflection
{

/// <summary>
/// This class provides BusinessObject interfaces for simple .NET objects.
/// </summary>
public abstract class ReflectionBusinessObject: BusinessObject
{
  public override IBusinessObjectProperty GetBusinessObjectProperty (string propertyIdentifier)
  {
    return BusinessObjectClass.GetProperty (propertyIdentifier);
  }

  [XmlIgnore]
  public override IBusinessObjectClass BusinessObjectClass
  {
    get { return new ReflectionBusinessObjectClass (this.GetType()); }
  }

  public override object GetProperty (IBusinessObjectProperty property)
  {
    ArgumentUtility.CheckNotNullAndType ("property", property, typeof (ReflectionBusinessObjectProperty));
    ReflectionBusinessObjectProperty reflectionProperty = (ReflectionBusinessObjectProperty) property;
    PropertyInfo propertyInfo = reflectionProperty.PropertyInfo;

    object internalValue = propertyInfo.GetValue (this, new object[0]);
    return reflectionProperty.FromInternalType (internalValue);
  }

  public override void SetProperty (IBusinessObjectProperty property, object value)
  {
    ArgumentUtility.CheckNotNullAndType ("property", property, typeof (ReflectionBusinessObjectProperty));
    ReflectionBusinessObjectProperty reflectionProperty = (ReflectionBusinessObjectProperty) property;
    PropertyInfo propertyInfo = reflectionProperty.PropertyInfo;

    object internalValue = reflectionProperty.ToInternalType (value);
    propertyInfo.SetValue (this, internalValue, new object[0]);
  }
}

}
