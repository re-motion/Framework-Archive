using System;
using System.Collections;

using Rubicon.Data.DomainObjects.DataManagement;
using Rubicon.Utilities;

namespace Rubicon.Data.DomainObjects
{
/// <summary>
/// Represents a collection of <see cref="DomainObject"/>s.
/// </summary>
/// <remarks>
/// A derived collection with additional state should override at least the following methods:
/// <list type="table">
///   <listheader>
///     <term>Method</term>
///     <description>Description</description>
///   </listheader>
///   <item>
///     <term><see cref="OnAdding"/>, <see cref="OnAdded"/></term>
///     <description>
///       These methods can be used to adjust internal state whenever a new item is added to the collection. 
///       The actual adjustment should be performed in the <see cref="OnAdded"/> method, 
///       because the operation could be cancelled after the <see cref="OnAdding"/> method has been called.
///     </description>
///   </item>
///   <item>
///     <term><see cref="OnRemoving"/>, <see cref="OnRemoved"/></term>
///     <description>
///       These methods can be used to adjust internal state whenever an item is removed from the collection. 
///       The actual adjustment should be performed in the <see cref="OnRemoved"/> method, 
///       because the operation could be cancelled after the <see cref="OnRemoving"/> method has been called. 
///       Note: If the collection is cleared through the <see cref="Clear"/> method <see cref="OnRemoving"/> 
///       and <see cref="OnRemoved"/> are called for every item.
///     </description>
///   </item>
///   <item>
///     <term><see cref="OnDeleting"/>, <see cref="OnDeleted"/></term>
///     <description>
///       These methods can be used to clear all internal state or to unsubscribe from events whenever the <see cref="DomainObject"/> 
///       holding this collection is deleted. The actual adjustment can be performed either in the 
///       <see cref="OnDeleting"/> or in the <see cref="OnDeleted"/> method, 
///       because the operation cannot be cancelled after the <see cref="OnDeleting"/> method has been called.
///     </description>
///   </item>
///   <item>
///     <term><see cref="Commit"/></term>
///     <description>
///       This method is only called on <see cref="DomainObjectCollection"/>s representing the original values 
///       of a one-to-many relation during the commit operation of the associated <see cref="ClientTransaction"/>. 
///       A derived collection should replace its internal state with the state of the provided collection passed 
///       as an argument to this method.
///     </description>
///   </item>
///   <item>
///     <term><see cref="Rollback"/></term>
///     <description>
///       This method is only called on <see cref="DomainObjectCollection"/>s representing the current values 
///       of a one-to-many relation during the rollback operation of the associated <see cref="ClientTransaction"/>. 
///       A derived collection should replace its internal state with the state of the provided collection passed 
///       as an argument to this method.
///     </description>
///   </item>
/// </list>
/// </remarks>
public class DomainObjectCollection : CommonCollection, ICloneable, IList
{
  // types

  // static members and constants

  /// <summary>
  /// Creates an empty <see cref="DomainObjectCollection"/> of a given <see cref="Type"/>.
  /// </summary>
  /// <param name="collectionType">The <see cref="Type"/> of the new collection that should be instantiated.</param>
  /// <returns>The new <see cref="DomainObjectCollection"/>.</returns>
  /// <exception cref="System.ArgumentNullException"><i>collectionType</i> is a null reference.</exception>
  /// <exception cref="System.InvalidCastException"><i>collectionType</i> cannot be casted to <see cref="DomainObjectCollection"/>.</exception>
  public static DomainObjectCollection Create (Type collectionType)
  {
    return Create (collectionType, new DataContainerCollection (), null);
  }

  /// <summary>
  /// Creates a <see cref="DomainObjectCollection"/> of a given <see cref="System.Type"/> and sets the <see cref="RequiredItemType"/>.
  /// </summary>
  /// <param name="collectionType">The <see cref="Type"/> of the new collection that should be instantiated.</param>
  /// <param name="requiredItemType">The permitted <see cref="Type"/> of an item in the <see cref="DomainObjectCollection"/>. If specified only this type or derived types can be added to the <b>DomainObjectCollection</b>.</param>
  /// <returns>The new <see cref="DomainObjectCollection"/>.</returns>
  /// <exception cref="System.ArgumentNullException"><i>collectionType</i> is a null reference.</exception>
  /// <exception cref="System.InvalidCastException"><i>collectionType</i> cannot be casted to <see cref="DomainObjectCollection"/>.</exception>
  public static DomainObjectCollection Create (Type collectionType, Type requiredItemType)
  {
    return Create (collectionType, new DataContainerCollection (), requiredItemType);
  }

