// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
// 
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// as published by the Free Software Foundation; either version 2.1 of the 
// License, or (at your option) any later version.
// 
// re-motion is distributed in the hope that it will be useful, 
// but WITHOUT ANY WARRANTY; without even the implied warranty of 
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the 
// GNU Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public License
// along with re-motion; if not, see http://www.gnu.org/licenses.
// 
using System;
using System.Collections;
using System.Collections.Generic;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.DataManagement.CollectionDataManagement;
using Remotion.Data.DomainObjects.DataManagement.EndPointModifications;
using Remotion.Utilities;
using System.Linq;

namespace Remotion.Data.DomainObjects
{
  /// <summary>
  /// Represents a collection of <see cref="DomainObject"/>s.
  /// </summary>
  /// <remarks>
  /// <para>
  /// If a <see cref="DomainObjectCollection"/> is used to model a bidirectional 1:n relation, consider the following about ordering:
  /// <list type="bullet">
  ///   <item>
  ///     When loading the collection from the database (via loading an object in a root transaction), the order of items is defined
  ///     by the sort order of the relation (see <see cref="DBBidirectionalRelationAttribute.SortExpression"/>). If there is no sort
  ///     order, the order of items is undefined.
  ///   </item>
  ///   <item>
  ///     When committing a root transaction, the order of items in the collection is ignored; the next time the object is loaded
  ///     in another root transaction, the sort order is again defined by the sort order (or undefined). Specifically, if only the
  ///     order of items changed in a 1:n relation, the respective objects will not be written to the database at all, as they will not
  ///     be considered to have changed.
  ///   </item>
  ///   <item>
  ///     When loading the collection from a parent transaction via loading an object in a subtransaction, the order of items is the same
  ///     as in the parent transaction.
  ///   </item>
  ///   <item>
  ///     When committing a subtransaction, the order of items in the collection is propagated to the parent transaction. After the commit,
  ///     the parent transaction's collection will have the items in the same order as the committed subtransaction.
  ///   </item>
  /// </list>
  /// </para>
  /// <para>
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
  ///       holding this collection is deleted. The actual adjustment should be performed in the 
  ///       <see cref="OnDeleted"/> method, because the operation could be cancelled after the <see cref="OnDeleting"/> method has been called. 
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
  /// </para>
  /// </remarks>
  [Serializable]
  public class DomainObjectCollection : ICloneable, IList, IDomainObjectCollectionEventRaiser
  {
    // types

    private class Transformer : IDomainObjectCollectionTransformer
    {
      public Transformer (DomainObjectCollection owningCollection)
      {
        Collection = owningCollection;
      }

      public DomainObjectCollection Collection { get; private set; }

      public void TransformToAssociated (ICollectionEndPoint endPoint)
      {
        var endPointDelegatingCollectionData = endPoint.CreateDelegatingCollectionData (Collection._data.GetUndecoratedDataStore ());
        Assertion.IsTrue (endPointDelegatingCollectionData.RequiredItemType == Collection.RequiredItemType);

        Collection._data = endPointDelegatingCollectionData;
      }

      public void TransformToStandAlone ()
      {
        Assertion.IsNotNull (Collection.AssociatedEndPoint);

        Collection._data = CreateDataStrategyForStandAloneCollection (Collection._data.GetUndecoratedDataStore (), Collection.RequiredItemType, Collection);
      }
    }

    // static members and constants

    /// <summary>
    /// Creates an <see cref="IDomainObjectCollectionData"/> object for stand-alone collections. The returned object takes care of argument checks,
    /// required item checks, and event raising.
    /// </summary>
    /// <param name="dataStore">The data store to use for the collection.</param>
    /// <param name="requiredItemType">The required item type to use for the collection.</param>
    /// <param name="eventRaiser">The event raiser to use for raising events.</param>
    /// <returns>An instance of <see cref="IDomainObjectCollectionData"/> that can be used for stand-alone collections.</returns>
    public static IDomainObjectCollectionData CreateDataStrategyForStandAloneCollection (IDomainObjectCollectionData dataStore, Type requiredItemType, IDomainObjectCollectionEventRaiser eventRaiser)
    {
      ArgumentUtility.CheckNotNull ("dataStore", dataStore);
      ArgumentUtility.CheckNotNull ("eventRaiser", eventRaiser);

      return new ArgumentCheckingCollectionDataDecorator (requiredItemType, new EventRaisingCollectionDataDecorator (eventRaiser, dataStore));
    }


