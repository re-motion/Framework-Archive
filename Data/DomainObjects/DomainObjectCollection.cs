using System;
using System.Collections;

using Rubicon.Data.DomainObjects.DataManagement;

namespace Rubicon.Data.DomainObjects
{
public class DomainObjectCollection : CollectionBase, ICloneable
{
  // types

  // static members and constants


  public static DomainObjectCollection Create (Type collectionType)
  {
    return Create (collectionType, new DataContainerCollection ());
  }

  public static DomainObjectCollection Create (Type collectionType, DataContainerCollection dataContainers)
  {
    ArgumentUtility.CheckNotNull ("collectionType", collectionType);
    ArgumentUtility.CheckNotNull ("dataContainers", dataContainers);

    DomainObjectCollection domainObjects = (DomainObjectCollection) ReflectionUtility.CreateObject (collectionType);

    foreach (DataContainer dataContainer in dataContainers)
      domainObjects.Add (dataContainer.DomainObject);

    return domainObjects;
  }

  public static bool Compare (DomainObjectCollection collection1, DomainObjectCollection collection2)
  {
    if (collection1 == null && collection2 == null) return true;
    if (collection1 == null) return false;
    if (collection2 == null) return false;
    if (collection1.Count != collection2.Count) return false;

    for (int i = 0; i < collection1.Count; i++)
    {
      if (!collection1[i].Equals (collection2[i]))
        return false;
    }

    return true;
  }

  // member fields

  public event DomainObjectCollectionChangingEventHandler Adding;
  public event DomainObjectCollectionChangedEventHandler Added;

  public event DomainObjectCollectionChangingEventHandler Removing;
  public event DomainObjectCollectionChangedEventHandler Removed;

  private Type _requiredItemType;
  private ICollectionChangeDelegate _changeDelegate = null;

  // construction and disposing

  public DomainObjectCollection () : this (null)
  {
  }

  public DomainObjectCollection (Type requiredItemType)
  {
    _requiredItemType = requiredItemType;    
  }

  // standard constructor for collections
  public DomainObjectCollection (DomainObjectCollection collection, bool isCollectionReadOnly)
  {
    ArgumentUtility.CheckNotNull ("collection", collection);

    foreach (DomainObject domainObject in collection)
    {
      Add (domainObject);
    }

    this.SetIsReadOnly (isCollectionReadOnly);
    _requiredItemType = collection.RequiredItemType;    
  }

  // methods and properties

  public Type RequiredItemType
  {
    get { return _requiredItemType; }
  }

  internal ICollectionChangeDelegate ChangeDelegate
  {
    set { _changeDelegate = value; }
  }

  #region Standard implementation for collections

  public bool Contains (DomainObject domainObject)
  {
    ArgumentUtility.CheckNotNull ("domainObject", domainObject);

    return Contains (domainObject.ID);
  }

  public bool Contains (ObjectID id)
  {
    return base.ContainsKey (id);
  }

  public DomainObject this[int index]  
  {
    get { return (DomainObject) GetObject (index); }
  }

  public DomainObject this[ObjectID id]  
  {
    get { return (DomainObject) GetObject (id); }
  }

  public void Add (DomainObject value)
  {
    ArgumentUtility.CheckNotNull ("value", value);
    CheckItemType (_requiredItemType, value.GetType ());

    if (_changeDelegate != null)
    {
      _changeDelegate.PerformAdd (this, value);
    }
    else
    {
      if (BeginAdd (value))
      {
        PerformAdd (value);
        EndAdd (value);
      }
    }
  }

  public void Remove (int index)
  {
    Remove (this[index]);
  }

  public void Remove (ObjectID id)
  {
    Remove (this[id]);
  }

  public void Remove (DomainObject domainObject)
  {
    ArgumentUtility.CheckNotNull ("domainObject", domainObject);

    // Do not perform remove, if domain object is not part of this collection     
    if (this[domainObject.ID] == null)
      return;

    if (_changeDelegate != null)
    {
      _changeDelegate.PerformRemove (this, domainObject);
    }
    else
    {
      if (BeginRemove (domainObject))
      {
        PerformRemove (domainObject);
        EndRemove (domainObject);
      }
    }
  }

  public void Clear ()
  {
    for (int i = Count - 1; i >= 0; i--)
      Remove (this[i].ID);
  }

  #endregion

  #region ICloneable Members

  public virtual object Clone()
  {
    return new DomainObjectCollection (this, this.IsReadOnly);
  }

  #endregion

  internal bool BeginAdd (DomainObject domainObject)
  {
    DomainObjectCollectionChangingEventArgs addingArgs = new DomainObjectCollectionChangingEventArgs (domainObject);
    OnAdding (addingArgs);
    return !addingArgs.Cancel;
  }

  internal protected virtual void PerformAdd (DomainObject domainObject)
  {
    base.Add (domainObject.ID, domainObject);
  }

  internal void EndAdd (DomainObject domainObject)
  {
    OnAdded (new DomainObjectCollectionChangedEventArgs (domainObject));
  }

  internal bool BeginRemove (DomainObject domainObject)
  {
    DomainObjectCollectionChangingEventArgs removingArgs = 
        new DomainObjectCollectionChangingEventArgs (domainObject); 

    OnRemoving (removingArgs);
    return !removingArgs.Cancel;
  }

  internal protected virtual void PerformRemove (DomainObject domainObject)
  {
    base.Remove (domainObject.ID);
  }

  internal void EndRemove (DomainObject domainObject)
  {
    OnRemoved (new DomainObjectCollectionChangedEventArgs (domainObject));
  }

  private void CheckItemType (Type requiredType, Type itemType)
  {
    if (!ValidateItemType (requiredType, itemType))
    {
      throw new ArgumentException (string.Format (
        "Values of type '{0}' cannot be added to this collection. " + 
        "Values must be of type '{1}' or derived from '{1}'.", 
        itemType, requiredType));
    }
  }

  private bool ValidateItemType (Type requiredType, Type itemType)
  {
    if (requiredType != null)
    {
      while (itemType != null)
      {
        if (itemType == requiredType) 
          return true;

        itemType = itemType.BaseType;
      }

      return false;
    }
    else
    {
      return true;
    }
  }

  protected virtual void OnAdding (DomainObjectCollectionChangingEventArgs args)
  {
    if (Adding != null)
      Adding (this, args);
  }

  protected virtual void OnAdded (DomainObjectCollectionChangedEventArgs args)
  {
    if (Added != null)
      Added (this, args);
  }

  protected virtual void OnRemoving (DomainObjectCollectionChangingEventArgs args)
  {
    if (Removing != null)
      Removing (this, args);
  }

  protected virtual void OnRemoved (DomainObjectCollectionChangedEventArgs args)
  {
    if (Removed != null)
      Removed (this, args);
  }
}
}
