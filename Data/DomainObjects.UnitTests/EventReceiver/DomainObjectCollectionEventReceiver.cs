using System;
using System.Collections;

using Rubicon.Data.DomainObjects.DataManagement;

namespace Rubicon.Data.DomainObjects.UnitTests.DomainObjects
{
public class DomainObjectCollectionEventReceiver
{
  // types

  // static members and constants

  // member fields

  private bool _cancel;

  private DomainObject _addingDomainObject;
  private DomainObject _addedDomainObject;
  private bool _hasAddingEventBeenCalled;
  private bool _hasAddedEventBeenCalled;

  private DomainObjectCollection _removingDomainObjects;
  private DomainObjectCollection _removedDomainObjects;
  private bool _hasRemovingEventBeenCalled;
  private bool _hasRemovedEventBeenCalled;

  // construction and disposing

  public DomainObjectCollectionEventReceiver (DomainObjectCollection collection)
      : this (collection, false)
  {
  }

  public DomainObjectCollectionEventReceiver (DomainObjectCollection collection, bool cancel)
  {
    _cancel = cancel;

    collection.Adding += new DomainObjectCollectionChangingEventHandler (DomainObjectCollection_Adding);
    collection.Added += new DomainObjectCollectionChangedEventHandler (DomainObjectCollection_Added);

    _removingDomainObjects = new DomainObjectCollection ();
    _removedDomainObjects = new DomainObjectCollection ();
    collection.Removing += new DomainObjectCollectionChangingEventHandler (DomainObjectCollection_Removing);
    collection.Removed += new DomainObjectCollectionChangedEventHandler (DomainObjectCollection_Removed);
  }

  // methods and properties

  public bool Cancel
  {
    get { return _cancel; }
    set { _cancel = value; }
  }

  public DomainObject AddingDomainObject
  {
    get { return _addingDomainObject; }
  }

  public DomainObject AddedDomainObject
  {
    get { return _addedDomainObject; }
  }

  public bool HasAddingEventBeenCalled
  {
    get { return _hasAddingEventBeenCalled; }
  }

  public bool HasAddedEventBeenCalled
  {
    get { return _hasAddedEventBeenCalled; }
  }

  public DomainObjectCollection RemovingDomainObjects
  {
    get { return _removingDomainObjects; }
  }

  public DomainObjectCollection RemovedDomainObjects
  {
    get { return _removedDomainObjects; }
  }

  public bool HasRemovingEventBeenCalled
  {
    get { return _hasRemovingEventBeenCalled; }
  }

  public bool HasRemovedEventBeenCalled
  {
    get { return _hasRemovedEventBeenCalled; }
  }

  private void DomainObjectCollection_Adding (object sender, DomainObjectCollectionChangingEventArgs args)
  {
    _hasAddingEventBeenCalled = true;
    _addingDomainObject = args.DomainObject;
    args.Cancel = _cancel;
  }

  private void DomainObjectCollection_Added (object sender, DomainObjectCollectionChangedEventArgs args)
  {
    _addedDomainObject = args.DomainObject;
    _hasAddedEventBeenCalled = true;
  }

  private void DomainObjectCollection_Removing (object sender, DomainObjectCollectionChangingEventArgs args)
  {
    _hasRemovingEventBeenCalled = true;
    _removingDomainObjects.Add (args.DomainObject);
    args.Cancel = _cancel;
  }

  private void DomainObjectCollection_Removed (object sender, DomainObjectCollectionChangedEventArgs args)
  {
    _removedDomainObjects.Add (args.DomainObject);
    _hasRemovedEventBeenCalled = true;
  }
}
}