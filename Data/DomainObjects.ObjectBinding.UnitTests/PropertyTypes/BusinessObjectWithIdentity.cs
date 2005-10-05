using System;

using Rubicon.ObjectBinding;

namespace Rubicon.Data.DomainObjects.ObjectBinding.UnitTests.PropertyTypes
{
public class BusinessObjectWithIdentity : IBusinessObjectWithIdentity
{
	public BusinessObjectWithIdentity()
	{
  }

  #region IBusinessObjectWithIdentity Members

  public string DisplayName
  {
    get { return null; }
  }

  public string UniqueIdentifier
  {
    get { return null; }
  }

  #endregion

  #region IBusinessObject Members

  public string GetPropertyString (string propertyIdentifier)
  {
    return null;
  }

  public string GetPropertyString (IBusinessObjectProperty property, string format)
  {
    return null;
  }

  public string GetPropertyString (IBusinessObjectProperty property)
  {
    return null;
  }

  public object GetProperty(string propertyIdentifier)
  {
    return null;
  }

  public object GetProperty (IBusinessObjectProperty property)
  {
    return null;
  }

  public object this[string propertyIdentifier]
  {
    get { return null;}
    set {}
  }

  public object this[IBusinessObjectProperty property]
  {
    get { return null;}
    set {}
  }

  public void SetProperty (string propertyIdentifier, object value)
  {
  }

  public void SetProperty (IBusinessObjectProperty property, object value)
  {
  }

  public IBusinessObjectClass BusinessObjectClass
  {
    get { return null; }
  }

  #endregion
}
}
