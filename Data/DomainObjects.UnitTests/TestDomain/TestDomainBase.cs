using System;

using Rubicon.Data.DomainObjects.DataManagement;

namespace Rubicon.Data.DomainObjects.UnitTests.TestDomain
{
public class TestDomainBase : DomainObject
{
  // types

  // static members and constants

  // member fields

  // construction and disposing

  protected TestDomainBase ()
  {
  }

  protected TestDomainBase (DataContainer dataContainer) : base (dataContainer)
  {
  }

  // methods and properties

  public new DataContainer DataContainer 
  {
    get { return base.DataContainer; }
  }

  public new void Delete ()
  {
    base.Delete ();
  }

  public new DomainObject GetOriginalRelatedObject (string propertyName)
  {
    return base.GetOriginalRelatedObject (propertyName);
  }

  public new DomainObjectCollection GetOriginalRelatedObjects (string propertyName)
  {
    return base.GetOriginalRelatedObjects (propertyName);
  }
}
}
