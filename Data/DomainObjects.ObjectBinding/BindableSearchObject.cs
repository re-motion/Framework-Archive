using System;
using System.ComponentModel;

using Rubicon.ObjectBinding;
using Rubicon.Data.DomainObjects.Queries;
using Rubicon.Data.DomainObjects.Queries.Configuration;

namespace Rubicon.Data.DomainObjects.ObjectBinding
{
public abstract class BindableQuery : IBusinessObject, IQuery
{
  private BusinessObjectReflector _objectReflector;

	public BindableQuery()
	{
    _objectReflector = new BusinessObjectReflector (this);
  }

  public IBusinessObjectProperty GetBusinessObjectProperty (string propertyIdentifier)
  {
    return BusinessObjectClass.GetPropertyDefinition (propertyIdentifier);
  }

  #region IBusinessObject Members

  public string GetPropertyString (string property)
  {
    return GetPropertyString (GetBusinessObjectProperty (property));
  }

  public string GetPropertyString (IBusinessObjectProperty property, string format)
  {
    return _objectReflector.GetPropertyString (property, format);
  }

  public string GetPropertyString (IBusinessObjectProperty property)
  {
    return GetPropertyString (property, null);
  }

  public object GetProperty (string property)
  {
    return GetProperty (GetBusinessObjectProperty (property));
  }

  public object GetProperty (IBusinessObjectProperty property)
  {
    return _objectReflector.GetProperty (property);
  }

  public object this[string property]
  {
    get { return GetProperty (property); }
    set { SetProperty (property, value); }
  }

  public object this[IBusinessObjectProperty property]
  {
    get { return GetProperty (property); }
    set { SetProperty (property, value); }
  }

  public void SetProperty (string property, object value)
  {
    SetProperty (GetBusinessObjectProperty (property), value);
  }

  public void SetProperty (IBusinessObjectProperty property, object value)
  {
    _objectReflector.SetProperty (property, value);
  }

  [EditorBrowsable (EditorBrowsableState.Never)]
  public IBusinessObjectClass BusinessObjectClass
  {
    get { return new DomainObjectClass (this.GetType()); }
  }

  #endregion

  #region IQuery Members

  [EditorBrowsable (EditorBrowsableState.Never)]
  public virtual Type CollectionType
  {
    get
    {
      if (this.QueryType != QueryType.Collection)
        return null;

      return typeof (DomainObjectCollection);
    }
  }

  [EditorBrowsable (EditorBrowsableState.Never)]
  public virtual string QueryID
  {
    get { return this.GetType ().FullName; }
  }

  [EditorBrowsable (EditorBrowsableState.Never)]
  public abstract QueryType QueryType { get; }

  [EditorBrowsable (EditorBrowsableState.Never)]
  public abstract string Statement { get; }

  [EditorBrowsable (EditorBrowsableState.Never)]
  public abstract string StorageProviderID { get; }

  [EditorBrowsable (EditorBrowsableState.Never)]
  public abstract QueryParameterCollection Parameters { get; }

  #endregion
}
}
