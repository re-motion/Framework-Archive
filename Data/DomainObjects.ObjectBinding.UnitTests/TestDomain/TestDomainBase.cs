using System;

namespace Rubicon.Data.DomainObjects.ObjectBinding.UnitTests.TestDomain
{
[Serializable]
public class TestDomainBase : BindableDomainObject
{
  // types

  // static members and constants

  // member fields

  // construction and disposing

  protected TestDomainBase ()
  {
  }

	protected TestDomainBase (ClientTransaction clientTransaction) : base (clientTransaction)
	{
	}

  protected TestDomainBase (DataContainer dataContainer) : base (dataContainer)
  {
  }

  // methods and properties

//  public new DomainObject GetRelatedObject (string propertyName)
//  {
//    return base.GetRelatedObject (propertyName);
//  }
//
//  public new DomainObjectCollection GetRelatedObjects (string propertyName)
//  {
//    return base.GetRelatedObjects (propertyName);
//  }
//
//  public new DomainObject GetOriginalRelatedObject (string propertyName)
//  {
//    return base.GetOriginalRelatedObject (propertyName);
//  }
//
//  public new DomainObjectCollection GetOriginalRelatedObjects (string propertyName)
//  {
//    return base.GetOriginalRelatedObjects (propertyName);
//  }
//
//  public new void SetRelatedObject (string propertyName, DomainObject newRelatedObject)
//  {
//    base.SetRelatedObject (propertyName, newRelatedObject);
//  }
}
}
