using System;
using System.Collections;
using Rubicon.Data.DomainObjects.Mapping;
using Rubicon.Data.DomainObjects.Legacy.UnitTests.TestDomain;

namespace Rubicon.Data.DomainObjects.Legacy.UnitTests.EventReceiver
{
  public class DomainObjectRelationCheckEventReceiver : DomainObjectEventReceiver
  {
    // types

    // static members and constants

    // member fields
    private Hashtable changingRelatedObjects;
    private Hashtable changedRelatedObjects;
    private StateType changingObjectState;
    private StateType changedObjectState;

    // construction and disposing

    public DomainObjectRelationCheckEventReceiver (DomainObject domainObject)
      : this (domainObject, false)
    {
    }

    public DomainObjectRelationCheckEventReceiver (DomainObject domainObject, bool cancel)
      : base (domainObject, cancel)
    {
      changingRelatedObjects = new Hashtable ();
      changedRelatedObjects = new Hashtable ();
    }

    // methods and properties

    public DomainObject GetChangingRelatedDomainObject (string propertyName)
    {
      return (DomainObject) changingRelatedObjects[propertyName];
    }

    public DomainObject GetChangedRelatedDomainObject (string propertyName)
    {
      return (DomainObject) changedRelatedObjects[propertyName];
    }

    protected override void DomainObject_RelationChanging (object sender, RelationChangingEventArgs args)
    {
      TestDomainBase domainObject = (TestDomainBase) sender;

      changingObjectState = domainObject.State;

      string changedProperty = args.PropertyName;

      if (CardinalityType.One == domainObject.DataContainer.ClassDefinition.GetRelationEndPointDefinition (changedProperty).Cardinality)
      {
        DomainObject relatedDomainObject = domainObject.GetRelatedObject (changedProperty);
        changingRelatedObjects.Add (changedProperty, relatedDomainObject);
      }
      else
      {
        DomainObjectCollection relatedDomainObjectCollection = domainObject.GetRelatedObjects (changedProperty);
        changingRelatedObjects.Add (changedProperty, relatedDomainObjectCollection.Clone (true));
      }

      base.DomainObject_RelationChanging (sender, args);
    }

    protected override void DomainObject_RelationChanged (object sender, RelationChangedEventArgs args)
    {
      TestDomainBase domainObject = (TestDomainBase) sender;

      changedObjectState = domainObject.State;

      string changedProperty = args.PropertyName;

      if (CardinalityType.One == domainObject.DataContainer.ClassDefinition.GetRelationEndPointDefinition (changedProperty).Cardinality)
      {
        DomainObject relatedDomainObject = domainObject.GetRelatedObject (changedProperty);
        changedRelatedObjects.Add (changedProperty, relatedDomainObject);
      }
      else
      {
        DomainObjectCollection relatedDomainObjectCollection = domainObject.GetRelatedObjects (changedProperty);
        changedRelatedObjects.Add (changedProperty, relatedDomainObjectCollection.Clone (true));
      }

      base.DomainObject_RelationChanged (sender, args);
    }
  }
}
