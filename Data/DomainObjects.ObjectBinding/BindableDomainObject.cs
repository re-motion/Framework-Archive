using System;
using System.ComponentModel;

using Rubicon.ObjectBinding;
using Rubicon.Utilities;

using Rubicon.Data.DomainObjects.ObjectBinding.PropertyTypes;

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

  // member fields

  private BusinessObjectReflector _objectReflector;

  // construction and disposing

  protected BindableDomainObject ()
  {
    _objectReflector = new BusinessObjectReflector (this);
  }

  protected BindableDomainObject (ClientTransaction clientTransaction) : base (clientTransaction)
  {
    _objectReflector = new BusinessObjectReflector (this);
  }

  protected BindableDomainObject (DataContainer dataContainer) : base (dataContainer)
  {
    _objectReflector = new BusinessObjectReflector (this);
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
    return _objectReflector.GetProperty (property);
  }

  public void SetProperty (IBusinessObjectProperty property, object value)
  {
    _objectReflector.SetProperty (property, value);
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
    return _objectReflector.GetPropertyString (property, format);
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
