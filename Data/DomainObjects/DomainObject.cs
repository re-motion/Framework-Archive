using System;

using Rubicon.Data.DomainObjects.Configuration.Mapping;
using Rubicon.Data.DomainObjects.DataManagement;

namespace Rubicon.Data.DomainObjects
{
public class DomainObject
{
  // types

  // static members and constants

  protected static DomainObject GetObject (ObjectID id)
  {
    return GetObject (id, false);
  }

  protected static DomainObject GetObject (ObjectID id, bool includeDeleted)
  {
    ArgumentUtility.CheckNotNull ("id", id);
    return ClientTransaction.Current.GetObject (id, includeDeleted);
  }
 
  internal static DomainObject Create (DataContainer dataContainer)
  {
    ArgumentUtility.CheckNotNull ("dataContainer", dataContainer);

    return (DomainObject) ReflectionUtility.CreateObject (dataContainer.DomainObjectType, dataContainer);
  }

  // member fields

  public event PropertyChangingEventHandler PropertyChanging;
  public event PropertyChangedEventHandler PropertyChanged;

  public event RelationChangingEventHandler RelationChanging;
  public event RelationChangedEventHandler RelationChanged;

  public event DeletingEventHandler Deleting;
  public event EventHandler Deleted;

  public event EventHandler Committed;

  private DataContainer _dataContainer;

  // construction and disposing

  protected DomainObject ()
  {
    _dataContainer = ClientTransaction.Current.CreateNewDataContainer (this.GetType ());
    _dataContainer.SetDomainObject (this);
    RegisterDataContainerEvents ();
  }

  protected DomainObject (DataContainer dataContainer)
  {
    ArgumentUtility.CheckNotNull ("dataContainer", dataContainer);

    _dataContainer = dataContainer;
    RegisterDataContainerEvents ();
  }

  // methods and properties

  public ObjectID ID
  {
    get 
    {
      CheckDiscarded ();
      return _dataContainer.ID; 
    }
  }

  public StateType State
  {
    get
    {
      CheckDiscarded ();
      if (_dataContainer.State == StateType.Unchanged)
      {
        if (ClientTransaction.Current.HasRelationChanged (this))
          return StateType.Changed;
        else
          return StateType.Unchanged;
      }

      return _dataContainer.State;
    }
  }

  protected internal DataContainer DataContainer
  {
    get 
    { 
      CheckDiscarded ();
      return _dataContainer; 
    }
  }

  protected virtual void Delete ()
  {
    CheckDiscarded ();
    ClientTransaction.Current.Delete (this);
  }

  protected virtual DomainObject GetRelatedObject (string propertyName)
  {
    ArgumentUtility.CheckNotNullOrEmpty ("propertyName", propertyName);
    CheckDiscarded ();

    return ClientTransaction.Current.GetRelatedObject (new RelationEndPointID (ID, propertyName));
  }

  protected virtual DomainObject GetOriginalRelatedObject (string propertyName)
  {
    ArgumentUtility.CheckNotNullOrEmpty ("propertyName", propertyName);
    CheckDiscarded ();

    return ClientTransaction.Current.GetOriginalRelatedObject (new RelationEndPointID (ID, propertyName));
  }

  protected virtual DomainObjectCollection GetOriginalRelatedObjects (string propertyName)
  {
    ArgumentUtility.CheckNotNullOrEmpty ("propertyName", propertyName);
    CheckDiscarded ();

    return ClientTransaction.Current.GetOriginalRelatedObjects (new RelationEndPointID (ID, propertyName));
  }

  protected virtual DomainObjectCollection GetRelatedObjects (string propertyName)
  {
    ArgumentUtility.CheckNotNullOrEmpty ("propertyName", propertyName);
    CheckDiscarded ();

    return ClientTransaction.Current.GetRelatedObjects (new RelationEndPointID (ID, propertyName));
  }

  protected virtual void SetRelatedObject (string propertyName, DomainObject newRelatedObject)
  {
    ArgumentUtility.CheckNotNullOrEmpty ("propertyName", propertyName);
    CheckDiscarded ();

    ClientTransaction.Current.SetRelatedObject (new RelationEndPointID (ID, propertyName), newRelatedObject);
  }

  protected virtual void OnLoaded ()
  {
  }

  protected virtual void OnRelationChanging (RelationChangingEventArgs args)
  {
    if (RelationChanging != null)
      RelationChanging (this, args);
  }

  protected virtual void OnRelationChanged (RelationChangedEventArgs args)
  {
    if (RelationChanged != null)
      RelationChanged (this, args);
  }

  protected virtual void OnPropertyChanging (PropertyChangingEventArgs args)
  {
    if (PropertyChanging != null)
      PropertyChanging (this, args);
  }

  protected virtual void OnPropertyChanged (PropertyChangedEventArgs args)
  {
    if (PropertyChanged != null)
      PropertyChanged (this, args);
  }

  protected virtual void OnDeleting (DeletingEventArgs args)
  {
    if (Deleting != null)
      Deleting (this, args);
  }

  protected virtual void OnDeleted (EventArgs args)
  {
    if (Deleted != null)
      Deleted (this, args);
  }

  protected virtual void OnCommitted (EventArgs args)
  {
    if (Committed != null)
      Committed (this, args);
  }

  internal bool BeginRelationChange (
    string propertyName,
    DomainObject oldRelatedObject,
    DomainObject newRelatedObject)
  {
    ArgumentUtility.CheckNotNullOrEmpty ("propertyName", propertyName);

    RelationChangingEventArgs args = new RelationChangingEventArgs (propertyName, oldRelatedObject, newRelatedObject);
    OnRelationChanging (args);
    return !args.Cancel;
  }

  internal void EndObjectLoading ()
  {
    OnLoaded ();
  }

  internal void EndCommit ()
  {
    OnCommitted (new EventArgs ());
  }

  internal void EndRelationChange (string propertyName)
  {
    ArgumentUtility.CheckNotNullOrEmpty ("propertyName", propertyName);

    OnRelationChanged (new RelationChangedEventArgs (propertyName));
  }

  internal bool BeginDelete ()
  {
    DeletingEventArgs args = new DeletingEventArgs ();
    OnDeleting (args);
    return !args.Cancel;
  }

  internal void EndDelete ()
  {
    EventArgs args = new EventArgs ();
    OnDeleted (args);
  }

  private void DataContainer_PropertyChanging (object sender, PropertyChangingEventArgs args)
  {
    OnPropertyChanging (args);
  }

  private void DataContainer_PropertyChanged (object sender, PropertyChangedEventArgs args)
  {
    OnPropertyChanged (args);
  }

  private void RegisterDataContainerEvents ()
  {
    _dataContainer.PropertyChanging += new PropertyChangingEventHandler (DataContainer_PropertyChanging);
    _dataContainer.PropertyChanged += new PropertyChangedEventHandler (DataContainer_PropertyChanged);
  }

  private void CheckDiscarded ()
  {
    if (_dataContainer.IsDiscarded)
      throw new ObjectDiscardedException (_dataContainer.GetID ());
  }
}
}