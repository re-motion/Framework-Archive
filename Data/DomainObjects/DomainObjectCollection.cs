using System;
using System.Collections;

using Rubicon.Data.DomainObjects.DataManagement;
using Rubicon.Utilities;

namespace Rubicon.Data.DomainObjects
{
//TODO documentation: Check entire class
public class DomainObjectCollection : CollectionBase, ICloneable, IList
{
  // types

  // static members and constants

  /// <summary>
  /// Creates an empty <see cref="DomainObjectCollection"/> of a given <see cref="Type"/>.
  /// </summary>
  /// <param name="collectionType">The <see cref="Type"/> of the new collection that should be instantiated.</param>
  /// <returns>The new <see cref="DomainObjectCollection"/>.</returns>
  /// <exception cref="System.InvalidCastException"><i>collectionType</i> cannot be casted to <see cref="DomainObjectCollection"/>.</exception>
  public static DomainObjectCollection Create (Type collectionType)
  {
    return Create (collectionType, new DataContainerCollection ());
  }

  /// <summary>
  /// Creates a <see cref="DomainObjectCollection"/> of a given <see cref="System.Type"/> and adds the <see cref="DomainObject"/>s of the given <see cref="DataContainerCollection"/>.
  /// </summary>
  /// <param name="collectionType">The <see cref="Type"/> of the new collection that should be instantiated.</param>
  /// <param name="dataContainers">The <see cref="DataContainer"/>s of the <see cref="DomainObject"/>s that are added to the collection.</param>
  /// <returns>The new <see cref="DomainObjectCollection"/>.</returns>
  /// <exception cref="System.InvalidCastException"><i>collectionType</i> cannot be casted to <see cref="DomainObjectCollection"/>.</exception>
  public static DomainObjectCollection Create (Type collectionType, DataContainerCollection dataContainers)
  {
    ArgumentUtility.CheckNotNull ("collectionType", collectionType);
    ArgumentUtility.CheckNotNull ("dataContainers", dataContainers);

    DomainObjectCollection domainObjects = (DomainObjectCollection) ReflectionUtility.CreateObject (collectionType);

    foreach (DataContainer dataContainer in dataContainers)
      domainObjects.Add (dataContainer.DomainObject);

    return domainObjects;
  }

  /// <summary>
  /// Compares two instances of <see cref="DomainObjectCollection"/> for equality.
  /// </summary>
  /// <param name="collection1">The first <see cref="DomainObjectCollection"/>.</param>
  /// <param name="collection2">The second <see cref="DomainObjectCollection"/>.</param>
  /// <returns><b>true</b> if the collections are equal; otherwise, <b>false</b>.</returns>
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

  /// <summary>
  /// Occurs before an object is added to the collection.
  /// </summary>
  public event DomainObjectCollectionChangingEventHandler Adding;
  /// <summary>
  /// Occurs after an object is added to the collection.
  /// </summary>
  public event DomainObjectCollectionChangedEventHandler Added;

  /// <summary>
  /// Occurs before an object is removed to the collection.
  /// </summary>
  public event DomainObjectCollectionChangingEventHandler Removing;
  /// <summary>
  /// Occurs after an object is removed to the collection.
  /// </summary>
  public event DomainObjectCollectionChangedEventHandler Removed;

  private Type _requiredItemType;
  private ICollectionChangeDelegate _changeDelegate = null;

  // construction and disposing

  /// <summary>
  /// Initializes a new <b>DomainObjectCollection</b>.
  /// </summary>
  public DomainObjectCollection () : this (null)
  {
  }

  /// <summary>
  /// Initializes a new <b>DomainObjectCollection</b> that only takes a certain <see cref="Type"/> as members.
  /// </summary>
  /// <param name="requiredItemType">The <see cref="Type"/> that are required for members.</param>
  public DomainObjectCollection (Type requiredItemType)
  {
    _requiredItemType = requiredItemType;    
  }

  // standard constructor for collections
  /// <summary>
  /// Initializes a new <b>DomainObjectCollection</b> as a shallow copy of a given <see cref="DomainObjectCollection"/>.
  /// </summary>
  /// <remarks>
  /// The new <b>DomainObjectCollection</b> has the same <see cref="RequiredItemType"/> and the same elements as the 
  /// given <i>collection</i>.
  /// </remarks>
  /// <param name="collection">The <see cref="DomainObjectCollection"/> to copy.</param>
  /// <param name="isCollectionReadOnly">Indicates wheather the new collection should be read only.</param>
  /// <exception cref="System.ArgumentNullException"><i>collection</i> is a null reference.</exception>
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

