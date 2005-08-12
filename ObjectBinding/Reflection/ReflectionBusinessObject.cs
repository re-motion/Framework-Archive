using System;
using System.Reflection;
using System.Collections;
using System.Xml.Serialization;
using System.ComponentModel;
using Rubicon.ObjectBinding;
using Rubicon.NullableValueTypes;
using Rubicon.Utilities;

namespace Rubicon.ObjectBinding.Reflection
{

/// <summary>
///   This class provides BusinessObject interfaces for simple .NET objects.
/// </summary>
[Serializable]
public abstract class ReflectionBusinessObject: BusinessObject, IBusinessObjectWithIdentity
{
  internal Guid _id;

  public ReflectionBusinessObject ()
  {
    _id = Guid.NewGuid(); // this id is replaced immediately if the object is loaded from xml
  }

  [XmlIgnore]
  [EditorBrowsable (EditorBrowsableState.Never)]
  public Guid ID
  {
    get { return _id; }
  }

//  public override IBusinessObjectProperty GetBusinessObjectProperty (string propertyIdentifier)
//  {
//    return BusinessObjectClass.GetPropertyDefinition (propertyIdentifier);
//  }

  [XmlIgnore]
  [EditorBrowsable (EditorBrowsableState.Never)]
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

  [EditorBrowsable (EditorBrowsableState.Never)]
  public virtual string DisplayName 
  { 
    get { return GetType().FullName; }
  }

  [EditorBrowsable (EditorBrowsableState.Never)]
  string IBusinessObjectWithIdentity.UniqueIdentifier
  {
    get { return _id.ToString(); }
  }

  public void SaveObject()
  {
    ReflectionBusinessObjectStorage.SaveObject (this);
  }
}

}
