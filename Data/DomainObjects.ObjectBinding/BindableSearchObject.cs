using System;
using System.ComponentModel;

using Rubicon.ObjectBinding;
using Rubicon.Data.DomainObjects.Queries;
using Rubicon.Data.DomainObjects.Queries.Configuration;

namespace Rubicon.Data.DomainObjects.ObjectBinding
{
public abstract class BindableSearchObject : IBusinessObject
{
  private BusinessObjectReflector _objectReflector;

	public BindableSearchObject()
	{
    _objectReflector = new BusinessObjectReflector (this);
  }

  public IBusinessObjectProperty GetBusinessObjectProperty (string propertyIdentifier)
  {
    return BusinessObjectClass.GetPropertyDefinition (propertyIdentifier);
  }

  public abstract IQuery CreateQuery ();

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
}
}