  /// <summary>
  /// Creates a <see cref="DomainObjectCollection"/> of a given <see cref="System.Type"/> and adds the <see cref="DomainObject"/>s of the given <see cref="DataContainerCollection"/>.
  /// </summary>
  /// <param name="collectionType">The <see cref="Type"/> of the new collection that should be instantiated.</param>
  /// <param name="dataContainers">The <see cref="DataContainer"/>s of the <see cref="DomainObject"/>s that are added to the collection.</param>
  /// <returns>The new <see cref="DomainObjectCollection"/>.</returns>
  /// <exception cref="System.ArgumentNullException">
  ///   <i>collectionType</i> is a null reference.<br /> -or- <br />
  ///   <i>dataContainers</i> is a null reference.
  /// </exception>
  /// <exception cref="System.InvalidCastException"><i>collectionType</i> cannot be casted to <see cref="DomainObjectCollection"/>.</exception>
  public static DomainObjectCollection Create (Type collectionType, DataContainerCollection dataContainers)
  {
    return Create (collectionType, dataContainers, null);
  }

  /// <summary>
  /// Creates a <see cref="DomainObjectCollection"/> of a given <see cref="System.Type"/> and adds the <see cref="DomainObject"/>s of the given <see cref="DataContainerCollection"/>.
  /// </summary>
  /// <param name="collectionType">The <see cref="Type"/> of the new collection that should be instantiated.</param>
  /// <param name="dataContainers">The <see cref="DataContainer"/>s of the <see cref="DomainObject"/>s that are added to the collection.</param>
  /// <param name="requiredItemType">The permitted <see cref="Type"/> of an item in the <see cref="DomainObjectCollection"/>. If specified only this type or derived types can be added to the <b>DomainObjectCollection</b>.</param>
  /// <returns>The new <see cref="DomainObjectCollection"/>.</returns>
  /// <exception cref="System.ArgumentNullException">
  ///   <i>collectionType</i> is a null reference.<br /> -or- <br />
  ///   <i>dataContainers</i> is a null reference.
  /// </exception>
  /// <exception cref="System.InvalidCastException"><i>collectionType</i> cannot be casted to <see cref="DomainObjectCollection"/>.</exception>
  public static DomainObjectCollection Create (
      Type collectionType, 
      DataContainerCollection dataContainers, 
      Type requiredItemType)
  {
    ArgumentUtility.CheckNotNull ("collectionType", collectionType);
    ArgumentUtility.CheckNotNull ("dataContainers", dataContainers);

    DomainObjectCollection domainObjects = (DomainObjectCollection) ReflectionUtility.CreateObject (collectionType);
    domainObjects._requiredItemType = requiredItemType;

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
      if (collection1[i] != (collection2[i]))
        return false;
    }

    return true;
  }

