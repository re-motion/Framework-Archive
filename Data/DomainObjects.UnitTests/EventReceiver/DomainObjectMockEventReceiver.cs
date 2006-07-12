using System;
using System.Collections.Generic;
using System.Text;
using Rubicon.Utilities;

namespace Rubicon.Data.DomainObjects.UnitTests.EventReceiver
{
  public abstract class DomainObjectMockEventReceiver
  {
    // types

    // static members and constants

    // member fields

    // construction and disposing

    public DomainObjectMockEventReceiver (DomainObject domainObject)
    {
      ArgumentUtility.CheckNotNull ("domainObject", domainObject);

      domainObject.Committed += new EventHandler (Committed);
      domainObject.Committing += new EventHandler (Committing);
      domainObject.RolledBack += new EventHandler (RolledBack);
      domainObject.RollingBack += new EventHandler (RollingBack);
      domainObject.Deleted += new EventHandler (Deleted);
      domainObject.Deleting += new EventHandler (Deleting);
      domainObject.PropertyChanged += new PropertyChangeEventHandler (PropertyChanged);
      domainObject.PropertyChanging += new PropertyChangeEventHandler (PropertyChanging);
      domainObject.RelationChanged += new RelationChangedEventHandler (RelationChanged);
      domainObject.RelationChanging += new RelationChangingEventHandler (RelationChanging);
    }

    // abstract methods and properties

    public abstract void RelationChanging (object sender, RelationChangingEventArgs args);
    public abstract void RelationChanged (object sender, RelationChangedEventArgs args);
    public abstract void PropertyChanging (object sender, PropertyChangeEventArgs args);
    public abstract void PropertyChanged (object sender, PropertyChangeEventArgs args);
    public abstract void Deleting (object sender, EventArgs e);
    public abstract void Deleted (object sender, EventArgs e);
    public abstract void Committing (object sender, EventArgs e);
    public abstract void Committed (object sender, EventArgs e);
    public abstract void RollingBack (object sender, EventArgs e);
    public abstract void RolledBack (object sender, EventArgs e);
  }
}