  /// <summary>
  /// Gets the required <see cref="Type"/> for all members of the collection.
  /// </summary>
  public Type RequiredItemType
  {
    get { return _requiredItemType; }
  }

  internal ICollectionChangeDelegate ChangeDelegate
  {
    set { _changeDelegate = value; }
  }

  /// <summary>
  /// Determines whether and element is in the <see cref="DomainObjectCollection"/>.
  /// </summary>
  /// <param name="domainObject">The <see cref="DomainObject"/> to locate in the <see cref="DomainObjectCollection"/>.</param>
  /// <returns><b>true</b> if <i>domainObject</i> is found in the <see cref="DomainObjectCollection"/>; otherwise, false;</returns>
  /// <exception cref="System.ArgumentNullException"><i>domainObject</i> is a null reference</exception>
  public bool Contains (DomainObject domainObject)
  {
    ArgumentUtility.CheckNotNull ("domainObject", domainObject);

    return Contains (domainObject.ID);
  }

  /// <summary>
  /// Determines whether and element is in the <see cref="DomainObjectCollection"/>.
  /// </summary>
  /// <param name="id">The <see cref="ObjectID"/> of the <see cref="DomainObject"/> to locate in the <see cref="DomainObjectCollection"/>.</param>
  /// <returns><b>true</b> if the <see cref="DomainObject"/> with the <see cref="ObjectID"/> <i>id</i> is found in the <see cref="DomainObjectCollection"/>; otherwise, false;</returns>
  /// <exception cref="System.ArgumentNullException"><i>id</i> is a null reference</exception>
  public bool Contains (ObjectID id)
  {
    ArgumentUtility.CheckNotNull ("id", id);
    return base.ContainsKey (id);
  }

  /// <summary>
  /// Gets the <see cref="DomainObject"/> with a given <i>index</i> in the <see cref="DomainObjectCollection"/>.
  /// </summary>
  public DomainObject this[int index]  
  {
    get 
    { 
      return (DomainObject) GetObject (index); 
    }
    set 
    {
      // TODO: Check index for validity

      if (_changeDelegate != null)
      {
        _changeDelegate.PerformReplace (this, value, index);
      }
      else
      {
        DomainObject oldObject = this[index];

        if (BeginRemove (oldObject) && BeginAdd (value))
        {
          PerformRemove (oldObject);
          PerformInsert (index, value);

          EndRemove (oldObject);
          EndAdd (value);
        }
      }
    }
  }

  /// <summary>
  /// Gets the <see cref="DomainObject"/> with a given <see cref="ObjectID"/> from the <see cref="DomainObjectCollection"/>.
  /// </summary>
  public DomainObject this[ObjectID id]  
  {
    get { return (DomainObject) GetObject (id); }
  }

  //TODO Documentation: Return type changed
  /// <summary>
  /// Adds a <see cref="DomainObject"/> to the collection.
  /// </summary>
  /// <param name="domainObject">The <see cref="DomainObject"/> to add.</param>
  /// <exception cref="System.ArgumentNullException"><i>value</i> is a null reference.</exception>
  /// <exception cref="System.ArgumentException"><i>value</i> is not of type <see cref="RequiredItemType"/> or one of its derived types.</exception>
  public int Add (DomainObject domainObject)
  {
    ArgumentUtility.CheckNotNull ("domainObject", domainObject);

    if (_changeDelegate != null)
    {
      _changeDelegate.PerformAdd (this, domainObject);
    }
    else
    {
      if (BeginAdd (domainObject))
      {
        PerformAdd (domainObject);
        EndAdd (domainObject);
      }
    }

    return Count - 1;
  }

//TODO documentation: ArgumentOutOfRangeException?
//TODO documentation: ArgumentNullException?
  /// <summary>
  /// Removes a <see cref="DomainObject"/> from the collection.
  /// </summary>
  /// <param name="index">The index of the <see cref="DomainObject"/> to remove.</param>
  public void RemoveAt (int index)
  {
    Remove (this[index]);
  }

//TODO documentation: ArgumentNullException?
  /// <summary>
  /// Removes a <see cref="DomainObject"/> from the collection.
  /// </summary>
  /// <param name="id">The <see cref="ObjectID"/> of the <see cref="DomainObject"/> to remove.</param>
  public void Remove (ObjectID id)
  {
    Remove (this[id]);
  }

