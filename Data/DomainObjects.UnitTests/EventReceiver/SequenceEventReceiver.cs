using System;
using System.Collections;
using NUnit.Framework;

using Rubicon.Utilities;

namespace Rubicon.Data.DomainObjects.UnitTests.EventSequence
{
public class SequenceEventReceiver
{
  // types

  // static members and constants

  // member fields

  private DomainObject[] _domainObjects;
  private DomainObjectCollection[] _collections;
  private ArrayList _states = new ArrayList ();
  private int _cancelEventNumber = 0;

  // construction and disposing

  public SequenceEventReceiver (DomainObjectCollection collection) 
      : this (new DomainObject[0], new DomainObjectCollection[] {collection})
  {
  }

  public SequenceEventReceiver (DomainObject domainObject) 
      : this (new DomainObject[] {domainObject}, new DomainObjectCollection[0])
  {
  }

  public SequenceEventReceiver (DomainObject[] domainObjects, DomainObjectCollection[] collections) 
      : this (domainObjects, collections, 0)
  {
  }

  public SequenceEventReceiver (DomainObjectCollection collection, int cancelEventNumber) 
      : this (new DomainObject[0], new DomainObjectCollection[] {collection}, cancelEventNumber)
  {
  }

  public SequenceEventReceiver (DomainObject[] domainObjects, DomainObjectCollection[] collections, int cancelEventNumber)
  {
    ArgumentUtility.CheckNotNull ("domainObjects", domainObjects);
    ArgumentUtility.CheckNotNull ("collections", collections);

    _domainObjects = domainObjects;
    _collections = collections;
    _cancelEventNumber = cancelEventNumber;

    foreach (DomainObject domainObject in domainObjects)
    {
      domainObject.Deleting += new DeletingEventHandler (DomainObject_Deleting);
      domainObject.Deleted += new EventHandler (DomainObject_Deleted);
      domainObject.PropertyChanging += new PropertyChangingEventHandler (DomainObject_PropertyChanging);
      domainObject.PropertyChanged += new PropertyChangedEventHandler (DomainObject_PropertyChanged);
      domainObject.RelationChanging += new RelationChangingEventHandler (DomainObject_RelationChanging);
      domainObject.RelationChanged += new RelationChangedEventHandler (DomainObject_RelationChanged);
    }

    foreach (DomainObjectCollection collection in collections)
    {
      collection.Adding += new DomainObjectCollectionChangingEventHandler (Collection_Changing);
      collection.Added += new DomainObjectCollectionChangedEventHandler (Collection_Changed);
      collection.Removing += new DomainObjectCollectionChangingEventHandler (Collection_Changing);
      collection.Removed += new DomainObjectCollectionChangedEventHandler (Collection_Changed);
    }
  }

  // methods and properties

  public int CancelEventNumber
  {
    get { return _cancelEventNumber; }
    set { _cancelEventNumber = value; }
  }

  public ChangeState this[int index]
  {
    get { return (ChangeState) _states[index]; }    
  }

  public int Count 
  {
    get { return _states.Count; }
  }

  public void Compare (ChangeState[] states)
  {
    for (int i = 0; i < states.Length; i++)
    {
      if (i >= _states.Count)
        Assert.Fail ("Missing event: " + states[i].Message);

      Assert.IsTrue (((ChangeState) _states[i]).Compare (states[i]), states[i].Message);
    }

    Assert.AreEqual (states.Length, _states.Count, "Length");
  }

  public void Unregister ()
  {
    foreach (DomainObject domainObject in _domainObjects)
    {
      domainObject.Deleting -= new DeletingEventHandler (DomainObject_Deleting);
      domainObject.Deleted -= new EventHandler (DomainObject_Deleted);
      domainObject.PropertyChanging -= new PropertyChangingEventHandler (DomainObject_PropertyChanging);
      domainObject.PropertyChanged -= new PropertyChangedEventHandler (DomainObject_PropertyChanged);
      domainObject.RelationChanging -= new RelationChangingEventHandler (DomainObject_RelationChanging);
      domainObject.RelationChanged -= new RelationChangedEventHandler (DomainObject_RelationChanged);
    }

    foreach (DomainObjectCollection collection in _collections)
    {
      collection.Adding -= new DomainObjectCollectionChangingEventHandler (Collection_Changing);
      collection.Added -= new DomainObjectCollectionChangedEventHandler (Collection_Changed);
      collection.Removing -= new DomainObjectCollectionChangingEventHandler (Collection_Changing);
      collection.Removed -= new DomainObjectCollectionChangedEventHandler (Collection_Changed);
    }
  }

  private void DomainObject_PropertyChanged (object sender, PropertyChangedEventArgs args)
  {
    _states.Add (new PropertyChangeState (sender, args.PropertyValue, null, null));
  }

  private void DomainObject_PropertyChanging(object sender, PropertyChangingEventArgs args)
  {
    _states.Add (new PropertyChangeState (sender, args.PropertyValue, args.OldValue, args.NewValue));

    if (_states.Count == _cancelEventNumber)
      args.Cancel = true;
  }

  private void DomainObject_RelationChanging (object sender, RelationChangingEventArgs args)
  {
    _states.Add (new RelationChangeState (sender, args.PropertyName, args.OldRelatedObject, args.NewRelatedObject));

    if (_states.Count == _cancelEventNumber)
      args.Cancel = true;
  }

  private void DomainObject_RelationChanged (object sender, RelationChangedEventArgs args)
  {
    _states.Add (new RelationChangeState (sender, args.PropertyName, null, null));
  }

  private void DomainObject_Deleting (object sender, DeletingEventArgs args)
  {
    _states.Add (new ObjectDeletionState (sender));

    if (_states.Count == _cancelEventNumber)
      args.Cancel = true;
  }

  private void DomainObject_Deleted (object sender, EventArgs args)
  {
    _states.Add (new ObjectDeletionState (sender));
  }

  private void Collection_Changing (object sender, DomainObjectCollectionChangingEventArgs args)
  {
    _states.Add (new CollectionChangeState (sender, args.DomainObject));

    if (_states.Count == _cancelEventNumber)
      args.Cancel = true;
  }

  private void Collection_Changed (object sender, DomainObjectCollectionChangedEventArgs args)
  {
    _states.Add (new CollectionChangeState (sender, args.DomainObject));
  }
}
}
