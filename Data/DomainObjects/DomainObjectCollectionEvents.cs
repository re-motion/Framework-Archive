using System;
using System.ComponentModel;

using Rubicon.Data.DomainObjects.DataManagement;
using Rubicon.Utilities;

namespace Rubicon.Data.DomainObjects
{
public delegate void DomainObjectCollectionChangingEventHandler (object sender, DomainObjectCollectionChangingEventArgs args);
public delegate void DomainObjectCollectionChangedEventHandler (object sender, DomainObjectCollectionChangedEventArgs args);

public class DomainObjectCollectionChangingEventArgs : CancelEventArgs
{
  private DomainObject _domainObject;

  public DomainObjectCollectionChangingEventArgs (DomainObject domainObject) : this (domainObject, false)
  {
  }

  public DomainObjectCollectionChangingEventArgs (DomainObject domainObject, bool cancel) : base (cancel)
  {
    ArgumentUtility.CheckNotNull ("domainObject", domainObject);
    _domainObject = domainObject;
  }

  public DomainObject DomainObject
  {
    get { return _domainObject; }
  }
}

public class DomainObjectCollectionChangedEventArgs : EventArgs
{
  private DomainObject _domainObject;

  public DomainObjectCollectionChangedEventArgs (DomainObject domainObject)
  {
    ArgumentUtility.CheckNotNull ("domainObject", domainObject);
    _domainObject = domainObject;
  }

  public DomainObject DomainObject
  {
    get { return _domainObject; }
  }
}
}