  /// <summary>
  /// Removes a <see cref="DomainObject"/> from the collection.
  /// </summary>
  /// <param name="domainObject">The <see cref="DomainObject"/> to remove.</param>
  /// <exception cref="System.ArgumentNullException"><i>domainObject</i> is a null reference.</exception>
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

  /// <summary>
  /// Removes all elements from the <see cref="DomainObjectCollection"/>.
  /// </summary>
  public void Clear ()
  {
    for (int i = Count - 1; i >= 0; i--)
      Remove (this[i].ID);
  }

  // TODO Documentation:
  public int IndexOf (DomainObject domainObject)
  {
    ObjectID id = null;

    if (domainObject != null)
      id = domainObject.ID;

    return IndexOf (id);
  }

  // TODO Documentation:
  public int IndexOf (ObjectID id)
  {
    return base.IndexOfKey (id);
  }

  // TODO Documentation:
  public void Insert (int index, DomainObject domainObject)
  {
    ArgumentUtility.CheckNotNull ("domainObject", domainObject);
    CheckIndex ("index", index);

    if (_changeDelegate != null)
    {
      _changeDelegate.PerformInsert (this, domainObject, index);
    }
    else
    {
      if (BeginAdd (domainObject))
      {
        PerformInsert (index, domainObject);
        EndAdd (domainObject);
      }
    }
  }

  // TODO Documentation:
  public bool IsFixedSize
  {
    get { return false; }
  }

  #region Explicitly implemeted IList Members

  // TODO Documentation: Check all explicitly implemented IList members if documentation is inherited.
  object IList.this[int index]
  {
    get 
    { 
      return this[index]; 
    }
    set 
    {
      ArgumentUtility.CheckNotNullAndType ("value", value, typeof (DomainObject));

      this[index] = (DomainObject) value; 
    } 
  }

  // TODO Documentation: Behavior for invalid object type.
  void IList.Insert (int index, object value)
  {
    ArgumentUtility.CheckNotNullAndType ("value", value, typeof (DomainObject));

    Insert (index, (DomainObject) value);
  }

  // TODO Documentation: Behavior for invalid object type.
  void IList.Remove (object value)
  {
    if (value is DomainObject)
      Remove ((DomainObject) value);

    if (value is ObjectID)
      Remove ((ObjectID) value);
  }

  // TODO Documentation: Behavior for invalid object type.
  bool IList.Contains (object value)
  {
    if (value is DomainObject)
      return Contains ((DomainObject) value);

    if (value is ObjectID)
      return Contains ((ObjectID) value);

    return false;
  }

  // TODO Documentation: Behavior for invalid object type.
  int IList.IndexOf (object value)
  {
    if (value is DomainObject)
      return IndexOf ((DomainObject) value);

    if (value is ObjectID)
      return IndexOf ((ObjectID) value);

    return -1;
  }

  // TODO Documentation: Behavior for invalid object type.
  int IList.Add (object value)
  {
    ArgumentUtility.CheckNotNullAndType ("value", value, typeof (DomainObject));

    return Add ((DomainObject) value);
  }

  #endregion

  #region ICloneable Members


  /// <summary>
  /// Creates a shallow copy of this collection.
  /// </summary>
  /// <returns>The cloned collection.</returns>
  /// <remarks>
  /// If this collection is read-only, the clone will be read-only too. 
  /// If this collection is not read-only, the clone will not be read-only too.<br/><br/>
  /// A shallow copy creates a new <see cref="DomainObjectCollection"/> instance
  /// which can be independently modified without affecting the original collection.
  /// Thus meaning the references to the domain objects are copied, not the domain objects themselves.
  /// </remarks>
  public object Clone ()
  {
    return Clone (this.IsReadOnly);
  }

