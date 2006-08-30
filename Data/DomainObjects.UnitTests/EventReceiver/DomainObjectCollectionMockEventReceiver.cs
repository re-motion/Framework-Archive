using System;
using System.Collections.Generic;
using System.Text;
using Rubicon.Utilities;

namespace Rubicon.Data.DomainObjects.UnitTests.EventReceiver
{
  public abstract class DomainObjectCollectionMockEventReceiver
  {
    // types

    // static members and constants

    // member fields

    // construction and disposing

    public DomainObjectCollectionMockEventReceiver (DomainObjectCollection domainObjectCollection)
    {
      ArgumentUtility.CheckNotNull ("domainObjectCollection", domainObjectCollection);

      domainObjectCollection.Added += new DomainObjectCollectionChangeEventHandler (Added);
      domainObjectCollection.Adding += new DomainObjectCollectionChangeEventHandler (Adding);
      domainObjectCollection.Removed += new DomainObjectCollectionChangeEventHandler (Removed);
      domainObjectCollection.Removing += new DomainObjectCollectionChangeEventHandler (Removing);
    }

    // abstract methods and properties

    public abstract void Added (object sender, DomainObjectCollectionChangeEventArgs args);
    public abstract void Adding (object sender, DomainObjectCollectionChangeEventArgs args);
    public abstract void Removed (object sender, DomainObjectCollectionChangeEventArgs args);
    public abstract void Removing (object sender, DomainObjectCollectionChangeEventArgs args);

  }
}
