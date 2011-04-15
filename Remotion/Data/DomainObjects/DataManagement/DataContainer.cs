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
using System.Collections.Generic;
using System.Linq;
using Remotion.Data.DomainObjects.Infrastructure.Serialization;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.DataManagement
{
  /// <summary>
  /// Represents a container for the persisted properties of a DomainObject.
  /// </summary>
  public sealed class DataContainer : IFlattenedSerializable
  {
    // types

    private enum DataContainerStateType
    {
      Existing = 0,
      New = 1,
      Deleted = 2
    }

    // static members and constants

    /// <summary>
    /// Creates an empty <see cref="DataContainer"/> for a new <see cref="Remotion.Data.DomainObjects.DomainObject"/>. The <see cref="DataContainer"/>
    /// contains a new <see cref="PropertyValue"/> object for every <see cref="PropertyDefinition"/> in the respective <see cref="ClassDefinition"/>.
    /// The <see cref="DataContainer"/> has be to <see cref="DataManager.RegisterDataContainer">registered</see> with a 
    /// <see cref="ClientTransaction"/> and its <see cref="DomainObject"/> must <see cref="SetDomainObject">be set</see> before it can be used.
    /// </summary>
    /// <remarks>
    /// The new <see cref="DataContainer"/> has a <see cref="State"/> of <see cref="StateType.New"/>. All <see cref="PropertyValue"/>s for the class specified by <see cref="ObjectID.ClassID"/> are created.
    /// </remarks>
    /// <param name="id">The <see cref="ObjectID"/> of the new <see cref="DataContainer"/> to create. Must not be <see langword="null"/>.</param>
    /// <returns>The new <see cref="DataContainer"/>.</returns>
    /// <exception cref="System.ArgumentNullException"><paramref name="id"/> is <see langword="null"/>.</exception>
    public static DataContainer CreateNew (ObjectID id)
    {
      ArgumentUtility.CheckNotNull ("id", id);

      var propertyValues = GetDefaultPropertyValues(id);
      return new DataContainer (id, DataContainerStateType.New, null, propertyValues);
    }

    /// <summary>
    /// Creates an empty <see cref="DataContainer"/> for an existing <see cref="Remotion.Data.DomainObjects.DomainObject"/>. The <see cref="DataContainer"/>
    /// contain all <see cref="PropertyValue"/> objects, just as if it had been created with <see cref="CreateNew"/>, but the values for persistent 
    /// properties are set as returned by a lookup method.
    /// The <see cref="DataContainer"/> has be to <see cref="DataManager.RegisterDataContainer">registered</see> with a 
    /// <see cref="ClientTransaction"/> and its <see cref="DomainObject"/> must <see cref="SetDomainObject">be set</see> before it can be used.
    /// </summary>
    /// <remarks>
    /// The new <see cref="DataContainer"/> has a <see cref="State"/> of <see cref="StateType.Unchanged"/>. All <see cref="PropertyValue"/>s for the class specified by <see cref="ObjectID.ClassID"/> are created.
    /// </remarks>
    /// <param name="id">The <see cref="ObjectID"/> of the new <see cref="DataContainer"/> to create. Must not be <see langword="null"/>.</param>
    /// <param name="timestamp">The timestamp value of the existing object in the data source.</param>
    /// <param name="persistentValueLookup">A function object returning the value of a given persistent property for the existing object.</param>
    /// <returns>The new <see cref="DataContainer"/>.</returns>
    /// <exception cref="System.ArgumentNullException"><paramref name="id"/> is <see langword="null"/>.</exception>
    /// <exception cref="Mapping.MappingException">ClassDefinition of <paramref name="id"/> does not exist in mapping.</exception>
    public static DataContainer CreateForExisting (ObjectID id, object timestamp, Func<PropertyDefinition, object> persistentValueLookup)
    {
      ArgumentUtility.CheckNotNull ("id", id);

      var propertyValues = from propertyDefinition in id.ClassDefinition.GetPropertyDefinitions ()
                           select propertyDefinition.StorageClass == StorageClass.Persistent 
                              ? new PropertyValue (propertyDefinition, persistentValueLookup (propertyDefinition)) 
                              : new PropertyValue (propertyDefinition);

      return new DataContainer (id, DataContainerStateType.Existing, timestamp, propertyValues);
    }

    private static IEnumerable<PropertyValue> GetDefaultPropertyValues (ObjectID id)
    {
      return from propertyDefinition in id.ClassDefinition.GetPropertyDefinitions ()
             select new PropertyValue (propertyDefinition);
    }

    private readonly ObjectID _id;
    private readonly PropertyValueCollection _propertyValues;

    private ClientTransaction _clientTransaction;
    private object _timestamp;
    private DataContainerStateType _state;
    private DomainObject _domainObject;
    private RelationEndPointID[] _associatedRelationEndPointIDs = null;
    private bool _isDiscarded = false;
    private bool _hasBeenMarkedChanged = false;
    private bool _hasBeenChanged = false;

    // construction and disposing

    private DataContainer (ObjectID id, DataContainerStateType state, object timestamp, IEnumerable<PropertyValue> propertyValues)
    {
      ArgumentUtility.CheckNotNull ("id", id);
      ArgumentUtility.CheckNotNull ("propertyValues", propertyValues);

      // TODO: Remove this check, it doesn't provide any value
      if (id.ClassDefinition != MappingConfiguration.Current.ClassDefinitions.GetMandatory (id.ClassID))
      {
        string message = string.Format ("The ClassDefinition '{0}' of the ObjectID '{1}' is not part of the current mapping.", id.ClassDefinition, id);
        throw new ArgumentException (message, "id");
      }

      _id = id;
      _timestamp = timestamp;
      _state = state;

      _propertyValues = new PropertyValueCollection ();
      foreach (var propertyValue in propertyValues)
        _propertyValues.Add (propertyValue);

      _propertyValues.RegisterForChangeNotification (this);
    }

    // methods and properties

    /// <summary>
    /// Gets the value of the <see cref="PropertyValue"/> specified by <paramref name="propertyName"/>.
    /// </summary>
    /// <param name="propertyName">The name of the property. Must not be <see langword="null"/>.</param>
    /// <exception cref="System.ArgumentNullException"><paramref name="propertyName"/> is <see langword="null"/>.</exception>
    /// <exception cref="Remotion.Utilities.ArgumentEmptyException"><paramref name="propertyName"/> is an empty string.</exception>
    /// <exception cref="System.ArgumentException">The given <paramref name="propertyName"/> does not exist in the data container.</exception>
    /// <exception cref="ObjectInvalidException">The object is already discarded. See <see cref="ObjectInvalidException"/> for further information.</exception>
    public object this [string propertyName]
    {
      get
      {
        ArgumentUtility.CheckNotNullOrEmpty ("propertyName", propertyName);
        CheckNotDiscarded();

        return _propertyValues[propertyName].Value;
      }
      set
      {
        ArgumentUtility.CheckNotNullOrEmpty ("propertyName", propertyName);
        CheckNotDiscarded();

        _propertyValues[propertyName].Value = value;
      }
    }

    public bool HasBeenMarkedChanged
    {
      get { return _hasBeenMarkedChanged; }
    }

    /// <summary>
    /// Gets the value of the <see cref="PropertyValue"/> specified by <paramref name="propertyName"/>.
    /// </summary>
    /// <param name="propertyName">The name of the <see cref="PropertyValue"/>. Must not be <see langword="null"/>.</param>
    /// <returns>The value of the <see cref="PropertyValue"/>.</returns>
    /// <exception cref="System.ArgumentNullException"><paramref name="propertyName"/> is <see langword="null"/>.</exception>
    /// <exception cref="Remotion.Utilities.ArgumentEmptyException"><paramref name="propertyName"/> is an empty string.</exception>
    /// <exception cref="System.ArgumentException">The given <paramref name="propertyName"/> does not exist in the data container.</exception>
    /// <exception cref="ObjectInvalidException">The <see cref="DomainObject"/> is invalid and its <see cref="DataContainer"/> has been discarded. 
    /// See <see cref="ObjectInvalidException"/> for further information.</exception>
    public object GetValue (string propertyName)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("propertyName", propertyName);
      CheckNotDiscarded();

      return this[propertyName];
    }

    /// <summary>
    /// Sets the value of the <see cref="PropertyValue"/> specified by <paramref name="propertyName"/>.
    /// </summary>
    /// <param name="propertyName">The name of the <see cref="PropertyValue"/>. Must not be <see langword="null"/>.</param>
    /// <param name="value">The value the <see cref="PropertyValue"/> is set to.</param>
    /// <exception cref="System.ArgumentNullException"><paramref name="propertyName"/> is <see langword="null"/>.</exception>
    /// <exception cref="Remotion.Utilities.ArgumentEmptyException"><paramref name="propertyName"/> is an empty string.</exception>
    /// <exception cref="System.ArgumentException">The given <paramref name="propertyName"/> does not exist in the data container.</exception>
    /// <exception cref="ObjectInvalidException">The <see cref="DomainObject"/> is invalid and its <see cref="DataContainer"/> has been discarded. 
    /// See <see cref="ObjectInvalidException"/> for further information.</exception>
    public void SetValue (string propertyName, object value)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("propertyName", propertyName);
      CheckNotDiscarded();

      this[propertyName] = value;
    }


    /// <summary>
    /// Gets the <see cref="Remotion.Data.DomainObjects.ClientTransaction"/> which the <see cref="DataContainer"/> is part of.
    /// </summary>
    /// <exception cref="ObjectInvalidException">The <see cref="DomainObject"/> is invalid and its <see cref="DataContainer"/> has been discarded. 
    /// See <see cref="ObjectInvalidException"/> for further information.</exception>
    public ClientTransaction ClientTransaction
    {
      get
      {
        CheckNotDiscarded();

        if (_clientTransaction == null)
          throw new InvalidOperationException ("DataContainer has not been registered with a transaction.");

        return _clientTransaction;
      }
    }

    /// <summary>
    /// Gets a value indicating whether this instance has been registered with a <see cref="ClientTransaction"/>.
    /// </summary>
    /// <value>
    /// 	<see langword="true"/> if this instance has been registered; otherwise, <see langword="false"/>.
    /// </value>
    public bool IsRegistered
    {
      get 
      {
        return _clientTransaction != null;
      }
    }

    /// <summary>
    /// Gets the <see cref="Remotion.Data.DomainObjects.DomainObject"/> associated with the <see cref="DataContainer"/>.
    /// </summary>
    /// <exception cref="InvalidOperationException">This instance has not been associated with a <see cref="DomainObject"/> yet.</exception>
    public DomainObject DomainObject
    {
      get
      {
        if (!HasDomainObject)
          throw new InvalidOperationException ("This DataContainer has not been associated with a DomainObject yet.");

        return _domainObject;
      }
    }

    /// <summary>
    /// Gets a value indicating whether this instance has been associated with a <see cref="DomainObjects.DomainObject"/>.
    /// </summary>
    /// <value>
    /// 	<see langword="true"/> if this instance has a <see cref="DomainObjects.DomainObject"/>; otherwise, <see langword="false"/>.
    /// </value>
    public bool HasDomainObject
    {
      get { return _domainObject != null; }
    }

    /// <summary>
    /// Gets the <see cref="ObjectID"/> of the <see cref="DataContainer"/>.
    /// </summary>
    /// <remarks>
    /// This property can also be used when the <see cref="DataContainer"/> has been discarded.
    /// </remarks>
    public ObjectID ID
    {
      get { return _id; }
    }

    /// <summary>
    /// Gets the <see cref="Mapping.ClassDefinition"/> of the <see cref="DataContainer"/>.
    /// </summary>
    /// <exception cref="ObjectInvalidException">The <see cref="DomainObject"/> is invalid and its <see cref="DataContainer"/> has been discarded. 
    /// See <see cref="ObjectInvalidException"/> for further information.</exception>
    public ClassDefinition ClassDefinition
    {
      get
      {
        CheckNotDiscarded();
        return _id.ClassDefinition;
      }
    }

    /// <summary>
    /// Gets the <see cref="Type"/> of the <see cref="Remotion.Data.DomainObjects.DomainObject"/> of the <see cref="DataContainer"/>.
    /// </summary>
    /// <exception cref="ObjectInvalidException">The <see cref="DomainObject"/> is invalid and its <see cref="DataContainer"/> has been discarded. 
    /// See <see cref="ObjectInvalidException"/> for further information.</exception>
    public Type DomainObjectType
    {
      get
      {
        CheckNotDiscarded();
        return _id.ClassDefinition.ClassType;
      }
    }


    /// <summary>
    /// Gets the <see cref="PropertyValueCollection"/> of all <see cref="PropertyValue"/>s that are part of the <see cref="DataContainer"/>.
    /// </summary>
    /// <exception cref="ObjectInvalidException">The <see cref="DomainObject"/> is invalid and its <see cref="DataContainer"/> has been discarded. 
    /// See <see cref="ObjectInvalidException"/> for further information.</exception>
    public PropertyValueCollection PropertyValues
    {
      get
      {
        CheckNotDiscarded();
        return _propertyValues;
      }
    }


    /// <summary>
    /// Gets the state of the <see cref="DataContainer"/>.
    /// </summary>
    public StateType State
    {
      get
      {
        if (_isDiscarded)
          return StateType.Invalid;
        
        switch (_state)
        {
          case DataContainerStateType.New:
            return StateType.New;
          case DataContainerStateType.Deleted:
            return StateType.Deleted;
          default:
            Assertion.IsTrue (_state == DataContainerStateType.Existing);

            if (_hasBeenMarkedChanged || _hasBeenChanged)
              return StateType.Changed;
            else
              return StateType.Unchanged;
        }
      }
    }

    /// <summary>
    /// Marks an existing <see cref="DataContainer"/> as changed. <see cref="State"/> will return <see cref="StateType.Changed"/> after this method
    /// has been called.
    /// </summary>
    /// <exception cref="InvalidOperationException">This <see cref="DataContainer"/> is not in state <see cref="DataContainerStateType.Existing"/>.
    /// New or deleted objects cannot be marked as changed.</exception>
    /// <exception cref="ObjectInvalidException">The <see cref="DomainObject"/> is invalid and its <see cref="DataContainer"/> has been discarded. 
    /// See <see cref="ObjectInvalidException"/> for further information.</exception>
    public void MarkAsChanged ()
    {
      CheckNotDiscarded();
      if (_state != DataContainerStateType.Existing)
        throw new InvalidOperationException ("Only existing DataContainers can be marked as changed.");

      _hasBeenMarkedChanged = true;

      RaiseStateUpdatedNotification (StateType.Changed);
    }

    /// <summary>
    /// Gets the timestamp of the last committed change of the data in the <see cref="DataContainer"/>.
    /// </summary>
    /// <exception cref="ObjectInvalidException">The <see cref="DomainObject"/> is invalid and its <see cref="DataContainer"/> has been discarded. 
    /// See <see cref="ObjectInvalidException"/> for further information.</exception>
    public object Timestamp
    {
      get
      {
        CheckNotDiscarded();
        return _timestamp;
      }
    }

    /// <summary>
    /// Gets a value indicating the discarded status of the <see cref="DataContainer"/>.
    /// </summary>
    /// <remarks>
    /// For more information why and when a <see cref="DataContainer"/> is discarded see <see cref="ObjectInvalidException"/>.
    /// </remarks>
    public bool IsDiscarded
    {
      get { return _isDiscarded; }
    }

    public RelationEndPointID[] AssociatedRelationEndPointIDs
    {
      get
      {
        if (_associatedRelationEndPointIDs == null)
          _associatedRelationEndPointIDs = RelationEndPointID.GetAllRelationEndPointIDs (ID);

        return _associatedRelationEndPointIDs;
      }
    }

    public void SetTimestamp (object timestamp)
    {
      _timestamp = timestamp;
    }

    public void CommitState ()
    {
      CheckNotDiscarded ();

      if (_state == DataContainerStateType.Deleted)
        throw new InvalidOperationException ("Deleted data containers cannot be committed, they have to be discarded.");
      
      _hasBeenMarkedChanged = false;
      _hasBeenChanged = false;

      foreach (PropertyValue propertyValue in _propertyValues)
        propertyValue.CommitState ();

      _state = DataContainerStateType.Existing;
      RaiseStateUpdatedNotification (StateType.Unchanged);
    }

    public void RollbackState ()
    {
      CheckNotDiscarded ();

      if (_state == DataContainerStateType.New)
        throw new InvalidOperationException ("New data containers cannot be rolled back, they have to be discarded.");

      _hasBeenMarkedChanged = false;
      _hasBeenChanged = false;

      foreach (PropertyValue propertyValue in _propertyValues)
        propertyValue.RollbackState ();

      _state = DataContainerStateType.Existing;
      RaiseStateUpdatedNotification (StateType.Unchanged);
    }

    public void Delete ()
    {
      CheckNotDiscarded ();

      if (_state == DataContainerStateType.New)
        throw new InvalidOperationException ("New data containers cannot be deleted, they have to be discarded.");

      _state = DataContainerStateType.Deleted;
      RaiseStateUpdatedNotification (StateType.Deleted);
    }

    public void Discard ()
    {
      CheckNotDiscarded ();

      _propertyValues.Discard ();

      _isDiscarded = true;
      RaiseStateUpdatedNotification (StateType.Invalid);

      _clientTransaction = null;
    }

    public void SetPropertyDataFromSubTransaction (DataContainer source)
    {
      ArgumentUtility.CheckNotNull ("source", source);

      CheckNotDiscarded ();
      source.CheckNotDiscarded ();

      if (source.ClassDefinition != ClassDefinition)
      {
        var message = string.Format (
            "Cannot set this data container's property values from '{0}'; the data containers do not have the same class definition.",
            source.ID);
        throw new ArgumentException (message, "source");
      }

      for (int i = 0; i < _propertyValues.Count; ++i)
        _propertyValues[i].SetDataFromSubTransaction (source._propertyValues[i]);

      _hasBeenChanged = CalculatePropertyValueChangeState ();
      RaiseStateUpdatedNotification (State);
    }

    public void SetDomainObject (DomainObject domainObject)
    {
      ArgumentUtility.CheckNotNull ("domainObject", domainObject);

      if (domainObject.ID != null && domainObject.ID != _id)
        throw new ArgumentException ("The given DomainObject has another ID than this DataContainer.", "domainObject");
      if (_domainObject != null && _domainObject != domainObject)
        throw new InvalidOperationException ("This DataContainer has already been associated with a DomainObject.");

      _domainObject = domainObject;
    }

    internal void SetClientTransaction (ClientTransaction clientTransaction)
    {
      ArgumentUtility.CheckNotNull ("clientTransaction", clientTransaction);

      if (_clientTransaction != null)
        throw new InvalidOperationException ("This DataContainer has already been registered with a ClientTransaction.");

      _clientTransaction = clientTransaction;
    }

    internal void PropertyValueChanging (PropertyValueCollection propertyValueCollection, PropertyChangeEventArgs args)
    {
      if (_state == DataContainerStateType.Deleted)
        throw new ObjectDeletedException (_id);

      if (_clientTransaction != null)
        _clientTransaction.TransactionEventSink.PropertyValueChanging (
            _clientTransaction, 
            this, 
            args.PropertyValue, 
            args.OldValue, 
            args.NewValue);

      if (args.PropertyValue.Definition.PropertyType != typeof (ObjectID))
      {
        // To save memory, DomainObject does not register any event handlers with its data management infrastructure.
        // Therefore notification of DomainObject when changing property values is not organized through events.
        if (_domainObject != null)
          _domainObject.OnPropertyChanging (args);
      }
    }

    internal void PropertyValueChanged (PropertyValueCollection propertyValueCollection, PropertyChangeEventArgs args)
    {
      // set _hasBeenChanged to true if:
      // - we were not changed before this event (now we must be - the property only fires this event when it was set to a different value)
      // - the property indicates that it doesn't have the original value ("HasChanged")
      // - recalculation of all property change states indicates another property doesn't have its original value
      _hasBeenChanged = !_hasBeenChanged || args.PropertyValue.HasChanged || CalculatePropertyValueChangeState();
      RaiseStateUpdatedNotification (State);

      if (args.PropertyValue.Definition.PropertyType != typeof (ObjectID))
      {
        if (_domainObject != null)
        {
          // To save memory, DomainObject does not register any event handlers with its data management infrastructure.
          // Therefore notification of DomainObject when changing property values is not organized through events.
          _domainObject.OnPropertyChanged (args);
        }
      }

      if (_clientTransaction != null)
        _clientTransaction.TransactionEventSink.PropertyValueChanged (_clientTransaction, this, args.PropertyValue, args.OldValue, args.NewValue);
    }

    internal void PropertyValueReading (PropertyValue propertyValue, ValueAccess valueAccess)
    {
      if (_clientTransaction != null)
        _clientTransaction.TransactionEventSink.PropertyValueReading (_clientTransaction, this, propertyValue, valueAccess);
    }

    internal void PropertyValueRead (PropertyValue propertyValue, object value, ValueAccess valueAccess)
    {
      if (_clientTransaction != null)
        _clientTransaction.TransactionEventSink.PropertyValueRead (_clientTransaction, this, propertyValue, value, valueAccess);
    }

    private void CheckNotDiscarded ()
    {
      if (_isDiscarded)
        throw new ObjectInvalidException (_id);
    }

    /// <summary>
    /// Creates a copy of this data container and its state.
    /// </summary>
    /// <returns>A copy of this data container with the same <see cref="ObjectID"/> and the same property values. The copy's
    /// <see cref="ClientTransaction"/> and <see cref="DomainObject"/> are not set, so the returned <see cref="DataContainer"/> cannot be 
    /// used until it is registered with a <see cref="DomainObjects.ClientTransaction"/>. Its <see cref="DomainObject"/> is set via the
    /// <see cref="SetDomainObject"/> method.</returns>
    public DataContainer Clone (ObjectID id)
    {
      CheckNotDiscarded();

      var clonePropertyValues = from propertyValue in _propertyValues.Cast<PropertyValue>()
                                select new PropertyValue (propertyValue.Definition, propertyValue.Value);

      var clone = new DataContainer (id, _state, _timestamp, clonePropertyValues);

      clone._hasBeenMarkedChanged = _hasBeenMarkedChanged;
      clone._hasBeenChanged = _hasBeenChanged;

      Assertion.IsNull (clone._clientTransaction);
      Assertion.IsNull (clone._domainObject);
      return clone;
    }

    private bool CalculatePropertyValueChangeState ()
    {
      return _propertyValues.Cast<PropertyValue> ().Any (pv => pv.HasChanged);
    }

    private void RaiseStateUpdatedNotification (StateType state)
    {
      Assertion.DebugAssert (State == state);

      if (_clientTransaction != null)
        _clientTransaction.TransactionEventSink.DataContainerStateUpdated (_clientTransaction, this, state);
    }

    #region Serialization