  /// <summary>
  /// Creates a shallow copy of this collection. Must be overridden in derived classes.
  /// </summary>
  /// <param name="makeCloneReadOnly">Specifies whether the cloned collection should be read-only.</param>
  /// <returns>The cloned collection.</returns>
  /// <remarks>
  /// A shallow copy creates a new <see cref="DomainObjectCollection"/> instance
  /// which can be independently modified without affecting the original collection.
  /// Thus meaning the references to the domain objects are copied, not the domain objects themselves. 
  /// </remarks>
  public virtual DomainObjectCollection Clone (bool makeCloneReadOnly)
  {
    return new DomainObjectCollection (this, makeCloneReadOnly);
  }

  #endregion

  internal bool BeginAdd (DomainObject domainObject)
  {
    DomainObjectCollectionChangingEventArgs addingArgs = new DomainObjectCollectionChangingEventArgs (domainObject);
    OnAdding (addingArgs);
    return !addingArgs.Cancel;
  }

  /// <summary>
  /// Adds a <see cref="DomainObject"/> to the collection without raising the <see cref="Adding"/> and <see cref="Added"/> events.
  /// </summary>
  /// <param name="domainObject">The <see cref="DomainObject"/> to add to the collection.</param>
  /// <exception cref="System.ArgumentNullException"><i>domainObject</i> is a null reference.</exception>
  internal protected void PerformAdd (DomainObject domainObject)
  {
//TODO: Added by ES, check with ML
    ArgumentUtility.CheckNotNull ("domainObject", domainObject);
    CheckItemType (_requiredItemType, domainObject.GetType ());

    base.Add (domainObject.ID, domainObject);
  }

  internal protected void PerformInsert (int index, DomainObject domainObject)
  {
    ArgumentUtility.CheckNotNull ("domainObject", domainObject);
    CheckItemType (_requiredItemType, domainObject.GetType ());

    base.Insert (index, domainObject.ID, domainObject);
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

  /// <summary>
  /// Removes a <see cref="DomainObject"/> from the collection without raising the <see cref="Removing"/> and <see cref="Removed"/> events.
  /// </summary>
  /// <param name="domainObject">The <see cref="DomainObject"/> to remove from the collection.</param>
  /// <exception cref="System.ArgumentNullException"><i>domainObject</i> is a null reference.</exception>
  internal protected void PerformRemove (DomainObject domainObject)
  {
//TODO: Added by ES, check with ML
    ArgumentUtility.CheckNotNull ("domainObject", domainObject);

    base.Remove (domainObject.ID);
  }

  internal void EndRemove (DomainObject domainObject)
  {
    OnRemoved (new DomainObjectCollectionChangedEventArgs (domainObject));
  }

  /// <summary>
  /// Clears the <see cref="DomainObjectCollection"/> without raising the <see cref="Removing"/> and <see cref="Removed"/> events.
  /// </summary>
  internal protected new void ClearCollection ()
  {
    base.ClearCollection ();
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

  /// <summary>
  /// Raises the <see cref="Adding"/> event.
  /// </summary>
  /// <param name="args">A <see cref="DomainObjectCollectionChangingEventArgs"/> object that contains the event data.</param>
  protected virtual void OnAdding (DomainObjectCollectionChangingEventArgs args)
  {
    if (Adding != null)
      Adding (this, args);
  }

  /// <summary>
  /// Raises the <see cref="Added"/> event.
  /// </summary>
  /// <param name="args">A <see cref="DomainObjectCollectionChangedEventArgs"/> object that contains the event data.</param>
  protected virtual void OnAdded (DomainObjectCollectionChangedEventArgs args)
  {
    if (Added != null)
      Added (this, args);
  }

  /// <summary>
  /// Raises the <see cref="Removing"/> event.
  /// </summary>
  /// <param name="args">A <see cref="DomainObjectCollectionChangingEventArgs"/> object that contains the event data.</param>
  protected virtual void OnRemoving (DomainObjectCollectionChangingEventArgs args)
  {
    if (Removing != null)
      Removing (this, args);
  }

  /// <summary>
  /// Raises the <see cref="Removed"/> event.
  /// </summary>
  /// <param name="args">A <see cref="DomainObjectCollectionChangedEventArgs"/> object that contains the event data.</param>
  protected virtual void OnRemoved (DomainObjectCollectionChangedEventArgs args)
  {
    if (Removed != null)
      Removed (this, args);
  }

}
}
