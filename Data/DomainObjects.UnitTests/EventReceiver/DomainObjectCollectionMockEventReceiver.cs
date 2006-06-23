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

      domainObjectCollection.Added += new DomainObjectCollectionChangedEventHandler (Added);
      domainObjectCollection.Adding += new DomainObjectCollectionChangingEventHandler (Adding);
      domainObjectCollection.Removed += new DomainObjectCollectionChangedEventHandler (Removed);
      domainObjectCollection.Removing += new DomainObjectCollectionChangingEventHandler (Removing);
    }

    // abstract methods and properties

    public abstract void Added (object sender, DomainObjectCollectionChangedEventArgs args);
    public abstract void Adding (object sender, DomainObjectCollectionChangingEventArgs args);
    public abstract void Removed (object sender, DomainObjectCollectionChangedEventArgs args);
    public abstract void Removing (object sender, DomainObjectCollectionChangingEventArgs args);

  }
}