    /// <summary>
    /// Compares two instances of <see cref="DomainObjectCollection"/> for equality.
    /// </summary>
    /// <param name="collection1">The first <see cref="DomainObjectCollection"/>.</param>
    /// <param name="collection2">The second <see cref="DomainObjectCollection"/>.</param>
    /// <returns><see langword="true"/> if the collections are equal; otherwise, <see langword="false"/>.</returns>
    public static bool Compare (DomainObjectCollection collection1, DomainObjectCollection collection2)
    {
      if (collection1 == null && collection2 == null)
        return true;
      if (collection1 == null)
        return false;
      if (collection2 == null)
        return false;
      if (collection1.Count != collection2.Count)
        return false;

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
    /// <returns><see langword="true"/> if the collections are equal; otherwise, <see langword="false"/>.</returns>
    public static bool Compare (DomainObjectCollection collection1, DomainObjectCollection collection2, bool ignoreItemOrder)
    {
      if (!ignoreItemOrder)
        return (Compare (collection1, collection2));

      if (collection1 == null && collection2 == null)
        return true;
      if (collection1 == null)
        return false;
      if (collection2 == null)
        return false;
      if (collection1.Count != collection2.Count)
        return false;

      foreach (DomainObject domainObject in collection1)
      {
        if (!collection2.ContainsObject (domainObject))
          return false;
      }

      return true;
    }

    // member fields

    /// <summary>
    /// Occurs before an object is added to the collection.
    /// </summary>
    public event DomainObjectCollectionChangeEventHandler Adding;

    /// <summary>
    /// Occurs after an object is added to the collection.
    /// </summary>
    public event DomainObjectCollectionChangeEventHandler Added;

    /// <summary>
    /// Occurs before an object is removed to the collection.
    /// </summary>
    /// <remarks>
    /// This event is not raised if the <see cref="DomainObject"/> holding the <see cref="DomainObjectCollection"/> is being deleted. 
    /// Either subscribe to the <see cref="DomainObject.Deleting"/> event or override the <see cref="OnDeleting"/> method to implement 
    /// business logic handling this situation.
    /// </remarks>
    public event DomainObjectCollectionChangeEventHandler Removing;

    /// <summary>
    /// Occurs after an object is removed to the collection.
    /// </summary>
    /// <remarks>
    /// This event is not raised if the <see cref="DomainObject"/> holding the <see cref="DomainObjectCollection"/> has been deleted. 
    /// Either subscribe to the <see cref="DomainObject.Deleted"/> event or override the <see cref="OnDeleted"/> method to implement 
    /// business logic handling this situation.
    /// </remarks>
    public event DomainObjectCollectionChangeEventHandler Removed;

    internal IDomainObjectCollectionData _data; // TODO 1033: Make private as soon as CollectionEndPoint manages the real data of managed DOCollections.
    private Type _requiredItemType;
    private bool _isReadOnly;
    
    // construction and disposing

    /// <summary>
    /// Initializes a new <see cref="DomainObjectCollection" />.
    /// </summary>
    /// <remarks>A derived class must support this constructor.</remarks>
    public DomainObjectCollection()
        : this ((Type) null)
    {
    }

    // standard constructor for collections
    /// <summary>
    /// Initializes a new instance of the <see cref="DomainObjectCollection"/> class with a given <see cref="IDomainObjectCollectionData"/>
    /// data storage strategy.
    /// </summary>
    /// <param name="dataStrategy">The <see cref="IDomainObjectCollectionData"/> instance to use as the data storage strategy.</param>
    /// <remarks>
    /// <para>
    /// Derived classes must support this constructor.
    /// </para>
    /// <para>
    /// The given <paramref name="dataStrategy"/> is directly used, so it should perform any argument checks and event raising on its own.
    /// </para>
    /// </remarks>
    public DomainObjectCollection (IDomainObjectCollectionData dataStrategy)
    {
      ArgumentUtility.CheckNotNull ("dataStrategy", dataStrategy);

      _data = dataStrategy;
    }

    /// <summary>
    /// Initializes a new <see cref="DomainObjectCollection" /> that only takes a certain <see cref="Type"/> as members.
    /// </summary>
    /// <param name="requiredItemType">The <see cref="Type"/> that are required for members.</param>
    public DomainObjectCollection (Type requiredItemType)
    {
      _data = CreateDataStrategyForStandAloneCollection (new DomainObjectCollectionData (), requiredItemType, this);
    }

    // standard constructor for collections
    /// <summary>
    /// Initializes a new <see cref="DomainObjectCollection"/> as a shallow copy of a given <see cref="DomainObjectCollection"/>.
    /// </summary>
    /// <remarks>
    /// The new <see cref="DomainObjectCollection" /> has the same <see cref="RequiredItemType"/> and the same items as the 
    /// given <paramref name="collection"/>.
    /// </remarks>
    /// <param name="collection">The <see cref="DomainObjectCollection"/> to copy. Must not be <see langword="null"/>.</param>
    /// <param name="makeCollectionReadOnly">Indicates whether the new collection should be read-only.</param>
    /// <exception cref="System.ArgumentNullException"><paramref name="collection"/> is <see langword="null"/>.</exception>
    public DomainObjectCollection (DomainObjectCollection collection, bool makeCollectionReadOnly)
    {
      _data = CreateDataStrategyForStandAloneCollection (new DomainObjectCollectionData (collection.Cast<DomainObject>()), collection.RequiredItemType, this);
      SetIsReadOnly (makeCollectionReadOnly);
    }

    /// <summary>
    /// Initializes a new <see cref="DomainObjectCollection"/> as a shallow copy of a given enumeration of <see cref="DomainObject"/>s.
    /// </summary>
    /// <param name="domainObjects">The <see cref="DomainObject"/>s to copy. Must not be <see langword="null"/>.</param>
    /// <param name="requiredItemType">The required item type of the new collection.</param>
    /// <param name="makeCollectionReadOnly">Indicates whether the new collection should be read-only.</param>
    /// <exception cref="System.ArgumentNullException"><paramref name="domainObjects"/> is <see langword="null"/>.</exception>
    public DomainObjectCollection (IEnumerable<DomainObject> domainObjects, Type requiredItemType, bool makeCollectionReadOnly)
    {
      var dataStore = new DomainObjectCollectionData ();
      dataStore.AddRangeAndCheckItems (domainObjects, requiredItemType);

      _data = CreateDataStrategyForStandAloneCollection (dataStore, requiredItemType, this);
      SetIsReadOnly(makeCollectionReadOnly);
    }

    // methods and properties

    /// <summary>
    /// Gets the number of elements contained in the <see cref="DomainObjectCollection"/>.
    /// </summary>
    /// <returns>
    /// The number of elements contained in the <see cref="DomainObjectCollection"/>.
    /// </returns>
    public int Count
    {
      get { return _data.Count; }
    }

    /// <summary>
    /// Gets a value indicating whether this collection is read-only.
    /// </summary>
    /// <returns>true if this collection is read-only; otherwise, false.
    /// </returns>
    public bool IsReadOnly
    {
      get { return _isReadOnly; }
    }

    public ICollectionEndPoint AssociatedEndPoint
    {
      get { return _data.AssociatedEndPoint; }
    }


    /// <summary>
    /// Adds all items of the given <see cref="DomainObjectCollection"/> to the <see cref="DomainObjectCollection" />, that are not already part of it.
    /// </summary>
    /// <remarks>
    /// <para>The method does not modify the given <see cref="DomainObjectCollection"/>.</para>
    /// <para>To check if an item is already part of the <see cref="DomainObjectCollection" /> its <see cref="DomainObject.ID"/> is used. 
    /// <see cref="Combine"/> does not check if the item references are identical.
    /// The two <see cref="DomainObjectCollection"/>s could contain items with the same ID, but reference different <see cref="DomainObject"/>s, 
    /// if the collections contain items of different <see cref="ClientTransaction"/>s.</para>
    /// </remarks>
    /// <param name="domainObjects">The <see cref="DomainObjectCollection"/> to add items from. Must not be <see langword="null"/>.</param>
    /// <exception cref="System.ArgumentNullException"><paramref name="domainObjects"/> is <see langword="null"/>.</exception>
    /// <exception cref="DataManagement.ClientTransactionsDifferException">
    ///   <paramref name="domainObjects"/> contains a <see cref="DomainObject"/> that belongs to a <see cref="ClientTransaction"/> that is different from 
    ///   the <see cref="ClientTransaction"/> managing this collection. 
    ///   This applies only to <see cref="DomainObjectCollection"/>s that represent a relation.
    /// </exception>
    public void Combine (DomainObjectCollection domainObjects)
    {
      ArgumentUtility.CheckNotNull ("domainObjects", domainObjects);
      CheckNotReadOnly ("A read-only collection cannot be combined with another collection.");

      foreach (DomainObject domainObject in domainObjects)
      {
        if (!Contains (domainObject.ID))
          Add (domainObject);
      }

      Touch (); // TODO: This call to Touch cannot be moved to the IDomainObjectCollectionData implementation. Document this.
    }

    /// <summary>
    /// Returns all items of a given <see cref="DomainObjectCollection"/> that are not part of the <see cref="DomainObjectCollection" />.
    /// </summary>
    /// <remarks>
    /// <para>The method does not modify the given <see cref="DomainObjectCollection"/> nor the <see cref="DomainObjectCollection" /> itself.</para>
    /// <para>To check if an item is already part of the <see cref="DomainObjectCollection" /> its <see cref="DomainObject.ID"/> 
    /// and the item reference are considered. 
    /// Therefore <see cref="GetItemsNotInCollection"/> does return items with the same ID but are from different <see cref="ClientTransaction"/>s.</para>
    /// </remarks>
    /// <param name="domainObjects">The collection to evaluate. Must not be <see langword="null"/>.</param>
    /// <returns>A <see cref="DomainObjectCollection"/> with all items of <paramref name="domainObjects"/> that are not part of the collection.</returns>
    /// <exception cref="System.ArgumentNullException"><paramref name="domainObjects"/> is <see langword="null"/>.</exception>
    public DomainObjectCollection GetItemsNotInCollection (DomainObjectCollection domainObjects)
    {
      ArgumentUtility.CheckNotNull ("domainObjects", domainObjects);

      DomainObjectCollection itemsNotInCollection = new DomainObjectCollection();

      foreach (DomainObject domainObject in domainObjects)
      {
        if (!ContainsObject (domainObject))
          itemsNotInCollection.Add (domainObject);
      }

      return itemsNotInCollection;
    }

    /// <summary>
    /// Gets the required <see cref="Type"/> for all members of the collection.
    /// </summary>
    public Type RequiredItemType
    {
      get { return _requiredItemType ?? _data.RequiredItemType; }
    }

    /// <summary>
    /// Determines whether the <see cref="DomainObjectCollection"/> contains a reference to the specified <paramref name="domainObject"/>.
    /// </summary>
    /// <param name="domainObject">The <see cref="DomainObject"/> to locate in the <see cref="DomainObjectCollection"/>. Must not be <see langword="null"/>.</param>
    /// <returns><see langword="true"/> if <paramref name="domainObject"/> is found in the <see cref="DomainObjectCollection"/>; otherwise, false;</returns>
    /// <exception cref="System.ArgumentNullException"><paramref name="domainObject"/> is <see langword="null"/></exception>
    /// <remarks>This method only returns true, if the same reference is found in the collection.</remarks>
    [Obsolete ("Use Contains (ObjectID) to determine if the DomainObjectCollection contains a DomainObject with the specified ID or use"
        + " ContainsObject (DomainObject) to determine if the DomainObjectCollection contains a reference to the specified DomainObject."
            + " Note: In most scenarios Contains (ObjectID) should be used.", true)]
    // TODO: Remove this method after 01.06.2006
    public bool Contains (DomainObject domainObject)
    {
      return ContainsObject (domainObject);
    }

    /// <summary>
    /// Determines whether the <see cref="DomainObjectCollection"/> contains a reference to the specified <paramref name="domainObject"/>.
    /// </summary>
    /// <param name="domainObject">The <see cref="DomainObject"/> to locate in the <see cref="DomainObjectCollection"/>. Must not be <see langword="null"/>.</param>
    /// <returns><see langword="true"/> if <paramref name="domainObject"/> is found in the <see cref="DomainObjectCollection"/>; otherwise, false;</returns>
    /// <exception cref="System.ArgumentNullException"><paramref name="domainObject"/> is <see langword="null"/></exception>
    /// <remarks>
    /// <para>This method only returns true, if the same reference is found in the collection.</para>
    /// <para>Note: The <see cref="DomainObjectCollection" /> could contain an item with the same ID, 
    /// but reference a different <see cref="DomainObject"/>, 
    /// if the given <paramref name="domainObject" /> is part of a different <see cref="ClientTransaction"/>.</para></remarks>
    public bool ContainsObject (DomainObject domainObject)
    {
      ArgumentUtility.CheckNotNull ("domainObject", domainObject);

      return _data.ContainsObjectID (domainObject.ID) && object.ReferenceEquals (_data.GetObject (domainObject.ID), domainObject);
    }

    /// <summary>
    /// Determines whether an item is in the <see cref="DomainObjectCollection"/>.
    /// </summary>
    /// <param name="id">The <see cref="ObjectID"/> of the <see cref="DomainObject"/> to locate in the <see cref="DomainObjectCollection"/>. Must not be <see langword="null"/>.</param>
    /// <returns><see langword="true"/> if the <see cref="DomainObject"/> with the <paramref name="id"/> is found in the <see cref="DomainObjectCollection"/>; otherwise, false;</returns>
    /// <exception cref="System.ArgumentNullException"><paramref name="id"/> is <see langword="null"/></exception>
    public bool Contains (ObjectID id)
    {
      ArgumentUtility.CheckNotNull ("id", id);

      return _data.ContainsObjectID (id);
    }

    /// <summary>
    /// Gets or sets the <see cref="DomainObject"/> with a given <paramref name="index"/> in the <see cref="DomainObjectCollection"/>.
    /// </summary>
    /// <exception cref="System.ArgumentOutOfRangeException">
    ///   <paramref name="index"/> is less than zero.<br /> -or- <br />
    ///   <paramref name="index"/> is equal to or greater than the number of items in the collection.
    /// </exception>
    /// <exception cref="System.NotSupportedException">The collection is read-only.</exception>
    /// <exception cref="System.ArgumentException">
    ///   <paramref name="value"/> is not a derived type of <see cref="DomainObject"/> and of type <see cref="RequiredItemType"/> or a derived type.
    /// </exception>
    /// <exception cref="System.InvalidOperationException"><paramref name="value"/> is already part of the collection.</exception>
    public DomainObject this [int index]
    {
      get { return _data.GetObject (index); }
      set
      {
        CheckIndexForIndexer ("index", index);
        CheckNotReadOnly ("Cannot modify a read-only collection.");

        // If new value is null: This is actually a remove operation
        if (value == null)
        {
          RemoveAt (index);
          return;
        }

        CheckItemType (value, "value");

        if (Contains (value.ID) && !ReferenceEquals (this[index], value))
          throw CreateInvalidOperationException ("The object '{0}' is already part of this collection.", value.ID);

        _data.Replace (index, value);
      }
    }

    /// <summary>
    /// Gets the <see cref="DomainObject"/> with a given <see cref="ObjectID"/> from the <see cref="DomainObjectCollection"/>.
    /// </summary>
    /// <remarks>The indexer returns <see langword="null"/> if the given <paramref name="id"/> was not found.</remarks>
    public DomainObject this [ObjectID id]
    {
      get { return _data.GetObject (id); }
    }

    /// <summary>
    /// Adds a <see cref="DomainObject"/> to the collection.
    /// </summary>
    /// <param name="domainObject">The <see cref="DomainObject"/> to add. Must not be <see langword="null"/>.</param>
    /// <returns>The zero-based index where the <paramref name="domainObject"/> has been added.</returns>
    /// <exception cref="System.NotSupportedException">The collection is read-only.</exception>
    /// <exception cref="System.ArgumentNullException"><paramref name="domainObject"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentTypeException"><paramref name="domainObject"/> is not of type <see cref="RequiredItemType"/> or one of its derived types.</exception>
    /// <exception cref="DataManagement.ClientTransactionsDifferException">
    ///   <paramref name="domainObject"/> belongs to a <see cref="ClientTransaction"/> that is different from the <see cref="ClientTransaction"/> 
    ///   managing this collection. 
    ///   This applies only to <see cref="DomainObjectCollection"/>s that represent a relation.
    /// </exception>
    public int Add (DomainObject domainObject)
    {
      ArgumentUtility.CheckNotNull ("domainObject", domainObject);
      CheckItemType (domainObject, "domainObject");
      CheckNotReadOnly ("Cannot add an item to a read-only collection.");

      if (Contains (domainObject.ID))
      {
        throw CreateArgumentException (
            "domainObject", "Cannot add object '{0}' already part of this collection.", domainObject.ID);
      }

      _data.Insert (Count, domainObject);

      return Count - 1;
    }

    /// <summary>
    /// Adds a range of <see cref="DomainObject"/> instances to this collection, calling <see cref="Add"/> for each single item.
    /// </summary>
    /// <param name="domainObjects">The domain objects to add.</param>
    /// <exception cref="ArgumentNullException">The <paramref name="domainObjects"/> parameter is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentItemNullException">The range contains a <see langword="null"/> element.</exception>
    /// <exception cref="ArgumentItemDuplicateException">The range contains a duplicate element.</exception>
    /// <exception cref="System.NotSupportedException">The collection is read-only.</exception>
    /// <exception cref="ArgumentItemTypeException">An element in the range is not of type <see cref="RequiredItemType"/> or one of its derived types.</exception>
    /// <exception cref="DataManagement.ClientTransactionsDifferException">
    ///   An element in the range belongs to a <see cref="ClientTransaction"/> that is different from the <see cref="ClientTransaction"/> 
    ///   managing this collection. 
    ///   This applies only to <see cref="DomainObjectCollection"/>s that represent a relation.
    /// </exception>
    public void AddRange (IEnumerable domainObjects)
    {
      ArgumentUtility.CheckNotNull ("domainObjects", domainObjects);

      int index = 0;
      foreach (DomainObject domainObject in domainObjects)
      {
        if (domainObject == null)
          throw new ArgumentItemNullException ("domainObjects", index);
        if (Contains (domainObject.ID))
          throw new ArgumentItemDuplicateException ("domainObjects", index, domainObject.ID);

        try
        {
          Add (domainObject);
        }
        catch (ArgumentTypeException ex)
        {
          throw new ArgumentItemTypeException ("domainObjects", index, ex.ExpectedType, ex.ActualType);
        }
        ++index;
      }
    }


    /// <summary>
    /// Removes a <see cref="DomainObject"/> from the collection.
    /// </summary>
    /// <param name="index">The index of the <see cref="DomainObject"/> to remove.</param>
    /// <exception cref="System.ArgumentOutOfRangeException">
    ///   <paramref name="index"/> is less than zero.<br /> -or- <br />
    ///   <paramref name="index"/> is equal to or greater than the number of items in the collection.
    /// </exception>
    /// <exception cref="System.NotSupportedException">The collection is read-only.</exception>
    public void RemoveAt (int index)
    {
      Remove (this[index]);
    }

    /// <summary>
    /// Removes a <see cref="DomainObject"/> from the collection.
    /// </summary>
    /// <param name="id">The <see cref="ObjectID"/> of the <see cref="DomainObject"/> to remove. Must not be <see langword="null"/>.</param>
    /// <exception cref="System.ArgumentNullException"><paramref name="id"/> is <see langword="null"/>.</exception>
    /// <exception cref="System.NotSupportedException">The collection is read-only.</exception>
    public void Remove (ObjectID id)
    {
      ArgumentUtility.CheckNotNull ("id", id);
      CheckNotReadOnly ("Cannot remove an item from a read-only collection.");

      DomainObject domainObject = this[id];
      if (domainObject != null)
        Remove (domainObject);
      else
        Touch (); // TODO: This call to Touch will be handled by EndPointDelegatingCollectionData.
    }

    /// <summary>
    /// Removes a <see cref="DomainObject"/> from the collection.
    /// </summary>
    /// <returns>True if the collection contained the given object when the method was called; false otherwise.
    /// </returns>
    /// <remarks>
    ///   If <see cref="Remove(Remotion.Data.DomainObjects.DomainObject)"/> is called with an object that is not in the collection, no exception is thrown, and no events are raised. 
    /// </remarks>
    /// <param name="domainObject">The <see cref="DomainObject"/> to remove. Must not be <see langword="null"/>.</param>
    /// <exception cref="System.ArgumentNullException"><paramref name="domainObject"/> is <see langword="null"/>.</exception>
    /// <exception cref="System.ArgumentException"><paramref name="domainObject"/> has the same <see cref="ObjectID"/> as an object in this collection, but it is a 
    /// different object reference. You can use <see cref="Remove(ObjectID)"/> to remove an object if you only know its <see cref="ObjectID"/>.</exception>
    /// <exception cref="System.NotSupportedException">The collection is read-only.</exception>
    /// <exception cref="DataManagement.ClientTransactionsDifferException">
    ///   <paramref name="domainObject"/> belongs to a <see cref="ClientTransaction"/> that is different from the <see cref="ClientTransaction"/> 
    ///   managing this collection. 
    ///   This applies only to <see cref="DomainObjectCollection"/>s that represent a relation.
    /// </exception>
    public bool Remove (DomainObject domainObject)
    {
      ArgumentUtility.CheckNotNull ("domainObject", domainObject);
      CheckNotReadOnly ("Cannot remove an item from a read-only collection.");

      // Do not perform remove if domain object is not part of this collection     
      if (this[domainObject.ID] == null)
      {
        Touch(); // TODO: This call to Touch will be handled by EndPointDelegatingCollectionData.
        return false;
      }
      else if (this[domainObject.ID] != domainObject)
      {
        var message = "The object to be removed has the same ID as an object in this collection, but is a different object reference.";
        throw new ArgumentException (message, "domainObject");
      } 

      _data.Remove (domainObject);
      return true;
    }

    /// <summary>
    /// Removes all items from the <see cref="DomainObjectCollection"/>.
    /// </summary>
    /// <exception cref="System.NotSupportedException">The collection is read-only.</exception>
    public void Clear()
    {
      CheckNotReadOnly ("Cannot clear a read-only collection.");

      for (int i = Count - 1; i >= 0; i--)
        Remove (this[i].ID);

      Touch (); // TODO: This call to Touch will be handled by EndPointDelegatingDomainObjectCollectionData.
    }

    /// <summary>
    /// Returns the zero-based index of a given <see cref="DomainObject"/> in the collection.
    /// </summary>
    /// <param name="domainObject">The <paramref name="domainObject"/> to locate in the collection.</param>
    /// <returns>The zero-based index of the <paramref name="domainObject"/>, if found; otherwise, -1.</returns>
    public int IndexOf (DomainObject domainObject)
    {
      if (domainObject != null)
        return IndexOf (domainObject.ID);
      else
        return -1;
    }

    /// <summary>
    /// Returns the zero-based index of a given <see cref="ObjectID"/> in the collection.
    /// </summary>
    /// <param name="id">The <paramref name="id"/> to locate in the collection.</param>
    /// <returns>The zero-based index of the <paramref name="id"/>, if found; otherwise, -1.</returns>
    public int IndexOf (ObjectID id)
    {
      if (id != null)
        return _data.IndexOf (id);
      else
        return -1;
    }

    /// <summary>
    /// Inserts a <see cref="DomainObject"/> into the collection at the specified index.
    /// </summary>
    /// <param name="index">The zero-based <paramref name="index"/> at which the item should be inserted.</param>
    /// <param name="domainObject">The <paramref name="domainObject"/> to add. Must not be <see langword="null"/>.</param>
    /// <exception cref="System.NotSupportedException">The collection is read-only.</exception>
    /// <exception cref="System.ArgumentOutOfRangeException">
    ///   <paramref name="index"/> is less than zero.<br /> -or- <br />
    ///   <paramref name="index"/> is greater than the number of items in the collection.
    /// </exception>
    /// <exception cref="System.ArgumentNullException"><paramref name="domainObject"/> is <see langword="null"/>.</exception>
    /// <exception cref="System.ArgumentException">
    ///   The <paramref name="domainObject"/> already exists in the collection.<br /> -or- <br />
    ///   <paramref name="domainObject"/> is not of type <see cref="RequiredItemType"/> or one of its derived types.
    /// </exception>
    /// <exception cref="DataManagement.ClientTransactionsDifferException">
    ///   <paramref name="domainObject"/> belongs to a <see cref="ClientTransaction"/> that is different from the <see cref="ClientTransaction"/> 
    ///   managing this collection. 
    ///   This applies only to <see cref="DomainObjectCollection"/>s that represent a relation.
    /// </exception>
    public void Insert (int index, DomainObject domainObject)
    {
      ArgumentUtility.CheckNotNull ("domainObject", domainObject);
      CheckIndexForInsert ("index", index);
      CheckNotReadOnly ("Cannot insert an item into a read-only collection.");
      CheckItemType (domainObject, "domainObject");

      if (Contains (domainObject.ID))
      {
        throw CreateArgumentException (
            "domainObject", "Cannot insert object '{0}' already part of this collection.", domainObject.ID);
      }


      _data.Insert (index, domainObject);
    }

    /// <summary>
    /// Copies the elements of the collection to an <see cref="T:System.Array"/>, starting at a particular <see cref="T:System.Array"/> index.
    /// </summary>
    /// <param name="array">The one-dimensional <see cref="T:System.Array"/> that is the destination of the elements copied from the collection. The 
    /// <see cref="T:System.Array"/> must have zero-based indexing.</param>
    /// <param name="index">The zero-based index in <paramref name="array"/> at which copying begins.</param>
    /// <exception cref="T:System.ArgumentNullException">
    /// 	<paramref name="array"/> is null.
    /// </exception>
    /// <exception cref="T:System.ArgumentOutOfRangeException">
    /// 	<paramref name="index"/> is less than zero.
    /// </exception>
    /// <exception cref="T:System.ArgumentException">
    /// 	<paramref name="array"/> is multidimensional.
    /// -or-
    /// <paramref name="index"/> is equal to or greater than the length of <paramref name="array"/>.
    /// -or-
    /// The number of elements in this collection is greater than the available space from <paramref name="index"/> to the end of the destination 
    /// <paramref name="array"/>.
    /// </exception>
    /// <exception cref="T:System.ArgumentException">
    /// The type of the source collection cannot be cast automatically to the type of the destination <paramref name="array"/>.
    /// </exception>
    public void CopyTo (Array array, int index)
    {
      _data.ToArray ().CopyTo (array, index);
    }

    /// <summary>
    /// Returns an implementation of <see cref="IDomainObjectCollectionData"/> that represents the data held by this collection but will
    /// not raise any notifications. This means that no events wil be raised when the data is manipulated and no bidirectional notifications will be
    /// performed. The returned object also does not check whether this collection is read-only.
    /// </summary>
    /// <returns>An implementation of <see cref="IDomainObjectCollectionData"/> that represents the data held by this collection and will
    /// not raise any notifications and manipulation.</returns>
    /// <remarks>
    /// <para>
    /// When the collection is part of a
    /// <see cref="CollectionEndPoint"/>, the manipulations performed on the data will not trigger bidirectional modifications on related objects,
    /// so manipulations must be performed with care, otherwise inconsistent state might arise. The end point will also not be marked as touched by 
    /// manipulations performed on the returned data. (The end point's <see cref="CollectionEndPoint.HasChanged"/> method might still return 
    /// <see langword="true" />, though, since it compares the original data with the collection's contents.)
    /// </para>
    /// </remarks>
    protected IDomainObjectCollectionData GetNonNotifyingData ()
    {
      return new ArgumentCheckingCollectionDataDecorator (RequiredItemType, _data.GetUndecoratedDataStore());
    }

    #region Explicitly implemeted IList and ICollection Members

    /// <summary>
    /// Gets or sets the element at the specified index. 
    /// </summary>
    /// <exception cref="Remotion.Utilities.ArgumentTypeException"><paramref name="value"/> is not of type <see cref="DomainObject"/> or a derived type.</exception>
    object IList.this [int index]
    {
      get { return this[index]; }
      set
      {
        ArgumentUtility.CheckType<DomainObject> ("value", value);

        this[index] = (DomainObject) value;
      }
    }

    /// <summary>
    /// Inserts an item to the IList at the specified position
    /// </summary>
    /// <param name="index">The zero-based index at which <paramref name="value"/> should be inserted.</param>
    /// <param name="value">The <see cref="DomainObject"/> to insert into the <see cref="IList"/>. Must not be <see langword="null"/>.</param>
    /// <exception cref="DataManagement.ClientTransactionsDifferException">
    ///   <paramref name="value"/> is a <see cref="DomainObject"/> that belongs to a <see cref="ClientTransaction"/> that is different 
    ///   from the <see cref="ClientTransaction"/> managing this collection. 
    ///   This applies only to <see cref="DomainObjectCollection"/>s that represent a relation.
    /// </exception>
    /// <exception cref="System.ArgumentNullException"><paramref name="value"/> is <see langword="null"/>.</exception>
    /// <exception cref="Remotion.Utilities.ArgumentTypeException"><paramref name="value"/> is not of type <see cref="DomainObject"/> or a derived type.</exception>
    void IList.Insert (int index, object value)
    {
      ArgumentUtility.CheckNotNullAndType<DomainObject> ("value", value);

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
    /// Determines whether the <see cref="IList"/> contains a specific <see cref="DomainObject"/> or <see cref="ObjectID"/>.
    /// </summary>
    /// <param name="value">The <see cref="DomainObject"/> or <see cref="ObjectID"/> to locate in the <see cref="IList"/>.</param>
    /// <returns><see langword="true"/> if the <see cref="DomainObject"/> or <see cref="ObjectID"/> is found in the <see cref="IList"/>; otherwise, <see langword="false"/></returns>
    bool IList.Contains (object value)
    {
      if (value is DomainObject)
        return ContainsObject ((DomainObject) value);

      if (value is ObjectID)
        return Contains ((ObjectID) value);

      return false;
    }

    /// <summary>
    /// Determines the index of a specific item in the <see cref="IList"/>.
    /// </summary>
    /// <param name="value">The <see cref="Object"/> to locate in the <see cref="IList"/>.</param>
    /// <returns>The index of <paramref name="value"/> if found in the list; otherwise, -1.</returns>
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
    /// <param name="value">The <see cref="DomainObject"/> to add to the <see cref="IList"/>. Must not be <see langword="null"/>.</param>
    /// <returns>The position into which the new element was inserted.</returns>
    /// <exception cref="DataManagement.ClientTransactionsDifferException">
    ///   <paramref name="value"/> is a <see cref="DomainObject"/> that belongs to a <see cref="ClientTransaction"/> that is different 
    ///   from the <see cref="ClientTransaction"/> managing this collection. 
    ///   This applies only to <see cref="DomainObjectCollection"/>s that represent a relation.
    /// </exception>
    /// <exception cref="System.ArgumentNullException"><paramref name="value"/> is <see langword="null"/>.</exception>
    /// <exception cref="Remotion.Utilities.ArgumentTypeException"><paramref name="value"/> is not of type <see cref="DomainObject"/> or a derived type.</exception>
    int IList.Add (object value)
    {
      ArgumentUtility.CheckNotNullAndType<DomainObject> ("value", value);

      return Add ((DomainObject) value);
    }

    object ICollection.SyncRoot
    {
      get { return this; }
    }

    bool ICollection.IsSynchronized
    {
      get { return false; }
    }

    /// <summary>
    /// Gets a value indicating if the collection has a fixed size. Always returns false.
    /// </summary>
    bool IList.IsFixedSize
    {
      get { return false; }
    }

    #endregion

    #region IEnumerable<DomainObject> members
    public IEnumerator<DomainObject> GetEnumerator ()
    {
      return _data.GetEnumerator ();
    }

    IEnumerator IEnumerable.GetEnumerator ()
    {
      return GetEnumerator ();
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
    object ICloneable.Clone()
    {
      return Clone();
    }

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
    public DomainObjectCollection Clone()
    {
      return Clone (IsReadOnly);
    }

    /// <summary>
    /// Creates a shallow copy of this collection. No events are raised on the clone.
    /// </summary>
    /// <param name="makeCloneReadOnly">Specifies whether the cloned collection should be read-only.</param>
    /// <returns>The cloned collection.</returns>
    /// <remarks>
    /// <para>
    /// A shallow copy creates a new <see cref="DomainObjectCollection"/> instance
    /// which can be independently modified without affecting the original collection.
    /// Thus meaning the references to the domain objects are copied, not the domain objects themselves.<br/><br/>
    /// The <see cref="System.Type"/> of the cloned collection is equal to the <see cref="System.Type"/> of the <see cref="DomainObjectCollection" />.
    /// </para>
    /// <para>
    /// The clone is always a stand-alone collection, even when the source was associated with a collection end point.
    /// </para>
    /// </remarks>
    public virtual DomainObjectCollection Clone (bool makeCloneReadOnly)
    {
      var clone = new DomainObjectCollectionFactory().CreateCollection (GetType (), _data, RequiredItemType);

      Assertion.IsTrue (clone._data != _data);
      Assertion.IsTrue (clone._data.GetUndecoratedDataStore () != _data);
      Assertion.IsTrue (clone.RequiredItemType == RequiredItemType);

      clone.SetIsReadOnly (makeCloneReadOnly);
      return clone;
    }

    #endregion

    /// <summary>
    /// Performs a rollback of the collection by replacing the items in the collection with the items of a given <see cref="DomainObjectCollection"/>.
    /// </summary>
    /// <param name="originalDomainObjects">A <see cref="DomainObjectCollection"/> containing the original items of the collection. Must not be <see langword="null"/>.</param>
    /// <remarks>
    ///   This method is only called on <see cref="DomainObjectCollection"/>s representing the current values 
    ///   of a one-to-many relation during the rollback operation of the associated <see cref="ClientTransaction"/>. 
    ///   A derived collection should replace its internal state with the state of <paramref name="originalDomainObjects"/>.
    /// </remarks>
    /// <exception cref="System.ArgumentNullException"><paramref name="originalDomainObjects"/> is <see langword="null"/>.</exception>
    protected internal virtual void Rollback (DomainObjectCollection originalDomainObjects)
    {
      ArgumentUtility.CheckNotNull ("originalDomainObjects", originalDomainObjects);

      ReplaceItems (originalDomainObjects);
    }

    /// <summary>
    /// Performs a commit of the collection by replacing the items in the collection with the items of a given <see cref="DomainObjectCollection"/>.
    /// </summary>
    /// <param name="domainObjects">A <see cref="DomainObjectCollection"/> containing the new items for the collection. Must not be <see langword="null"/>.</param>
    /// <remarks>
    ///   This method is only called on <see cref="DomainObjectCollection"/>s representing the original values 
    ///   of a one-to-many relation during the commit operation of the associated <see cref="ClientTransaction"/>. 
    ///   A derived collection should replace its internal state with the state of <paramref name="domainObjects"/>.
    /// </remarks>
    /// <exception cref="System.ArgumentNullException"><paramref name="domainObjects"/> is <see langword="null"/>.</exception>
    protected internal virtual void Commit (DomainObjectCollection domainObjects)
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
    /// <exception cref="DataManagement.ClientTransactionsDifferException">
    ///   <paramref name="domainObjects"/> contains a <see cref="DomainObject"/> that belongs to a <see cref="ClientTransaction"/> that is different from 
    ///   the <see cref="ClientTransaction"/> managing this collection. 
    ///   This applies only to <see cref="DomainObjectCollection"/>s that represent a relation.
    /// </exception>
    // TODO 1870: Remove this method, move logic to CollectionEndPoint.
    protected internal virtual void ReplaceItems (DomainObjectCollection domainObjects)
    {
      var nonNotifyingData = GetNonNotifyingData ();
      nonNotifyingData.ReplaceContents (domainObjects.Cast<DomainObject> ());
      Touch (); // TODO: This call to Touch cannot be moved to the IDomainObjectCollectionData implementation. However, this method is removed anyway.
    }

    /// <summary>
    /// Raises the <see cref="Adding"/> event.
    /// </summary>
    /// <param name="args">A <see cref="DomainObjectCollectionChangeEventArgs"/> object that contains the event data.</param>
    protected virtual void OnAdding (DomainObjectCollectionChangeEventArgs args)
    {
      if (Adding != null)
        Adding (this, args);
    }

    /// <summary>
    /// Raises the <see cref="Added"/> event.
    /// </summary>
    /// <param name="args">A <see cref="DomainObjectCollectionChangeEventArgs"/> object that contains the event data.</param>
    /// <remarks>This method can be used to adjust internal state whenever a new item is added to the collection.</remarks>
    protected virtual void OnAdded (DomainObjectCollectionChangeEventArgs args)
    {
      if (Added != null)
        Added (this, args);
    }

    /// <summary>
    /// Raises the <see cref="Removing"/> event.
    /// </summary>
    /// <param name="args">A <see cref="DomainObjectCollectionChangeEventArgs"/> object that contains the event data.</param>
    ///   If the collection is cleared through the <see cref="Clear"/> method <see cref="OnRemoving"/> 
    ///   is called for every item.
    protected virtual void OnRemoving (DomainObjectCollectionChangeEventArgs args)
    {
      if (Removing != null)
        Removing (this, args);
    }

    /// <summary>
    /// Raises the <see cref="Removed"/> event.
    /// </summary>
    /// <param name="args">A <see cref="DomainObjectCollectionChangeEventArgs"/> object that contains the event data.</param>
    /// <remarks>
    ///   This method can be used to adjust internal state whenever an item is removed from the collection.
    ///   If the collection is cleared through the <see cref="Clear"/> method <see cref="OnRemoved"/> 
    ///   is called for every item.
    /// </remarks>
    protected virtual void OnRemoved (DomainObjectCollectionChangeEventArgs args)
    {
      if (Removed != null)
        Removed (this, args);
    }

    /// <summary>
    /// The method is invoked immediately before the <see cref="DomainObject"/> holding this collection is deleted if the <see cref="DomainObjectCollection" /> represents a one-to-many relation.
    /// </summary>
    /// <remarks>
    /// During the delete process of a <see cref="DomainObject"/> all <see cref="DomainObject"/>s are removed from the <see cref="DomainObjectCollection" /> without notifying other objects.
    /// Immediately before all <see cref="DomainObject"/>s will be removed the <see cref="OnDeleting"/> method is invoked.
    /// To clear any internal state or to unsubscribe from events whenever the <see cref="DomainObject"/> holding this collection is deleted 
    /// use the <see cref="OnDeleted"/> method, because the operation could be cancelled after the <see cref="OnDeleting"/> method has been called.<br/><br/>
    /// <note type="inotes">Inheritors overriding this method must not throw an exception from the override.</note>
    /// </remarks>
    protected virtual void OnDeleting()
    {
    }

    /// <summary>
    /// The method is invoked after the <see cref="DomainObject"/> holding this collection is deleted if the <see cref="DomainObjectCollection" /> represents a one-to-many relation.
    /// </summary>
    /// <remarks>
    /// During the delete process of a <see cref="DomainObject"/> all <see cref="DomainObject"/>s are removed from the <see cref="DomainObjectCollection" /> without notifying other objects.
    /// After all <see cref="DomainObject"/>s have been removed the <see cref="OnDeleted"/> method is invoked 
    /// to allow derived collections to adjust their internal state or to unsubscribe from events of contained <see cref="DomainObject"/>s.<br/><br/>
    /// <note type="inotes">Inheritors overriding this method must not throw an exception from the override.</note>
    /// </remarks>
    protected virtual void OnDeleted()
    {
    }

    protected void SetIsReadOnly (bool isReadOnly)
    {
      _isReadOnly = isReadOnly;
    }

    private void CheckItemType (DomainObject domainObject, string argumentName)
    {
      if (RequiredItemType != null && !RequiredItemType.IsInstanceOfType (domainObject))
      {
        string message = string.Format ("Values of type '{0}' cannot be added to this collection. Values must be of type '{1}' or derived from '{1}'.",
                                        domainObject.GetPublicDomainObjectType (), RequiredItemType);
        throw new ArgumentTypeException (message, argumentName, RequiredItemType, domainObject.GetPublicDomainObjectType());
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

    internal void CopyEventHandlersFrom (DomainObjectCollection source)
    {
      ArgumentUtility.CheckNotNull ("source", source);

      Adding += source.Adding;
      Added += source.Added;
      Removing += source.Removing;
      Removed += source.Removed;
    }

    private void Touch () // TODO: Replaced by EndPointDelegatingCollectionData.
    {
      if (AssociatedEndPoint != null)
        AssociatedEndPoint.Touch ();
    }

    private void CheckIndexForInsert (string argumentName, int index)
    {
      if (index < 0 || index > Count)
      {
        throw new ArgumentOutOfRangeException (
            argumentName,
            index,
            "Index is out of range. Must be non-negative and less than or equal to the size of the collection.");
      }
    }

    private void CheckIndexForIndexer (string argumentName, int index)
    {
      if (index < 0 || index >= Count)
      {
        throw new ArgumentOutOfRangeException (
            argumentName,
            index,
            "Index is out of range. Must be non-negative and less than the size of the collection.");
      }
    }

    void IDomainObjectCollectionEventRaiser.BeginAdd (int index, DomainObject domainObject)
    {
      ArgumentUtility.CheckNotNull ("domainObject", domainObject);
      OnAdding (new DomainObjectCollectionChangeEventArgs (domainObject));
    }

    void IDomainObjectCollectionEventRaiser.EndAdd (int index, DomainObject domainObject)
    {
      ArgumentUtility.CheckNotNull ("domainObject", domainObject);
      OnAdded (new DomainObjectCollectionChangeEventArgs (domainObject));
    }

    void IDomainObjectCollectionEventRaiser.BeginRemove (int index, DomainObject domainObject)
    {
      ArgumentUtility.CheckNotNull ("domainObject", domainObject);
      OnRemoving (new DomainObjectCollectionChangeEventArgs (domainObject));
    }

    void IDomainObjectCollectionEventRaiser.EndRemove (int index, DomainObject domainObject)
    {
      ArgumentUtility.CheckNotNull ("domainObject", domainObject);
      OnRemoved (new DomainObjectCollectionChangeEventArgs (domainObject));
    }

    void IDomainObjectCollectionEventRaiser.BeginDelete ()
    {
      OnDeleting ();
    }

    void IDomainObjectCollectionEventRaiser.EndDelete ()
    {
      OnDeleted ();
    }

    /// <summary>
    /// Creates an <see cref="IRelationEndPointModification"/> instance that encapsulates all the modifications required to associate this
    /// <see cref="DomainObjectCollection"/> with the given <paramref name="endPoint"/>. This API is usually not employed by framework users,
    /// but it is invoked when a collection-valued relation property is set to a new collection.
    /// </summary>
    /// <param name="endPoint">The end point to associate with. That end point's <see cref="ICollectionEndPoint.OppositeDomainObjects"/> collection
    /// must have the same type and <see cref="RequiredItemType"/> as this collection.</param>
    /// <exception cref="NotSupportedException">This collection is read-only.</exception>
    /// <exception cref="InvalidOperationException">This collection has another type or item type, or it is already associated with an end point.</exception>
    /// <remarks>
    /// <para>
    /// When the modification is executed, it replaces the given end point's <see cref="ICollectionEndPoint.OppositeDomainObjects"/> collection with 
    /// this <see cref="DomainObjectCollection"/> instance, which is transformed into an associated collection. The previous 
    /// <see cref="ICollectionEndPoint.OppositeDomainObjects"/> collection of the end point is transformed into a stand-alone collection.
    /// </para>
    /// <para>
    /// The returned <see cref="IRelationEndPointModification"/> should be executed as a bidirectional modification 
    /// (<see cref="IRelationEndPointModification.CreateBidirectionalModification"/>), otherwise inconsistent state might arise.
    /// </para>
    /// <para>
    /// This method does not check whether this collection is already associated with another end-point and should therefore be handled with care,
    /// otherwise an inconsistent state might result.
    /// </para>
    /// <para>
    /// This method is part of <see cref="DomainObjectCollection"/> rather than <see cref="CollectionEndPoint"/> because it is very tightly
    /// coupled to <see cref="DomainObjectCollection"/>: associating a collection will modify its inner data storage strategy, and 
    /// <see cref="CollectionEndPoint"/> has no possibility to do that.
    /// </para>
    /// </remarks>
    public IRelationEndPointModification CreateAssociationModification (CollectionEndPoint endPoint)
    {
      ArgumentUtility.CheckNotNull ("endPoint", endPoint);

      if (RequiredItemType != endPoint.OppositeDomainObjects.RequiredItemType)
        throw new InvalidOperationException ("This collection has a different item type than the end point's current opposite collection.");

      if (GetType () != endPoint.OppositeDomainObjects.GetType ())
      {
        var message = string.Format (
            "This collection ('{0}') is not of the same type as the end point's current opposite collection ('{1}').",
            GetType (),
            endPoint.OppositeDomainObjects.GetType ());
        throw new InvalidOperationException (message);
      }

      return new CollectionEndPointReplaceWholeCollectionModification (
          endPoint,
          this,
          new Transformer (endPoint.OppositeDomainObjects), 
          new Transformer (this));
    }

    private void CheckNotReadOnly (string message)
    {
      if (IsReadOnly)
        throw new NotSupportedException (message);
    }

    #region Obsolete
    [Obsolete (
        "This method is obsolete. Directly call the constructor DomainObjectCollection (IEnumerable<DomainObject>, Type) or use DomainObjectCollectionFactory.",
        true)]
    public static DomainObjectCollection Create (Type collectionType)
    {
      throw new NotImplementedException ();
    }

    [Obsolete (
        "This method is obsolete. Directly call the constructor DomainObjectCollection (IEnumerable<DomainObject>, Type) or use DomainObjectCollectionFactory.",
        true)]
    public static DomainObjectCollection Create (Type collectionType, Type requiredItemType)
    {
      throw new NotImplementedException ();
    }

    [Obsolete (
        "This method is obsolete. Directly call the constructor DomainObjectCollection (IEnumerable<DomainObject>, Type) or use DomainObjectCollectionFactory.",
        true)]
    public static DomainObjectCollection Create (Type collectionType, IEnumerable<DomainObject> contents)
    {
      throw new NotImplementedException ();
    }

    [Obsolete (
        "This method is obsolete. Directly call the constructor DomainObjectCollection (IEnumerable<DomainObject>, Type) or use DomainObjectCollectionFactory.",
        true)]
    public static DomainObjectCollection Create (
        Type collectionType,
        IEnumerable<DomainObject> contents,
        Type requiredItemType)
    {
      throw new NotImplementedException ();
    }
    #endregion
  }
}