// ReSharper disable UnusedMember.Local
    private DataContainer (FlattenedDeserializationInfo info)
    {
      ArgumentUtility.CheckNotNull ("info", info);

      _id = info.GetValueForHandle<ObjectID> ();
      _timestamp = info.GetValue<object>();
      _isDiscarded = info.GetBoolValue ();

      _propertyValues = new PropertyValueCollection ();
      foreach (var propertyValue in GetDefaultPropertyValues (_id))
        _propertyValues.Add (propertyValue);

      if (!_isDiscarded)
        RestorePropertyValuesFromData (info);
      
      _propertyValues.RegisterForChangeNotification (this);

      _clientTransaction = info.GetValueForHandle<ClientTransaction> ();
      _state = (DataContainerStateType) info.GetIntValue ();
      _domainObject = info.GetValueForHandle<DomainObject> ();
      _hasBeenMarkedChanged = info.GetBoolValue ();
      _hasBeenChanged = info.GetBoolValue();
    }
// ReSharper restore UnusedMember.Local

    private void RestorePropertyValuesFromData (FlattenedDeserializationInfo info)
    {
      int numberOfProperties = _propertyValues.Count;
      for (int i = 0; i < numberOfProperties; ++i)
      {
        var propertyName = info.GetValueForHandle<string>();
        _propertyValues[propertyName].DeserializeFromFlatStructure (info);
      }
    }

    void IFlattenedSerializable.SerializeIntoFlatStructure (FlattenedSerializationInfo info)
    {
      info.AddHandle (_id);
      info.AddValue (_timestamp);
      info.AddBoolValue (_isDiscarded);
      if (!_isDiscarded)
      {
        foreach (PropertyValue propertyValue in _propertyValues)
        {
          info.AddHandle (propertyValue.Name);
          propertyValue.SerializeIntoFlatStructure (info);
        }
      }

      info.AddHandle (_clientTransaction);
      info.AddIntValue ((int) _state);
      info.AddHandle (_domainObject);
      info.AddBoolValue(_hasBeenMarkedChanged);
      info.AddBoolValue(_hasBeenChanged);
    }

    #endregion Serialization

    #region Obsolete
    [Obsolete ("This method is obsolete. Use Clone (ObjectID id) instead. (1.13.39)", true)]
    public static DataContainer CreateAndCopyState (ObjectID id, DataContainer stateSource)
    {
      throw new NotImplementedException ();
    }
    #endregion
  }
}