using System;
using System.Reflection;
using System.Collections;
using System.ComponentModel;

using Rubicon.ObjectBinding;
using Rubicon.NullableValueTypes;
using Rubicon.Utilities;

namespace Rubicon.Data.DomainObjects.ObjectBinding
{

public class BindableDomainObject: DomainObject, IBusinessObjectWithIdentity
{
  // types

  // static members and constants

  internal protected static new BindableDomainObject GetObject (ObjectID id)
  {
    return (BindableDomainObject) DomainObject.GetObject (id);
  }

  internal protected static new BindableDomainObject GetObject (ObjectID id, bool includeDeleted)
  {
    return (BindableDomainObject) DomainObject.GetObject (id, includeDeleted);
  }

  internal protected static new BindableDomainObject GetObject (ObjectID id, ClientTransaction clientTransaction)
  {
    return (BindableDomainObject) DomainObject.GetObject (id, clientTransaction);
  }

  internal protected static new BindableDomainObject GetObject (ObjectID id, ClientTransaction clientTransaction, bool includeDeleted)
  {
    ArgumentUtility.CheckNotNull ("id", id);
    ArgumentUtility.CheckNotNull ("clientTransaction", clientTransaction);

    return (BindableDomainObject) DomainObject.GetObject (id, clientTransaction, includeDeleted);
  }

  public static string GetPropertyString (IBusinessObject obj, IBusinessObjectProperty property, string format)
  {
    object value;
    int count;

    if (property.IsList)
    {
      IList list = (IList) obj.GetProperty (property);
      count = list.Count;
      if (count > 0)
        value = list[0];
      else 
        value = null;
    }
    else
    {
      value = obj.GetProperty (property);
      count = 1;
    }

    if (value == null)
      return string.Empty;

    string strValue = null;
    IBusinessObjectWithIdentity businessObject = value as IBusinessObjectWithIdentity;
    if (businessObject != null)
    {
      strValue = businessObject.DisplayName;
    }
    else if (format != null)
    {
      IFormattable formattable = value as IFormattable;
      if (formattable != null)
        strValue = formattable.ToString (format, null);
    }

    if (strValue == null)
      strValue = value.ToString();

    if (count > 1)
      strValue += " ... [" + count.ToString() + "]";
    return strValue;
  }

  // member fields

  // construction and disposing

  protected BindableDomainObject ()
  {
  }

  protected BindableDomainObject (ClientTransaction clientTransaction) : base (clientTransaction)
  {
  }

  protected BindableDomainObject (DataContainer dataContainer) : base (dataContainer)
  {
  }

  // methods and properties

  [EditorBrowsable (EditorBrowsableState.Never)]
  public new ObjectID ID
  {
    get { return base.ID; }
  }

  public IBusinessObjectProperty GetBusinessObjectProperty (string propertyIdentifier)
  {
    return BusinessObjectClass.GetPropertyDefinition (propertyIdentifier);
  }

  [EditorBrowsable (EditorBrowsableState.Never)]
  public IBusinessObjectClass BusinessObjectClass
  {
    get { return new DomainObjectClass (this.GetType()); }
  }

  public object GetProperty (IBusinessObjectProperty property)
  {
    ArgumentUtility.CheckNotNullAndType ("property", property, typeof (DomainObjectProperty));
    DomainObjectProperty reflectionProperty = (DomainObjectProperty) property;
    PropertyInfo propertyInfo = reflectionProperty.PropertyInfo;

    object internalValue = propertyInfo.GetValue (this, new object[0]);
    return reflectionProperty.FromInternalType (internalValue);
  }

  public void SetProperty (IBusinessObjectProperty property, object value)
  {
    ArgumentUtility.CheckNotNullAndType ("property", property, typeof (DomainObjectProperty));
    DomainObjectProperty reflectionProperty = (DomainObjectProperty) property;
    PropertyInfo propertyInfo = reflectionProperty.PropertyInfo;

    object internalValue = reflectionProperty.ToInternalType (value);
    propertyInfo.SetValue (this, internalValue, new object[0]);
  }

  [EditorBrowsable (EditorBrowsableState.Never)]
  public virtual string DisplayName 
  { 
    get { return ID.ToString (); } 
  }

  [EditorBrowsable (EditorBrowsableState.Never)]
  string IBusinessObjectWithIdentity.UniqueIdentifier
  {
    get { return ID.ToString (); }
  }

  public object this [IBusinessObjectProperty property]
  {
    get { return GetProperty (property); }
    set { SetProperty (property, value); }
  }

  public string GetPropertyString (IBusinessObjectProperty property)
  {
    return GetPropertyString (property, null);
  }

  public virtual string GetPropertyString (IBusinessObjectProperty property, string format)
  {
    return GetPropertyString (this, property, format);
  }
  
  public object GetProperty (string property)
  {
    return GetProperty (GetBusinessObjectProperty (property));
  }

  public void SetProperty (string property, object value)
  {
    SetProperty (GetBusinessObjectProperty (property), value);
  }

  public object this [string property]
  {
    get { return GetProperty (property); }
    set { SetProperty (property, value); }
  }

  public string GetPropertyString (string property)
  {
    return GetPropertyString (GetBusinessObjectProperty (property));
  }
}

}