  /// <summary>
  /// Compares two instances of <see cref="DomainObjectCollection"/> for equality.
  /// </summary>
  /// <param name="collection1">The first <see cref="DomainObjectCollection"/>.</param>
  /// <param name="collection2">The second <see cref="DomainObjectCollection"/>.</param>
  /// <param name="ignoreItemOrder">Indicates whether the compare should ignore the order of the items in the collections for the compare operation.</param>
  /// <returns><b>true</b> if the collections are equal; otherwise, <b>false</b>.</returns>
  public static bool Compare (DomainObjectCollection collection1, DomainObjectCollection collection2, bool ignoreItemOrder)
  {
    if (!ignoreItemOrder)
      return (Compare (collection1, collection2));

    if (collection1 == null && collection2 == null) return true;
    if (collection1 == null) return false;
    if (collection2 == null) return false;
    if (collection1.Count != collection2.Count) return false;

    foreach (DomainObject domainObject in collection1)
    {
      if (!collection2.Contains (domainObject))
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
  /// <remarks>
  /// This event is not raised if the <see cref="DomainObject"/> holding the <see cref="DomainObjectCollection"/> is being deleted. 
  /// Either subscribe to the <see cref="DomainObject.Deleting"/> event or override the <see cref="OnDeleting"/> method to implement 
  /// business logic handling this situation.
  /// </remarks>
  public event DomainObjectCollectionChangingEventHandler Removing;
  /// <summary>
  /// Occurs after an object is removed to the collection.
  /// </summary>
  /// <remarks>
  /// This event is not raised if the <see cref="DomainObject"/> holding the <see cref="DomainObjectCollection"/> has been deleted. 
  /// Either subscribe to the <see cref="DomainObject.Deleted"/> event or override the <see cref="OnDeleted"/> method to implement 
  /// business logic handling this situation.
  /// </remarks>
  public event DomainObjectCollectionChangedEventHandler Removed;

  private Type _requiredItemType;
  private ICollectionChangeDelegate _changeDelegate = null;

  // construction and disposing

  /// <summary>
  /// Initializes a new <b>DomainObjectCollection</b>.
  /// </summary>
  /// <remarks>A derived class must support this constructor.</remarks>
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
  /// The new <b>DomainObjectCollection</b> has the same <see cref="RequiredItemType"/> and the same items as the 
  /// given <i>collection</i>.
  /// </remarks>
  /// <param name="collection">The <see cref="DomainObjectCollection"/> to copy.</param>
  /// <param name="makeCollectionReadOnly">Indicates whether the new collection should be read-only.</param>
  /// <exception cref="System.ArgumentNullException"><i>collection</i> is a null reference.</exception>
  public DomainObjectCollection (DomainObjectCollection collection, bool makeCollectionReadOnly)
  {
    ArgumentUtility.CheckNotNull ("collection", collection);

    foreach (DomainObject domainObject in collection)
      Add (domainObject);

    this.SetIsReadOnly (makeCollectionReadOnly);
    _requiredItemType = collection.RequiredItemType;    
  }

  /// <summary>
  /// Initializes a new <b>DomainObjectCollection</b> as a shallow copy of a given array of <see cref="DomainObject"/>s.
  /// </summary>
  /// <param name="domainObjects">The array of <see cref="DomainObject"/>s to copy.</param>
  /// <param name="makeCollectionReadOnly">Indicates whether the new collection should be read-only.</param>
  /// <exception cref="System.ArgumentNullException"><i>domainObjects</i> is a null reference.</exception>
  public DomainObjectCollection (DomainObject[] domainObjects, bool makeCollectionReadOnly)
  {
    ArgumentUtility.CheckNotNull ("domainObjects", domainObjects);

    foreach (DomainObject domainObject in domainObjects)
      Add (domainObject);

    this.SetIsReadOnly (makeCollectionReadOnly);
  }

  /// <summary>
  /// Initializes a new <b>DomainObjectCollection</b> as a shallow copy of a <see cref="DataManagement.DataContainerCollection"/>s.
  /// </summary>
  /// <param name="dataContainers">The <see cref="DataManagement.DataContainerCollection"/> to copy.</param>
  /// <param name="makeCollectionReadOnly">Indicates whether the new collection should be read-only.</param>
  /// <exception cref="System.ArgumentNullException"><i>dataContainers</i> is a null reference.</exception>
  public DomainObjectCollection (DataContainerCollection dataContainers, bool makeCollectionReadOnly)
  {
    ArgumentUtility.CheckNotNull ("dataContainers", dataContainers);

    foreach (DataContainer dataContainer in dataContainers)
      Add (dataContainer.DomainObject);

    this.SetIsReadOnly (makeCollectionReadOnly);
  }

  // methods and properties

  /// <summary>
  /// Adds all items of the given <see cref="DomainObjectCollection"/> to the <b>DomainObjectCollection</b>, that are not already part of it.
  /// </summary>
  /// <remarks>The method does not modify the given <see cref="DomainObjectCollection"/>.</remarks>
  /// <param name="domainObjects">The <see cref="DomainObjectCollection"/> to add items from.</param>
  /// <exception cref="System.ArgumentNullException"><i>domainObjects</i> is a null reference.</exception>
  public void Combine (DomainObjectCollection domainObjects)
  {
    ArgumentUtility.CheckNotNull ("domainObjects", domainObjects);
    if (IsReadOnly) throw new NotSupportedException ("A read-only collection cannot be combined with another collection.");

    foreach (DomainObject domainObject in domainObjects)
    {
      if (!Contains (domainObject))
        Add (domainObject);
    }
  }

  /// <summary>
  /// Returns all items of a given <see cref="DomainObjectCollection"/> that are not part of the <b>DomainObjectCollection</b>.
  /// </summary>
  /// <remarks>The method does not return any items that are in the collection, but not in <i>domainObjects</i>.</remarks>
  /// <param name="domainObjects">The collection to evaluate.</param>
  /// <returns>A <see cref="DomainObjectCollection"/> with all items of <i>domainObjects</i> that are not part of the collection.</returns>
  /// <exception cref="System.ArgumentNullException"><i>domainObjects</i> is a null reference.</exception>
  public DomainObjectCollection GetItemsNotInCollection (DomainObjectCollection domainObjects)
  {
    ArgumentUtility.CheckNotNull ("domainObjects", domainObjects);

    DomainObjectCollection itemsNotInCollection = new DomainObjectCollection ();

    foreach (DomainObject domainObject in domainObjects)
    {
      if (!Contains (domainObject))
        itemsNotInCollection.Add (domainObject);
    }

    return itemsNotInCollection;
  }

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
  /// Determines whether an item is in the <see cref="DomainObjectCollection"/>.
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
  /// Determines whether an item is in the <see cref="DomainObjectCollection"/>.
  /// </summary>
  /// <param name="id">The <see cref="ObjectID"/> of the <see cref="DomainObject"/> to locate in the <see cref="DomainObjectCollection"/>.</param>
  /// <returns><b>true</b> if the <see cref="DomainObject"/> with the <i>id</i> is found in the <see cref="DomainObjectCollection"/>; otherwise, false;</returns>
  /// <exception cref="System.ArgumentNullException"><i>id</i> is a null reference</exception>
  public bool Contains (ObjectID id)
  {
    ArgumentUtility.CheckNotNull ("id", id);

    return BaseContainsKey (id);
  }

  /// <summary>
  /// Gets or sets the <see cref="DomainObject"/> with a given <i>index</i> in the <see cref="DomainObjectCollection"/>.
  /// </summary>
  /// <exception cref="System.ArgumentOutOfRangeException">
  ///   <i>index</i> is less than zero.<br /> -or- <br />
  ///   <i>index</i> is equal to or greater than the number of items in the collection.
  /// </exception>
  /// <exception cref="System.NotSupportedException">The collection is read-only.</exception>
  /// <exception cref="System.ArgumentException">
  ///   <i>value</i> is not a derived type of <see cref="DomainObject"/> and of type <see cref="RequiredItemType"/> or a derived type.
  /// </exception>
  /// <exception cref="System.InvalidOperationException"><i>value</i> is already part of the collection.</exception>
  public DomainObject this[int index]  
  {
    get 
    { 
      return (DomainObject) BaseGetObject (index); 
    }
    set 
    {
      CheckIndexForIndexer ("index", index);
      if (IsReadOnly) throw new NotSupportedException ("Cannot modify a read-only collection.");

      // If new value is null: This is actually a remove operation
      if (value == null)
      {
        RemoveAt (index);
        return;
      }
      
      CheckItemType (value, "value");
      
      // If old and new objects are the same: Perform no operation
      if (object.ReferenceEquals (this[index], value))
        return;

      if (Contains (value))
      {
        throw CreateInvalidOperationException (
            "Cannot replace an object '{0}' with another object '{1}' already part of this collection.", 
            this[index].ID, value.ID);
      }

      if (_changeDelegate != null)
      {
        _changeDelegate.PerformReplace (this, value, index);
      }
      else
      {
        DomainObject oldObject = this[index];

        BeginRemove (oldObject);
        BeginAdd (value);

        PerformRemove (oldObject);
        PerformInsert (index, value);

        EndRemove (oldObject);
        EndAdd (value);
      }
    }
  }

  /// <summary>
  /// Gets the <see cref="DomainObject"/> with a given <see cref="ObjectID"/> from the <see cref="DomainObjectCollection"/>.
  /// </summary>
  /// <remarks>The indexer returns null if the given <i>id</i> was not found.</remarks>
  public DomainObject this[ObjectID id]  
  {
    get { return (DomainObject) BaseGetObject (id); }
  }

  /// <summary>
  /// Adds a <see cref="DomainObject"/> to the collection.
  /// </summary>
  /// <param name="domainObject">The <see cref="DomainObject"/> to add.</param>
  /// <returns>The zero-based index where the <i>domainObject</i> has been added.</returns>
  /// <exception cref="System.NotSupportedException">The collection is read-only.</exception>
  /// <exception cref="System.ArgumentNullException"><i>domainObject</i> is a null reference.</exception>
  /// <exception cref="System.ArgumentException"><i>domainObject</i> is not of type <see cref="RequiredItemType"/> or one of its derived types.</exception>
  public int Add (DomainObject domainObject)
  {
    ArgumentUtility.CheckNotNull ("domainObject", domainObject);
    CheckItemType (domainObject, "domainObject");
    if (IsReadOnly) throw new NotSupportedException ("Cannot add an item to a read-only collection.");

    if (Contains (domainObject))
    {
      throw CreateArgumentException (
          "domainObject", "Cannot add object '{0}' already part of this collection.", domainObject.ID);
    }

    if (_changeDelegate != null)
    {
      _changeDelegate.PerformInsert (this, domainObject, Count);
    }
    else
    {
      BeginAdd (domainObject);
      PerformAdd (domainObject);
      EndAdd (domainObject);
    }

    return Count - 1;
  }

  /// <summary>
  /// Removes a <see cref="DomainObject"/> from the collection.
  /// </summary>
  /// <param name="index">The index of the <see cref="DomainObject"/> to remove.</param>
  /// <exception cref="System.ArgumentOutOfRangeException">
  ///   <i>index</i> is less than zero.<br /> -or- <br />
  ///   <i>index</i> is equal to or greater than the number of items in the collection.
  /// </exception>
  /// <exception cref="System.NotSupportedException">The collection is read-only.</exception>
  public void RemoveAt (int index)
  {
    Remove (this[index]);
  }

  /// <summary>
  /// Removes a <see cref="DomainObject"/> from the collection.
  /// </summary>
  /// <param name="id">The <see cref="ObjectID"/> of the <see cref="DomainObject"/> to remove.</param>
  /// <exception cref="System.ArgumentNullException"><i>id</i> is a null reference.</exception>
  /// <exception cref="System.NotSupportedException">The collection is read-only.</exception>
  public void Remove (ObjectID id)
  {
    ArgumentUtility.CheckNotNull ("id", id);
    if (IsReadOnly) throw new NotSupportedException ("Cannot remove an item from a read-only collection.");

    DomainObject domainObject = this[id];
    if (domainObject != null)
      Remove (domainObject);
  }

  /// <summary>
  /// Removes a <see cref="DomainObject"/> from the collection.
  /// </summary>
  /// <remarks>
  ///   If <b>Remove</b> is called with an object that is not in the collection, no exception is thrown, and no events are raised. 
  /// </remarks>
  /// <param name="domainObject">The <see cref="DomainObject"/> to remove.</param>
  /// <exception cref="System.ArgumentNullException"><i>domainObject</i> is a null reference.</exception>
  /// <exception cref="System.NotSupportedException">The collection is read-only.</exception>
  public void Remove (DomainObject domainObject)
  {
    ArgumentUtility.CheckNotNull ("domainObject", domainObject);
    if (IsReadOnly) throw new NotSupportedException ("Cannot remove an item from a read-only collection.");

    // Do not perform remove, if domain object is not part of this collection     
    if (this[domainObject.ID] == null)
      return;

    if (_changeDelegate != null)
    {
      _changeDelegate.PerformRemove (this, domainObject);
    }
    else
    {
      BeginRemove (domainObject);
      PerformRemove (domainObject);
      EndRemove (domainObject);
    }
  }

  /// <summary>
  /// Removes all items from the <see cref="DomainObjectCollection"/>.
  /// </summary>
  /// <exception cref="System.NotSupportedException">The collection is read-only.</exception>
  public void Clear ()
  {
    if (IsReadOnly) throw new NotSupportedException ("Cannot clear a read-only collection.");

    for (int i = Count - 1; i >= 0; i--)
      Remove (this[i].ID);
  }

  /// <summary>
  /// Returns the zero-based index of a given <see cref="DomainObject"/> in the collection.
  /// </summary>
  /// <param name="domainObject">The <i>domainObject</i> to locate in the collection.</param>
  /// <returns>The zero-based index of the <i>domainObject</i>, if found; otherwise, -1.</returns>
  public int IndexOf (DomainObject domainObject)
  {
    ObjectID id = null;

    if (domainObject != null)
      id = domainObject.ID;

    return IndexOf (id);
  }

  /// <summary>
  /// Returns the zero-based index of a given <see cref="ObjectID"/> in the collection.
  /// </summary>
  /// <param name="id">The <i>id</i> to locate in the collection.</param>
  /// <returns>The zero-based index of the <i>id</i>, if found; otherwise, -1.</returns>
  public int IndexOf (ObjectID id)
  {
    return BaseIndexOfKey (id);
  }

  /// <summary>
  /// Inserts a <see cref="DomainObject"/> into the collection at the specified index.
  /// </summary>
  /// <param name="index">The zero-based <i>index</i> at which the item should be inserted.</param>
  /// <param name="domainObject">The <i>domainObject</i> to add.</param>
  /// <exception cref="System.NotSupportedException">The collection is read-only.</exception>
  /// <exception cref="System.ArgumentOutOfRangeException">
  ///   <i>index</i> is less than zero.<br /> -or- <br />
  ///   <i>index</i> is greater than the number of items in the collection.
  /// </exception>
  /// <exception cref="System.ArgumentNullException"><i>domainObject</i> is a null reference.</exception>
  /// <exception cref="System.ArgumentException">
  ///   The <i>domainObject</i> already exists in the collection.<br /> -or- <br />
  ///   <i>domainObject</i> is not of type <see cref="RequiredItemType"/> or one of its derived types.
  /// </exception>
  public void Insert (int index, DomainObject domainObject)
  {
    ArgumentUtility.CheckNotNull ("domainObject", domainObject);
    CheckIndexForInsert ("index", index);
    if (IsReadOnly) throw new NotSupportedException ("Cannot insert an item into a read-only collection.");
    CheckItemType (domainObject, "domainObject");
    
    if (Contains (domainObject))
    {
      throw CreateArgumentException (
          "domainObject", "Cannot insert object '{0}' already part of this collection.", domainObject.ID);
    }

    if (_changeDelegate != null)
    {
      _changeDelegate.PerformInsert (this, domainObject, index);
    }
    else
    {
      BeginAdd (domainObject);
      PerformInsert (index, domainObject);
      EndAdd (domainObject);
    }
  }

  /// <summary>
  /// Gets a value indicating if the collection has a fixed size. Always returns false.
  /// </summary>
  public bool IsFixedSize
  {
    get { return false; }
  }


  #region Explicitly implemeted IList Members

  /// <summary>
  /// Gets or sets the element at the specified index. 
  /// </summary>
  object IList.this[int index]
  {
    get 
    { 
      return this[index]; 
    }
    set 
    {
      ArgumentUtility.CheckType ("value", value, typeof (DomainObject));

      this[index] = (DomainObject) value; 
    } 
  }

  /// <summary>
  /// Inserts an item to the IList at the specified position
  /// </summary>
  /// <param name="index">The zero-based index at which <i>value</i> should be inserted.</param>
  /// <param name="value">The <see cref="Object"/> to insert into the <see cref="IList"/>.</param>
  void IList.Insert (int index, object value)
  {
    ArgumentUtility.CheckNotNullAndType ("value", value, typeof (DomainObject));

    Insert (index, (DomainObject) value);
  }

  /// <summary>
  /// Removes a specific object from the <see cref="IList"/>.
  /// </summary>
  /// <param name="value">The <see cref="Object"/> to remove from the <see cref="IList"/>.</param>
  void IList.Remove (object value)
  {
    if (value is DomainObject)
      Remove ((DomainObject) value);

    if (value is ObjectID)
      Remove ((ObjectID) value);
  }

  /// <summary>
  /// Determines whether the <see cref="IList"/> contains a specific <i>>value</i>.
  /// </summary>
  /// <param name="value">The <see cref="Object"/> to locate in the <see cref="IList"/>.</param>
  /// <returns><b>true</b> if the <see cref="Object"/> is found in the <see cref="IList"/>; otherwise, <b>false</b></returns>
  bool IList.Contains (object value)
  {
    if (value is DomainObject)
      return Contains ((DomainObject) value);

    if (value is ObjectID)
      return Contains ((ObjectID) value);

    return false;
  }

  /// <summary>
  /// Determines the index of a specific item in the <see cref="IList"/>.
  /// </summary>
  /// <param name="value">The <see cref="Object"/> to locate in the <see cref="IList"/>.</param>
  /// <returns>The index of <i>value</i> if found in the list; otherwise, -1.</returns>
  int IList.IndexOf (object value)
  {
    if (value is DomainObject)
      return IndexOf ((DomainObject) value);

    if (value is ObjectID)
      return IndexOf ((ObjectID) value);

    return -1;
  }

  /// <summary>
  /// Adds an item to the <see cref="IList"/>.
  /// </summary>
  /// <param name="value">The <see cref="Object"/> to add to the <see cref="IList"/>.</param>
  /// <returns>The position into which the new element was inserted.</returns>
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
  /// Creates a shallow copy of this collection. Can be overridden in derived classes.
  /// </summary>
  /// <param name="makeCloneReadOnly">Specifies whether the cloned collection should be read-only.</param>
  /// <returns>The cloned collection.</returns>
  /// <remarks>
  /// A shallow copy creates a new <see cref="DomainObjectCollection"/> instance
  /// which can be independently modified without affecting the original collection.
  /// Thus meaning the references to the domain objects are copied, not the domain objects themselves.<br/><br/>
  /// The <see cref="System.Type"/> of the cloned collection is equal to the <see cref="System.Type"/> of the <b>DomainObjectCollection</b>.
  /// </remarks>
  public virtual DomainObjectCollection Clone (bool makeCloneReadOnly)
  {
    DomainObjectCollection clone = Create (this.GetType ());

    clone._requiredItemType = this.RequiredItemType;
    clone.Combine (this);
    clone.SetIsReadOnly (makeCloneReadOnly);
    
    return clone;
  }

  #endregion

  internal void BeginAdd (DomainObject domainObject)
  {
    OnAdding (new DomainObjectCollectionChangingEventArgs (domainObject));
  }

  /// <summary>
  /// Performs a rollback of the collection by replacing the items in the collection with the items of a given <see cref="DomainObjectCollection"/>.
  /// </summary>
  /// <param name="originalDomainObjects">A <see cref="DomainObjectCollection"/> containing the original items of the collection.</param>
  /// <remarks>
  ///   This method is only called on <see cref="DomainObjectCollection"/>s representing the current values 
  ///   of a one-to-many relation during the rollback operation of the associated <see cref="ClientTransaction"/>. 
  ///   A derived collection should replace its internal state with the state of <i>originalDomainObjects</i>.
  /// </remarks>
  internal protected virtual void Rollback (DomainObjectCollection originalDomainObjects)
  {
    ArgumentUtility.CheckNotNull ("originalDomainObjects", originalDomainObjects);
    
    ReplaceItems (originalDomainObjects);
  }

  /// <summary>
  /// Performs a commit of the collection by replacing the items in the collection with the items of a given <see cref="DomainObjectCollection"/>.
  /// </summary>
  /// <param name="domainObjects">A <see cref="DomainObjectCollection"/> containing the new items for the collection.</param>
  /// <remarks>
  ///   This method is only called on <see cref="DomainObjectCollection"/>s representing the original values 
  ///   of a one-to-many relation during the commit operation of the associated <see cref="ClientTransaction"/>. 
  ///   A derived collection should replace its internal state with the state of <i>domainObjects</i>.
  /// </remarks>
  internal protected virtual void Commit (DomainObjectCollection domainObjects)
  {
    ArgumentUtility.CheckNotNull ("domainObjects", domainObjects);
    
    ReplaceItems (domainObjects);
  }

  /// <summary>
  /// Replaces the items in the collection with the items of a given <see cref="DomainObjectCollection"/>.
  /// </summary>
  /// <param name="domainObjects">A <see cref="DomainObjectCollection"/> containing the new items for the collection.</param>
  /// <remarks>
  ///   This method actually performs the replace operation for <see cref="Commit"/> and <see cref="Rollback"/>.
  ///   Note: The replacement raises no events.
  /// </remarks>
  protected virtual void ReplaceItems (DomainObjectCollection domainObjects)
  {
    bool isReadOnly = IsReadOnly;
    
    SetIsReadOnly (false);

    BaseClear ();
    foreach (DomainObject domainObject in domainObjects)
      PerformAdd (domainObject);

    SetIsReadOnly (isReadOnly);
  }

  /// <summary>
  /// Adds a <see cref="DomainObject"/> to the collection without raising the <see cref="Adding"/> and <see cref="Added"/> events.
  /// </summary>
  /// <param name="domainObject">The <see cref="DomainObject"/> to add to the collection.</param>
  /// <returns>The position into which the new <see cref="DomainObject"/> was inserted.</returns>
  /// <exception cref="System.ArgumentNullException"><i>domainObject</i> is a null reference.</exception>
  /// <exception cref="System.NotSupportedException">The collection is read-only.</exception>
  /// <exception cref="System.ArgumentException"><i>domainObject</i> is not of type <see cref="RequiredItemType"/> or one of its derived types.</exception>
  internal protected int PerformAdd (DomainObject domainObject)
  {
    ArgumentUtility.CheckNotNull ("domainObject", domainObject);
    if (IsReadOnly) throw new NotSupportedException ("Cannot add an item to a read-only collection.");
    CheckItemType (domainObject, "domainObject");

    return BaseAdd (domainObject.ID, domainObject);
  }

  /// <summary>
  /// Inserts a <see cref="DomainObject"/> at a given index to the collection without raising the <see cref="Adding"/> and <see cref="Added"/> events.
  /// </summary>
  /// <param name="index">The zero-based <i>index</i> at which the item should be inserted.</param>
  /// <param name="domainObject">The <i>domainObject</i> to add.</param>
  /// <exception cref="System.NotSupportedException">The collection is read-only.</exception>
  /// <exception cref="System.ArgumentOutOfRangeException">
  ///   <i>index</i> is less than zero.<br /> -or- <br />
  ///   <i>index</i> is greater than the number of items in the collection.
  /// </exception>
  /// <exception cref="System.ArgumentNullException"><i>domainObject</i> is a null reference.</exception>
  /// <exception cref="System.ArgumentException">
  ///   The <i>domainObject</i> already exists in the collection.<br /> -or- <br />
  ///   <i>domainObject</i> is not of type <see cref="RequiredItemType"/> or one of its derived types.
  /// </exception>
  internal protected void PerformInsert (int index, DomainObject domainObject)
  {
    ArgumentUtility.CheckNotNull ("domainObject", domainObject);
    if (IsReadOnly) throw new NotSupportedException ("Cannot insert an item into a read-only collection.");
    CheckItemType (domainObject, "domainObject");

    BaseInsert (index, domainObject.ID, domainObject);
  }

  internal void EndAdd (DomainObject domainObject)
  {
    OnAdded (new DomainObjectCollectionChangedEventArgs (domainObject));
  }

  internal void BeginRemove (DomainObject domainObject)
  {
    OnRemoving (new DomainObjectCollectionChangingEventArgs (domainObject));
  }

  /// <summary>
  /// Removes a <see cref="DomainObject"/> from the collection without raising the <see cref="Removing"/> and <see cref="Removed"/> events.
  /// </summary>
  /// <param name="domainObject">The <see cref="DomainObject"/> to remove from the collection.</param>
  /// <exception cref="System.ArgumentNullException"><i>domainObject</i> is a null reference.</exception>
  /// <exception cref="System.NotSupportedException">The collection is read-only.</exception>
  internal protected void PerformRemove (DomainObject domainObject)
  {
    ArgumentUtility.CheckNotNull ("domainObject", domainObject);
    if (IsReadOnly) throw new NotSupportedException ("Cannot remove an item from a read-only collection.");

    BaseRemove (domainObject.ID);
  }

  internal void EndRemove (DomainObject domainObject)
  {
    OnRemoved (new DomainObjectCollectionChangedEventArgs (domainObject));
  }

  /// <summary>
  /// Clears the <see cref="DomainObjectCollection"/> without raising the <see cref="Removing"/> and <see cref="Removed"/> events.
  /// </summary>
  /// <exception cref="System.NotSupportedException">The collection is read-only.</exception>
  internal void PerformDelete ()
  {
    if (IsReadOnly) throw new NotSupportedException ("Cannot clear a read-only collection.");

    OnDeleting ();
    BaseClear ();
    OnDeleted ();
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
  /// <remarks>This method can be used to adjust internal state whenever a new item is added to the collection.</remarks>
  protected virtual void OnAdded (DomainObjectCollectionChangedEventArgs args)
  {
    if (Added != null)
      Added (this, args);
  }

  /// <summary>
  /// Raises the <see cref="Removing"/> event.
  /// </summary>
  /// <param name="args">A <see cref="DomainObjectCollectionChangingEventArgs"/> object that contains the event data.</param>
  ///   If the collection is cleared through the <see cref="Clear"/> method <see cref="OnRemoving"/> 
  ///   is called for every item.
  protected virtual void OnRemoving (DomainObjectCollectionChangingEventArgs args)
  {
    if (Removing != null)
      Removing (this, args);
  }

  /// <summary>
  /// Raises the <see cref="Removed"/> event.
  /// </summary>
  /// <param name="args">A <see cref="DomainObjectCollectionChangedEventArgs"/> object that contains the event data.</param>
  /// <remarks>
  ///   This method can be used to adjust internal state whenever an item is removed from the collection.
  ///   If the collection is cleared through the <see cref="Clear"/> method <see cref="OnRemoved"/> 
  ///   is called for every item.
  /// </remarks>
  protected virtual void OnRemoved (DomainObjectCollectionChangedEventArgs args)
  {
    if (Removed != null)
      Removed (this, args);
  }

  /// <summary>
  /// The method is invoked immediately before the <see cref="DomainObject"/> holding this collection is deleted if the <b>DomainObjectCollection</b> represents a one-to-many relation.
  /// </summary>
  /// <remarks>
  /// During the delete process of a <see cref="DomainObject"/> all <see cref="DomainObject"/>s are removed from the <b>DomainObjectCollection</b> without notifying other objects.
  /// Immediately before all <see cref="DomainObject"/>s will be removed the <b>OnDeleting</b> method is invoked 
  /// to allow derived collections to adjust their internal state or to unsubscribe from events raised by <see cref="DomainObject"/>s that 
  /// are part of the <b>DomainObjectCollection</b>.<br/><br/>
  /// <b>Note:</b> A derived collection overriding this method must not raise an exception.
  /// </remarks>
  protected virtual void OnDeleting ()
  {
  }

  /// <summary>
  /// The method is invoked after the <see cref="DomainObject"/> holding this collection is deleted if the <b>DomainObjectCollection</b> represents a one-to-many relation.
  /// </summary>
  /// <remarks>
  /// During the delete process of a <see cref="DomainObject"/> all <see cref="DomainObject"/>s are removed from the <b>DomainObjectCollection</b> without notifying other objects.
  /// After all <see cref="DomainObject"/>s have been removed the <b>OnDeleted</b> method is invoked 
  /// to allow derived collections to adjust their internal state.<br/><br/>
  /// <b>Note:</b> A derived collection overriding this method must not raise an exception.
  /// </remarks>
  protected virtual void OnDeleted ()
  {
  }

  private void CheckItemType (DomainObject domainObject, string argumentName)
  {
    CheckItemType (_requiredItemType, domainObject.GetType (), argumentName);
  }

  private void CheckItemType (Type requiredType, Type itemType, string argumentName)
  {
    if (requiredType != null && !requiredType.Equals (itemType) && !itemType.IsSubclassOf (requiredType))
    {
      throw CreateArgumentException (
        argumentName,
        "Values of type '{0}' cannot be added to this collection. Values must be of type '{1}' or derived from '{1}'.", 
        itemType, 
        requiredType);
    }
  }

  private InvalidOperationException CreateInvalidOperationException (string message, params object[] args)
  {
    return new InvalidOperationException (string.Format (message, args));
  }

  private ArgumentException CreateArgumentException (string argumentName, string message, params object[] args)
  {
    return new ArgumentException (string.Format (message, args), argumentName);
  }
}
}
