using System;
using System.Reflection;
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

  public override IBusinessObjectClass BusinessObjectClass
  {
    get { return new ReflectionBusinessObjectClass (this.GetType()); }
  }

  public override object GetProperty (IBusinessObjectProperty property)
  {
    ArgumentUtility.CheckNotNullAndType ("property", property, typeof (ReflectionBusinessObjectProperty));
    PropertyInfo propertyInfo = ((ReflectionBusinessObjectProperty)property).PropertyInfo;

    return propertyInfo.GetValue (this, new object[0]);
  }

  public override void SetProperty (IBusinessObjectProperty property, object value)
  {
    ArgumentUtility.CheckNotNullAndType ("property", property, typeof (ReflectionBusinessObjectProperty));
    PropertyInfo propertyInfo = ((ReflectionBusinessObjectProperty)property).PropertyInfo;

    propertyInfo.SetValue (this, value, new object[0]);
  }
}

}
