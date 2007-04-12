using System;
using Rubicon.Data.DomainObjects.DataManagement;
using Rubicon.Data.DomainObjects.Mapping;
using Rubicon.Utilities;
using System.Reflection;
using Rubicon.Logging;
using Rubicon.Data.DomainObjects.Configuration;
using Rubicon.Data.DomainObjects.Infrastructure;

namespace Rubicon.Data.DomainObjects
{
/// <summary>
/// Base class for all objects that are persisted by the framework.
/// </summary>
[Serializable]
public class DomainObject
{
  // types

  // static members and constants

  #region Creation factory methods
  /// <summary>
  /// Creates a new instance of a domain object for the current transaction.
  /// </summary>
  /// <typeparam name="T">The type to be supported by the object.</typeparam>
  /// <returns>A new domain object instance.</returns>
  /// <remarks>
  /// <para>This method supports two methods of object creation: legacy and via factory, determined via <see cref="FactoryInstantiatedAttribute"/>
  /// and <see cref="FactoryInstantiationScope"/>. Legacy objects are created by simply invoking their default constructor.
  /// Objects created by the factory are not directly instantiated; instead a proxy is dynamically created that 
  /// intercepts certain method calls in order to perform management tasks. Factory-created objects need to have a constructor taking a
  /// <see cref="ClientTransaction"/> and an <see cref="ObjectID"/> argument, and they automatically support the new property syntax.</para>
  /// </remarks>
  /// <exception cref="ArgumentException">The type <typeparamref name="T"/> is sealed or contains abstract methods (apart from automatic
  /// properties).</exception>
  /// <exception cref="MissingMethodException">The given type <typeparamref name="T"/> does not implement the required public or protected constructor
  /// (see Remarks section).
  /// </exception>
  /// <exception cref="Exception">Any exception thrown by the constructor is propagated to the caller. (Note that this is different to
  /// <see cref="DomainObjectFactory.Create"/>, where a <see cref="TargetInvocationException"/>  is thrown. For this method, the
  /// <see cref="TargetInvocationException"/> is logged and its inner exception is rethrown.)</exception>
  protected static T Create<T> () where T : DomainObject
  {
    return (T) GetCreator (typeof (T)).CreateWithCurrentTransaction (typeof (T));
  }

  /// <summary>
  /// Creates a new instance of a domain object.
  /// </summary>
  /// <typeparam name="T">The type to be supported by the object.</typeparam>
  /// <param name="tx">The client transaction to be passed to the domain object's constructor.</param>
  /// <returns>A new domain object instance.</returns>
  /// <remarks>
  /// <para>This method supports two methods of object creation: legacy and via factory, determined via <see cref="FactoryInstantiatedAttribute"/>
  /// and <see cref="FactoryInstantiationScope"/>. Legacy objects are created by simply invoking their constructor taking a single
  /// <see cref="ClientTransaction"/> parameter. Objects created by the factory are not directly instantiated; instead a subclass is dynamically created that 
  /// intercepts certain method calls in order to perform management tasks. Factory-created objects need to have a constructor taking a
  /// <see cref="ClientTransaction"/> and an <see cref="ObjectID"/> argument, and they  and automatically support the new property syntax.</para>
  /// </remarks>
  /// <exception cref="ArgumentNullException">The <paramref name="clientTransaction"/> parameter is <see langword="null"/>.</exception>
  /// <exception cref="ArgumentException">The type <typeparamref name="T"/> is sealed or contains abstract methods (apart from automatic
  /// properties).</exception>
  /// <exception cref="MissingMethodException">The given type <typeparamref name="T"/> does not implement the required public or protected constructor
  /// (see Remarks section).</exception>
  /// <exception cref="Exception">Any exception thrown by the constructor is propagated to the caller. (Note that this is different to
  /// <see cref="DomainObjectFactory.Create"/>, where a <see cref="TargetInvocationException"/>  is thrown. For this method, the
  /// <see cref="TargetInvocationException"/> is logged and its inner exception is rethrown.)</exception>
  protected static T Create<T> (ClientTransaction tx) where T : DomainObject
  {
    ArgumentUtility.CheckNotNull ("tx", tx);
    return (T) GetCreator (typeof (T)).CreateWithTransaction (typeof(T), tx);
  }

  /// <summary>
  /// Creates a <see cref="DomainObject"/> from a given data container.
  /// </summary>
  /// <param name="dataContainer">The data container for the new domain object.</param>
  /// <returns>A new <see cref="DomainObject"/> for the given data container.</returns>
  /// <remarks>
  /// <para>This method is used by the <see cref="DataContainer"/> class when it is asked to load an object.</para>
  /// <para>It supports two methods of object creation: legacy and via factory, determined via <see cref="FactoryInstantiatedAttribute"/>
  /// and <see cref="FactoryInstantiationScope"/>. Legacy objects are created by simply invoking their constructor taking a single
  /// <see cref="DataContainer"/> parameter. Objects created by the factory are not directly instantiated; instead a subclass is dynamically created that 
  /// intercepts certain method calls in order to perform management tasks. Factory-created objects need to have a constructor taking a
  /// <see cref="ClientTransaction"/> and an <see cref="ObjectID"/> argument, and they automatically support the new property syntax.</para>
  /// </remarks>
  /// <exception cref="ArgumentNullException">The <paramref name="dataContainer"/> parameter is <see langword="null"/>.</exception>
  /// <exception cref="MissingMethodException">The instantiated type does not implement the required public or protected constructor
  /// (see Remarks section).</exception>
  /// <exception cref="Exception">Any exception thrown by the constructor is propagated to the caller. (Note that this is different to
  /// <see cref="DomainObjectFactory.Create"/>, where a <see cref="TargetInvocationException"/>  is thrown. For this method, the
  /// <see cref="TargetInvocationException"/> is logged and its inner exception is rethrown.)</exception>
  internal static DomainObject CreateWithDataContainer (DataContainer dataContainer)
  {
    ArgumentUtility.CheckNotNull ("dataContainer", dataContainer);
    return GetCreator (dataContainer.DomainObjectType).CreateWithDataContainer (dataContainer, dataContainer.ID);
  }

  #endregion

  #region GetObject factory methods
  /// <summary>
  /// Gets a <see cref="DomainObject"/> that is already loaded or attempts to load it from the datasource.
  /// </summary>
  /// <param name="id">The <see cref="ObjectID"/> of the <see cref="DomainObject"/> that should be loaded. Must not be <see langword="null"/>.</param>
  /// <typeparam name="T">The expected type of the concrete <see cref="DomainObject"/></typeparam>
  /// <returns>The <see cref="DomainObject"/> with the specified <paramref name="id"/>.</returns>
  /// <exception cref="System.ArgumentNullException"><paramref name="id"/> is <see langword="null"/>.</exception>
  /// <exception cref="Persistence.StorageProviderException">
  ///   The Mapping does not contain a class definition for the given <paramref name="id"/>.<br /> -or- <br />
  ///   An error occurred while reading a <see cref="PropertyValue"/>.<br /> -or- <br />
  ///   An error occurred while accessing the datasource.
  /// </exception>
  /// <exception cref="MissingMethodException">The concrete <see cref="DomainObject"/> doesn't implement the required constructor.</exception>
  /// <exception cref="InvalidCastException">The loaded <see cref="DomainObject"/> is not of the expected type <typeparamref name="T"/>.</exception>
  public static T GetObject<T> (ObjectID id) where T : DomainObject
  {
    return GetObject<T> (id, false);
  }

  /// <summary>
  /// Gets a <see cref="DomainObject"/> that is already loaded or attempts to load it from the datasource.
  /// </summary>
  /// <param name="id">The <see cref="ObjectID"/> of the <see cref="DomainObject"/> that should be loaded. Must not be <see langword="null"/>.</param>
  /// <param name="includeDeleted">Indicates if the method should return <see cref="DomainObject"/>s that are already deleted.</param>
  /// <typeparam name="T">The expected type of the concrete <see cref="DomainObject"/></typeparam>
  /// <returns>The <see cref="DomainObject"/> with the specified <paramref name="id"/>.</returns>
  /// <exception cref="System.ArgumentNullException"><paramref name="id"/> is <see langword="null"/>.</exception>
  /// <exception cref="Persistence.StorageProviderException">
  ///   The Mapping does not contain a class definition for the given <paramref name="id"/>.<br /> -or- <br />
  ///   An error occurred while reading a <see cref="PropertyValue"/>.<br /> -or- <br />
  ///   An error occurred while accessing the datasource.
  /// </exception>
  /// <exception cref="MissingMethodException">The concrete <see cref="DomainObject"/> doesn't implement the required constructor.</exception>
  /// <exception cref="InvalidCastException">The loaded <see cref="DomainObject"/> is not of the expected type <typeparamref name="T"/>.</exception>
  public static T GetObject<T> (ObjectID id, bool includeDeleted) where T : DomainObject
  {
    return GetObject<T> (id, ClientTransaction.Current, includeDeleted);
  }

  /// <summary>
  /// Gets a <see cref="DomainObject"/> that is already loaded or attempts to load it from the datasource.
  /// </summary>
  /// <param name="id">The <see cref="ObjectID"/> of the <see cref="DomainObject"/> that is loaded. Must not be <see langword="null"/>.</param>
  /// <param name="clientTransaction">The <see cref="Rubicon.Data.DomainObjects.ClientTransaction"/> that is used to load the <see cref="DomainObject"/>.</param>
  /// <typeparam name="T">The expected type of the concrete <see cref="DomainObject"/></typeparam>
  /// <returns>The <see cref="DomainObject"/> with the specified <paramref name="id"/>.</returns>
  /// <exception cref="System.ArgumentNullException"><paramref name="id"/> or <paramref name="clientTransaction"/>is <see langword="null"/>.</exception>
  /// <exception cref="Persistence.StorageProviderException">
  ///   The Mapping does not contain a class definition for the given <paramref name="id"/>.<br /> -or- <br />
  ///   An error occurred while reading a <see cref="PropertyValue"/>.<br /> -or- <br />
  ///   An error occurred while accessing the datasource.
  /// </exception>
  /// <exception cref="MissingMethodException">The concrete <see cref="DomainObject"/> doesn't implement the required constructor.</exception>
  /// <exception cref="InvalidCastException">The loaded <see cref="DomainObject"/> is not of the expected type <typeparamref name="T"/>.</exception>
  public static T GetObject<T> (ObjectID id, ClientTransaction clientTransaction) where T : DomainObject
  {
    return GetObject<T> (id, clientTransaction, false);
  }

  /// <summary>
  /// Gets a <see cref="DomainObject"/> that is already loaded or attempts to load it from the datasource.
  /// </summary>
  /// <param name="id">The <see cref="ObjectID"/> of the <see cref="DomainObject"/> that is loaded. Must not be <see langword="null"/>.</param>
  /// <param name="clientTransaction">The <see cref="Rubicon.Data.DomainObjects.ClientTransaction"/> that us used to load the <see cref="DomainObject"/>.</param>
  /// <param name="includeDeleted">Indicates if the method should return <see cref="DomainObject"/>s that are already deleted.</param>
  /// <typeparam name="T">The expected type of the concrete <see cref="DomainObject"/></typeparam>
  /// <returns>The <see cref="DomainObject"/> with the specified <paramref name="id"/>.</returns>
  /// <exception cref="System.ArgumentNullException"><paramref name="id"/> or <paramref name="clientTransaction"/>is <see langword="null"/>.</exception>
  /// <exception cref="Persistence.StorageProviderException">
  ///   The Mapping does not contain a class definition for the given <paramref name="id"/>.<br /> -or- <br />
  ///   An error occurred while reading a <see cref="PropertyValue"/>.<br /> -or- <br />
  ///   An error occurred while accessing the datasource.
  /// </exception>
  /// <exception cref="MissingMethodException">The concrete <see cref="DomainObject"/> doesn't implement the required constructor.</exception>
  /// <exception cref="InvalidCastException">The loaded <see cref="DomainObject"/> is not of the expected type <typeparamref name="T"/>.</exception>
  public static T GetObject<T> (ObjectID id, ClientTransaction clientTransaction, bool includeDeleted) where T : DomainObject
  {
    ArgumentUtility.CheckNotNull ("id", id);
    ArgumentUtility.CheckNotNull ("clientTransaction", clientTransaction);

    return (T) clientTransaction.GetObject (id, includeDeleted);
  }

  #endregion

  #region Obsolete GetObject legacy methods

  /// <summary>
  /// Gets a <see cref="DomainObject"/> that is already loaded or attempts to load it from the datasource.
  /// </summary>
  /// <param name="id">The <see cref="ObjectID"/> of the <see cref="DomainObject"/> that should be loaded. Must not be <see langword="null"/>.</param>
  /// <returns>The <see cref="DomainObject"/> with the specified <paramref name="id"/>.</returns>
  /// <exception cref="System.ArgumentNullException"><paramref name="id"/> is <see langword="null"/>.</exception>
  /// <exception cref="Persistence.StorageProviderException">
  ///   The Mapping does not contain a class definition for the given <paramref name="id"/>.<br /> -or- <br />
  ///   An error occurred while reading a <see cref="PropertyValue"/>.<br /> -or- <br />
  ///   An error occurred while accessing the datasource.
  /// </exception>
  /// <exception cref="MissingMethodException">The concrete <see cref="DomainObject"/> doesn't implement the required constructor.</exception>
  // TODO: [Obsolete ("This method is obsolete, use the generic variant instead.")]
  protected static DomainObject GetObject (ObjectID id)
  {
    return GetObject<DomainObject> (id);
  }

  /// <summary>
  /// Gets a <see cref="DomainObject"/> that is already loaded or attempts to load it from the datasource.
  /// </summary>
  /// <param name="id">The <see cref="ObjectID"/> of the <see cref="DomainObject"/> that should be loaded. Must not be <see langword="null"/>.</param>
  /// <param name="includeDeleted">Indicates if the method should return <see cref="DomainObject"/>s that are already deleted.</param>
  /// <returns>The <see cref="DomainObject"/> with the specified <paramref name="id"/>.</returns>
  /// <exception cref="System.ArgumentNullException"><paramref name="id"/> is <see langword="null"/>.</exception>
  /// <exception cref="Persistence.StorageProviderException">
  ///   The Mapping does not contain a class definition for the given <paramref name="id"/>.<br /> -or- <br />
  ///   An error occurred while reading a <see cref="PropertyValue"/>.<br /> -or- <br />
  ///   An error occurred while accessing the datasource.
  /// </exception>
  /// <exception cref="MissingMethodException">The concrete <see cref="DomainObject"/> doesn't implement the required constructor.</exception>
  // TODO: [Obsolete ("This method is obsolete, use the generic variant instead.")]
  protected static DomainObject GetObject (ObjectID id, bool includeDeleted)
  {
    return GetObject<DomainObject> (id, includeDeleted);
  }

  /// <summary>
  /// Gets a <see cref="DomainObject"/> that is already loaded or attempts to load it from the datasource.
  /// </summary>
  /// <param name="id">The <see cref="ObjectID"/> of the <see cref="DomainObject"/> that is loaded. Must not be <see langword="null"/>.</param>
  /// <param name="clientTransaction">The <see cref="Rubicon.Data.DomainObjects.ClientTransaction"/> that is used to load the <see cref="DomainObject"/>.</param>
  /// <returns>The <see cref="DomainObject"/> with the specified <paramref name="id"/>.</returns>
  /// <exception cref="System.ArgumentNullException"><paramref name="id"/> or <paramref name="clientTransaction"/>is <see langword="null"/>.</exception>
  /// <exception cref="Persistence.StorageProviderException">
  ///   The Mapping does not contain a class definition for the given <paramref name="id"/>.<br /> -or- <br />
  ///   An error occurred while reading a <see cref="PropertyValue"/>.<br /> -or- <br />
  ///   An error occurred while accessing the datasource.
  /// </exception>
  /// <exception cref="MissingMethodException">The concrete <see cref="DomainObject"/> doesn't implement the required constructor.</exception>
  // TODO: [Obsolete ("This method is obsolete, use the generic variant instead.")]
  protected static DomainObject GetObject (ObjectID id, ClientTransaction clientTransaction)
  {
    return GetObject<DomainObject> (id, clientTransaction);
  }

  /// <summary>
  /// Gets a <see cref="DomainObject"/> that is already loaded or attempts to load it from the datasource.
  /// </summary>
  /// <param name="id">The <see cref="ObjectID"/> of the <see cref="DomainObject"/> that is loaded. Must not be <see langword="null"/>.</param>
  /// <param name="clientTransaction">The <see cref="Rubicon.Data.DomainObjects.ClientTransaction"/> that us used to load the <see cref="DomainObject"/>.</param>
  /// <param name="includeDeleted">Indicates if the method should return <see cref="DomainObject"/>s that are already deleted.</param>
  /// <returns>The <see cref="DomainObject"/> with the specified <paramref name="id"/>.</returns>
  /// <exception cref="System.ArgumentNullException"><paramref name="id"/> or <paramref name="clientTransaction"/>is <see langword="null"/>.</exception>
  /// <exception cref="Persistence.StorageProviderException">
  ///   The Mapping does not contain a class definition for the given <paramref name="id"/>.<br /> -or- <br />
  ///   An error occurred while reading a <see cref="PropertyValue"/>.<br /> -or- <br />
  ///   An error occurred while accessing the datasource.
  /// </exception>
  /// <exception cref="MissingMethodException">The concrete <see cref="DomainObject"/> doesn't implement the required constructor.</exception>
  // TODO: [Obsolete ("This method is obsolete, use the generic variant instead.")]
  protected static DomainObject GetObject (ObjectID id, ClientTransaction clientTransaction, bool includeDeleted)
  {
    return GetObject<DomainObject> (id, clientTransaction, includeDeleted);
  }

  #endregion

  // TODO: Change to use mapping instead of attribute later.
  public static bool ShouldUseFactoryForInstantiation (Type domainObjectType)
  {
    if (FactoryInstantiationScope.WithinScope)
      return true;
    
    ClassDefinition classDefinition = MappingConfiguration.Current.ClassDefinitions.GetMandatory (domainObjectType);
    return classDefinition is ReflectionBasedClassDefinition;
  }

  private static IDomainObjectCreator GetCreator (Type domainObjectType)
  {
    if (ShouldUseFactoryForInstantiation (domainObjectType))
      return NewStyleDomainObjectCreator.Instance;
    else
      return LegacyDomainObjectCreator.Instance;
  }

  // member fields

  /// <summary>
  /// Occurs before a <see cref="PropertyValue"/> of the <see cref="DomainObject"/> is changed.
  /// </summary>
  /// <remarks>
  /// This event does not fire when a <see cref="PropertyValue"/> has been changed due to a relation change.
  /// </remarks>
  /// <include file='Doc\include\DomainObjects.xml' path='documentation/allEvents/remarks'/>
  public event PropertyChangeEventHandler PropertyChanging;

  /// <summary>
  /// Occurs after a <see cref="PropertyValue"/> of the <see cref="DomainObject"/> is changed.
  /// </summary>
  /// <remarks>
  /// This event does not fire when a <see cref="PropertyValue"/> has been changed due to a relation change.
  /// </remarks>
  /// <include file='Doc\include\DomainObjects.xml' path='documentation/allEvents/remarks'/>
  public event PropertyChangeEventHandler PropertyChanged;

  /// <summary>
  /// Occurs before a Relation of the <see cref="DomainObject"/> is changed.
  /// </summary>
  /// <include file='Doc\include\DomainObjects.xml' path='documentation/allEvents/remarks'/>
  public event RelationChangingEventHandler RelationChanging;

  /// <summary>
  /// Occurs after a Relation of the <see cref="DomainObject"/> has been changed.
  /// </summary>
  /// <include file='Doc\include\DomainObjects.xml' path='documentation/allEvents/remarks'/>
  public event RelationChangedEventHandler RelationChanged;

  /// <summary>
  /// Occurs before the <see cref="DomainObject"/> is deleted.
  /// </summary>
  /// <include file='Doc\include\DomainObjects.xml' path='documentation/allEvents/remarks'/>
  public event EventHandler Deleting;

  /// <summary>
  /// Occurs before the changes of a <see cref="DomainObject"/> are committed.
  /// </summary>
  /// <include file='Doc\include\DomainObjects.xml' path='documentation/allEvents/remarks'/>
  public event EventHandler Committing;

  /// <summary>
  /// Occurs after the changes of a <see cref="DomainObject"/> are successfully committed.
  /// </summary>
  /// <include file='Doc\include\DomainObjects.xml' path='documentation/allEvents/remarks'/>
  public event EventHandler Committed;

  /// <summary>
  /// Occurs before the changes of a <see cref="DomainObject"/> are rolled back.
  /// </summary>
  /// <include file='Doc\include\DomainObjects.xml' path='documentation/allEvents/remarks'/>
  public event EventHandler RollingBack;

  /// <summary>
  /// Occurs after the changes of a <see cref="DomainObject"/> are successfully rolled back.
  /// </summary>
  /// <include file='Doc\include\DomainObjects.xml' path='documentation/allEvents/remarks'/>
  public event EventHandler RolledBack;

  /// <summary>
  /// Occurs after the <see cref="DomainObject"/> has been deleted.
  /// </summary>
  /// <include file='Doc\include\DomainObjects.xml' path='documentation/allEvents/remarks'/>
  public event EventHandler Deleted;

  private DataContainer _dataContainer;

  // construction and disposing

  /// <summary>
  /// Initializes a new <see cref="DomainObject"/>.
  /// </summary>
  /// <param name="clientTransaction">The <see cref="Rubicon.Data.DomainObjects.ClientTransaction"/> the <see cref="DomainObject"/> should be part of.
  /// Must not be <see langword="null"/>, unless an <paramref name="id"/> is given.</param>
  /// <param name="id">The <see cref="ObjectID"/> of the object. If this is a <see langword="null"/>, the object is considered to be newly created.
  /// If it has a value, the constructor assumes that it is invoked as part of a loading process.</param>
  /// <remarks>Implement a protected constructor with the same signature on concrete domain objects, delegate to this base constructor, and only use
  /// the <see cref="Create"/> and <see cref="GetObject"/> factory methods to invoke it. Domain objects generally should not be constructed via the
  /// <c>new</c> operator.</remarks>
  /// <exception cref="ArgumentNullException">Both the <paramref name="clientTransaction"/> and <paramref name="id"/> arguments are <see langword="null"/>.</exception>
  protected DomainObject (ClientTransaction clientTransaction, ObjectID id)
  {
    if (clientTransaction == null && id == null)
    {
      throw new ArgumentNullException ("clientTransaction, id", "One of the arguments must be non-null.");
    }
    if (id != null)
    {
      // assume loading, _dataContainer is set by the NewStyleDomainObjectCreator
    }
    else
    {
      // assume creation
      clientTransaction.NewObjectCreating (GetPublicDomainObjectType ());

      _dataContainer = clientTransaction.CreateNewDataContainer (GetPublicDomainObjectType ());
      _dataContainer.SetDomainObject (this);
    }
  }

  #region Obsolete legacy constructors
  /// <summary>
  /// Initializes a new <see cref="DomainObject"/>.
  /// </summary>
  // TODO: [Obsolete("This constructor is obsolete, use the DomainObject (ClientTransaction, ObjectID) one instead.")]
  protected DomainObject () : this (ClientTransaction.Current)
  {
  }

  /// <summary>
  /// Initializes a new <see cref="DomainObject"/>.
  /// </summary>
  /// <param name="clientTransaction">The <see cref="Rubicon.Data.DomainObjects.ClientTransaction"/> the <see cref="DomainObject"/> should be part of. Must not be <see langword="null"/>.</param>
  /// <exception cref="System.ArgumentNullException"><paramref name="clientTransaction"/> is <see langword="null"/>.</exception>
  // TODO: [Obsolete ("This constructor is obsolete, use the DomainObject (ClientTransaction, ObjectID) one instead.")]
  protected DomainObject (ClientTransaction clientTransaction)
  {
    ArgumentUtility.CheckNotNull ("clientTransaction", clientTransaction);

    clientTransaction.NewObjectCreating (GetPublicDomainObjectType ());

    _dataContainer = clientTransaction.CreateNewDataContainer (GetPublicDomainObjectType ());
    _dataContainer.SetDomainObject (this);
  }


  /// <summary>
  /// Infrastructure constructor necessary to load a <see cref="DomainObject"/> from a datasource.
  /// </summary>
  /// <remarks>
  /// All derived classes have to implement an (empty) constructor with this signature.
  /// Do not implement any initialization logic in this constructor, but use <see cref="DomainObject.OnLoaded"/> instead.
  /// </remarks>
  /// <param name="dataContainer">The newly loaded <b>DataContainer</b></param>
  // TODO: [Obsolete ("This constructor is obsolete, use the DomainObject (ClientTransaction, ObjectID) one instead.")]
  protected DomainObject (DataContainer dataContainer)
  {
    ArgumentUtility.CheckNotNull ("dataContainer", dataContainer);

    _dataContainer = dataContainer;
  }

  #endregion

  // methods and properties

  /// <summary>
  /// Returns the public type representation of this domain object, i.e. the type object visible to mappings, database, etc.
  /// </summary>
  /// <returns>The public type representation of this domain object.</returns>
  /// <remarks>A domain object should override this method if it wants to impersonate one of its base types. The framework will handle this object
  /// as if it was of the type returned by this method and ignore its actual type.</remarks>
  public virtual Type GetPublicDomainObjectType ()
  {
    return this.GetType ();
  }

  /// <summary>
  /// Gets the <see cref="ObjectID"/> of the <see cref="DomainObject"/>.
  /// </summary>
  /// <exception cref="DataManagement.ObjectDiscardedException">The object is already discarded. See <see cref="DataManagement.ObjectDiscardedException"/> for further information.</exception>
  public ObjectID ID
  {
    get 
    {
      CheckIfObjectIsDiscarded ();
      return _dataContainer.ID; 
    }
  }

  /// <summary>
  /// Gets the current state of the <see cref="DomainObject"/>.
  /// </summary>
  /// <exception cref="DataManagement.ObjectDiscardedException">The object is already discarded. See <see cref="DataManagement.ObjectDiscardedException"/> for further information.</exception>
  public StateType State
  {
    get
    {
      CheckIfObjectIsDiscarded ();
      if (_dataContainer.State == StateType.Unchanged)
      {
        if (ClientTransaction.HasRelationChanged (this))
          return StateType.Changed;
        else
          return StateType.Unchanged;
      }

      return _dataContainer.State;
    }
  }

  /// <summary>
  /// Gets a value indicating the discarded status of the object.
  /// </summary>
  /// <remarks>
  /// For more information why and when an object is discarded see <see cref="Rubicon.Data.DomainObjects.DataManagement.ObjectDiscardedException"/>.
  /// </remarks>
  public bool IsDiscarded
  {
    get { return _dataContainer.IsDiscarded; }
  }

  /// <summary>
  /// Gets the <see cref="Rubicon.Data.DomainObjects.ClientTransaction"/> to which the <see cref="DomainObject"/> belongs.
  /// </summary>
  public ClientTransaction ClientTransaction
  {
    get { return _dataContainer.ClientTransaction; }
  }

  /// <summary>
  /// Gets the <see cref="DataContainer"/> of the <see cref="DomainObject"/>.
  /// </summary>
  /// <exception cref="DataManagement.ObjectDiscardedException">The object is already discarded. See <see cref="DataManagement.ObjectDiscardedException"/> for further information.</exception>
  protected internal DataContainer DataContainer
  {
    get 
    {
      CheckIfObjectIsDiscarded ();
      return _dataContainer; 
    }
    internal set
    {
      if (_dataContainer != null)
      {
        throw new InvalidOperationException ("The data container can only be set once.");
      }
      _dataContainer = value;
    }
  }

  /// <summary>
  /// Deletes the <see cref="DomainObject"/>.
  /// </summary>
  /// <exception cref="DataManagement.ObjectDiscardedException">The object is already discarded. See <see cref="DataManagement.ObjectDiscardedException"/> for further information.</exception>
  /// <remarks>To perform custom actions when a <see cref="DomainObject"/> is deleted <see cref="OnDeleting"/> and <see cref="OnDeleted"/> should be overridden.</remarks>
  public void Delete ()
  {
    CheckIfObjectIsDiscarded ();

    ClientTransaction.Delete (this);
  }

  #region New-style property access implementation

  /// <summary>
  /// Prepares access to the <see cref="PropertyValue"/> of the given name.
  /// </summary>
  /// <param name="propertyName">The name of the <see cref="PropertyValue"/> to be accessed.</param>
  /// <remarks>This method prepares the given property for access via <see cref="GetPropertyValue"/> and <see cref="SetPropertyValue"/>.
  /// It is automatically invoked for virtual properties in domain objects created with the <see cref="DomainObjectFactory"/> and thus doesn't
  /// have to be called manually for these objects. If you choose to invoke <see cref="PreparePropertyAccess"/> and
  /// <see cref="PropertyAccessFinished"/> yourself, be sure to finish the property access with exactly one call to 
  /// <see cref="PropertyAccessFinished"/> from a finally-block.</remarks>
  /// <exception cref="System.ArgumentNullException"><paramref name="propertyName"/> is <see langword="null"/>.</exception>
  /// <exception cref="Rubicon.Utilities.ArgumentEmptyException"><paramref name="propertyName"/> is an empty string.</exception>
  protected internal virtual void PreparePropertyAccess (string propertyName)
  {
    ArgumentUtility.CheckNotNullOrEmpty ("propertyName", propertyName);
    CurrentPropertyManager.PreparePropertyAccess (propertyName);
  }

  /// <summary>
  /// Indicates that access to the <see cref="PropertyValue"/> of the given name is finished.
  /// </summary>
  /// <remarks>This method must be executed after a property previously prepared via <see cref="PreparePropertyAccess"/> has been accessed as needed.
  /// It is automatically invoked for virtual properties in domain objects created with the <see cref="DomainObjectFactory"/> and thus doesn't
  /// have to be called manually for these objects. If you choose to invoke <see cref="PreparePropertyAccess"/> and
  /// <see cref="PropertyAccessFinished"/> yourself, be sure to invoke this method in a finally-block in order to guarantee its execution.</remarks>
  /// <exception cref="InvalidOperationException">There is no property to be finished. There is likely a mismatched number of calls to
  /// <see cref="PreparePropertyAccess"/> and <see cref="PropertyAccessFinished"/>.</exception>
  protected internal virtual void PropertyAccessFinished ()
  {
    CurrentPropertyManager.PropertyAccessFinished ();
  }

  /// <summary>
  /// Retrieves the current property name and throws an exception if there is no current property.
  /// </summary>
  /// <returns>The current property name.</returns>
  /// <remarks>Retrieves the current property name previously initialized via <see cref="PreparePropertyAccess"/>. Domain objects created with the
  /// <see cref="DomainObjectFactory"/> automatically initialize their virtual properties without needing any further work.</remarks>
  /// <exception cref="InvalidOperationException">There is no current property or it hasn't been properly initialized.</exception>
  protected internal virtual string GetAndCheckCurrentPropertyName ()
  {
    string propertyName = CurrentPropertyManager.CurrentPropertyName;
    if (propertyName == null)
    {
      throw new InvalidOperationException ("There is no current property or it hasn't been properly initialized.");
    }
    else
    {
      return propertyName;
    }
  }

  #endregion

  #region Get/SetPropertyValue
  /// <summary>
  /// Gets the value of the current <see cref="PropertyValue"/>.
  /// </summary>
  /// <typeparam name="T">The expected type of the property value.</typeparam>
  /// <returns>The value of the current property.</returns>
  /// <remarks>The current property must previously have been initialized with <see cref="PreparePropertyAccess" /> and should be de-initialized
  /// with <see cref="PropertyAccessFinished"/> when its value has been retrieved. Domain objects created with the <see cref="DomainObjectFactory"/>
  /// automatically initialize and de-initialize their virtual properties without needing any further work.</remarks>
  /// <exception cref="InvalidOperationException">The current property hasn't been initialized or there is no current property. Perhaps the domain 
  /// object was created by the <c>new</c> operator instead of the factory or the property is not virtual.</exception>
  /// <exception cref="ArgumentException">The current property was initialized with an invalid name. This will only occur if the current property
  /// has been explicitly initialized with <see cref="PreparePropertyAccess"/>; it should not occur if the initialization was performed automatically.
  /// </exception>
  /// <exception cref="InvalidTypeException">The current property's value does not match the requested type.</exception>
  /// <exception cref="DataManagement.ObjectDiscardedException">The object is already discarded. See <see cref="DataManagement.ObjectDiscardedException"/> for further information.</exception>
  protected internal virtual T GetPropertyValue<T> ()
  {
    string currentPropertyName = GetAndCheckCurrentPropertyName ();
    return GetPropertyValue<T> (currentPropertyName);
  }

  /// <summary>
  /// Sets the value of the current <see cref="PropertyValue"/>.
  /// </summary>
  /// <param name="value">The value the current property is to be set to.</param>
  /// <remarks>The current property must have previously been initialized with <see cref="PreparePropertyAccess" /> and should be de-initialized
  /// with <see cref="PropertyAccessFinished"/> when its value has been set. Domain objects created with the <see cref="DomainObjectFactory"/>
  /// automatically initialize and de-initialize their virtual properties without needing any further work.</remarks>
  /// <exception cref="InvalidOperationException">The current property hasn't been initialized or there is no current property. Perhaps the domain 
  /// object was created by the <c>new</c> operator instead of the factory or the property is not virtual.</exception>
  /// <exception cref="ArgumentException">The current property was initialized with an invalid name. This will only occur if the current property
  /// has been explicitly initialized with <see cref="PreparePropertyAccess"/>; it should not occur if the initialization was performed automatically.
  /// </exception>
  /// <exception cref="DataManagement.ObjectDiscardedException">The object is already discarded. See <see cref="DataManagement.ObjectDiscardedException"/> for further information.</exception>
  protected internal virtual void SetPropertyValue (object value)
  {
    string currentPropertyName = GetAndCheckCurrentPropertyName ();
    SetPropertyValue (currentPropertyName, value);
  }

  /// <summary>
  /// Gets the value of the given <see cref="PropertyValue"/>.
  /// </summary>
  /// <typeparam name="T">The expected type of the property value.</typeparam>
  /// <param name="propertyName">The name of the property to get.</param>
  /// <returns>The value of the given property.</returns>
  /// <exception cref="System.ArgumentNullException"><paramref name="propertyName"/> is <see langword="null"/>.</exception>
  /// <exception cref="Rubicon.Utilities.ArgumentEmptyException"><paramref name="propertyName"/> is an empty string.</exception>
  /// <exception cref="System.ArgumentException">The given <paramref name="propertyName"/> does not exist in the data container.</exception>
  /// <exception cref="InvalidTypeException">The current property's value does not match the requested type.</exception>
  /// <exception cref="DataManagement.ObjectDiscardedException">The object is already discarded. See <see cref="DataManagement.ObjectDiscardedException"/> for further information.</exception>
  protected internal virtual T GetPropertyValue<T> (string propertyName)
  {
    ArgumentUtility.CheckNotNullOrEmpty ("propertyName", propertyName);
    object value = DataContainer.GetValue(propertyName);

    Assertion.DebugAssert (
        !(value == null && typeof (T).IsValueType), "Property '{0}' is a value type but the DataContainer returned null.", propertyName);

    if (value == null || value is T)
      return (T) value;

    throw new InvalidTypeException (propertyName, typeof (T), value.GetType ());    
  }

  /// <summary>
  /// Sets the value of the given <see cref="PropertyValue"/>.
  /// </summary>
  /// <param name="propertyName">The name of the property whose value is to be set.</param>
  /// <param name="value">The value the property is to be set to.</param>
  /// <exception cref="System.ArgumentNullException"><paramref name="propertyName"/> is <see langword="null"/>.</exception>
  /// <exception cref="Rubicon.Utilities.ArgumentEmptyException"><paramref name="propertyName"/> is an empty string.</exception>
  /// <exception cref="System.ArgumentException">The given <paramref name="propertyName"/> does not exist in the data container.</exception>
  /// <exception cref="DataManagement.ObjectDiscardedException">The object is already discarded. See <see cref="DataManagement.ObjectDiscardedException"/> for further information.</exception>
  protected internal virtual void SetPropertyValue (string propertyName, object value)
  {
    ArgumentUtility.CheckNotNullOrEmpty ("propertyName", propertyName);
    DataContainer.SetValue(propertyName, value);
  }

  #endregion

  #region Related objects

  /// <summary>
  /// Gets the related object of a given <paramref name="propertyName"/>.
  /// </summary>
  /// <param name="propertyName">The name of the property referring to the relation. <paramref name="propertyName"/> must refer to a one-to-one or many-to-one relation. Must not be <see langword="null"/>.</param>
  /// <returns>The <see cref="DomainObject"/> that is the current related object.</returns>
  /// <exception cref="System.ArgumentNullException"><paramref name="propertyName"/> is <see langword="null"/>.</exception>
  /// <exception cref="Rubicon.Utilities.ArgumentEmptyException"><paramref name="propertyName"/> is an empty string.</exception>
  /// <exception cref="DataManagement.ObjectDiscardedException">The object is already discarded. See <see cref="DataManagement.ObjectDiscardedException"/> for further information.</exception>
  /// <exception cref="System.ArgumentException"><paramref name="propertyName"/> does not refer to an one-to-one or many-to-one relation.</exception>
  protected internal virtual DomainObject GetRelatedObject (string propertyName)
  {
    ArgumentUtility.CheckNotNullOrEmpty ("propertyName", propertyName);
    CheckIfObjectIsDiscarded ();

    return ClientTransaction.GetRelatedObject (new RelationEndPointID (ID, propertyName));
  }

  /// <summary>
  /// Gets the related object for the current property.
  /// </summary>
  /// <returns>The <see cref="DomainObject"/> that is the current related object.</returns>
  /// <remarks>The current property must previously have been initialized with <see cref="PreparePropertyAccess" /> and should be de-initialized
  /// with <see cref="PropertyAccessFinished"/> when its value has been retrieved. Domain objects created with the <see cref="DomainObjectFactory"/>
  /// automatically initialize and de-initialize their virtual properties without needing any further work.</remarks>
  /// <exception cref="InvalidOperationException">The current property hasn't been initialized or there is no current property. Perhaps the domain 
  /// object was created by the <c>new</c> operator instead of the factory or the property is not virtual.</exception>
  /// <exception cref="System.ArgumentException"><paramref name="propertyName"/> does not refer to an one-to-one or many-to-one relation.</exception>
  /// <exception cref="ArgumentException">The current property does not refer to a one-to-one or many-to-one relation.</exception>
  /// <exception cref="DataManagement.ObjectDiscardedException">The object is already discarded. See <see cref="DataManagement.ObjectDiscardedException"/> for further information.</exception>
  protected internal virtual DomainObject GetRelatedObject ()
  {
    string currentPropertyName = GetAndCheckCurrentPropertyName ();
    return GetRelatedObject (currentPropertyName);
  }

  /// <summary>
  /// Gets the original related object of a given <paramref name="propertyName"/> at the point of instantiation, loading, commit or rollback.
  /// </summary>
  /// <param name="propertyName">The name of the property referring to the relation. <paramref name="propertyName"/> must refer to a one-to-many relation. Must not be <see langword="null"/>.</param>
  /// <returns>The <see cref="DomainObject"/> that is the current related object.</returns>
  /// <remarks>There is no overload for this method that uses the current property initialized via <see cref="PreparePropertyAccess"/>. The reason
  /// for this is that code accessing original related objects will not typically be called from within a property called like the internal property
  /// name. Making an automatic mechanism for this method would thus only lead to confusion or subtle errors and is therefore not implemented.
  /// </remarks>
  /// <exception cref="System.ArgumentNullException"><paramref name="propertyName"/> is <see langword="null"/>.</exception>
  /// <exception cref="Rubicon.Utilities.ArgumentEmptyException"><paramref name="propertyName"/> is an empty string.</exception>
  /// <exception cref="DataManagement.ObjectDiscardedException">The object is already discarded. See <see cref="DataManagement.ObjectDiscardedException"/> for further information.</exception>
  /// <exception cref="System.InvalidCastException"><paramref name="propertyName"/> does not refer to an <see cref="DataManagement.ObjectEndPoint"/>.</exception>
  /// <exception cref="System.ArgumentException"><paramref name="propertyName"/> does not refer to an one-to-one or many-to-one relation.</exception>
  protected internal virtual DomainObject GetOriginalRelatedObject (string propertyName)
  {
    ArgumentUtility.CheckNotNullOrEmpty ("propertyName", propertyName);
    CheckIfObjectIsDiscarded ();

    return ClientTransaction.GetOriginalRelatedObject (new RelationEndPointID (ID, propertyName));
  }

  /// <summary>
  /// Gets the related objects of a given <paramref name="propertyName"/>.
  /// </summary>
  /// <param name="propertyName">The name of the property referring to the relation. <paramref name="propertyName"/> must refer to a one-to-many relation. Must not be <see langword="null"/>.</param>
  /// <returns>A <see cref="DomainObjectCollection"/> containing the current related objects.</returns>
  /// <exception cref="System.ArgumentNullException"><paramref name="propertyName"/> is <see langword="null"/>.</exception>
  /// <exception cref="Rubicon.Utilities.ArgumentEmptyException"><paramref name="propertyName"/> is an empty string.</exception>
  /// <exception cref="DataManagement.ObjectDiscardedException">The object is already discarded. See <see cref="DataManagement.ObjectDiscardedException"/> for further information.</exception>
  /// <exception cref="System.InvalidCastException"><paramref name="propertyName"/> does not refer to an <see cref="DataManagement.ObjectEndPoint"/>.</exception>
  /// <exception cref="System.ArgumentException"><paramref name="propertyName"/> does not refer to an one-to-many relation.</exception>
  protected internal virtual DomainObjectCollection GetRelatedObjects (string propertyName)
  {
    ArgumentUtility.CheckNotNullOrEmpty ("propertyName", propertyName);
    CheckIfObjectIsDiscarded ();

    return ClientTransaction.GetRelatedObjects (new RelationEndPointID (ID, propertyName));
  }

  /// <summary>
  /// Gets the related objects of the current given property.
  /// </summary>
  /// <returns>A <see cref="DomainObjectCollection"/> containing the current related objects.</returns>
  /// <remarks>The current property must previously have been initialized with <see cref="PreparePropertyAccess" /> and should be de-initialized
  /// with <see cref="PropertyAccessFinished"/> when its value has been retrieved. Domain objects created with the <see cref="DomainObjectFactory"/>
  /// automatically initialize and de-initialize their virtual properties without needing any further work.</remarks>
  /// <exception cref="InvalidOperationException">The current property hasn't been initialized or there is no current property. Perhaps the domain 
  /// object was created by the <c>new</c> operator instead of the factory or the property is not virtual.</exception>
  /// <exception cref="DataManagement.ObjectDiscardedException">The object is already discarded. See <see cref="DataManagement.ObjectDiscardedException"/> for further information.</exception>
  /// <exception cref="System.InvalidCastException">The current property does not refer to an <see cref="DataManagement.ObjectEndPoint"/>.</exception>
  /// <exception cref="System.ArgumentException">The current property does not refer to an one-to-many relation.</exception>
  protected internal virtual DomainObjectCollection GetRelatedObjects ()
  {
    string currentPropertyName = GetAndCheckCurrentPropertyName ();
    return GetRelatedObjects (currentPropertyName);
  }

  /// <summary>
  /// Gets the original related objects of a given <paramref name="propertyName"/> at the point of instantiation, loading, commit or rollback.
  /// </summary>
  /// <param name="propertyName">The name of the property referring to the relation. <paramref name="propertyName"/> must refer to a one-to-many relation. Must not be <see langword="null"/>.</param>
  /// <returns>A <see cref="DomainObjectCollection"/> containing the original related objects.</returns>
  /// <exception cref="System.ArgumentNullException"><paramref name="propertyName"/> is <see langword="null"/>.</exception>
  /// <exception cref="Rubicon.Utilities.ArgumentEmptyException"><paramref name="propertyName"/> is an empty string.</exception>
  /// <exception cref="DataManagement.ObjectDiscardedException">The object is already discarded. See <see cref="DataManagement.ObjectDiscardedException"/> for further information.</exception>
  /// <exception cref="System.ArgumentException"><paramref name="propertyName"/> does not refer to an one-to-many relation.</exception>
  protected internal virtual DomainObjectCollection GetOriginalRelatedObjects (string propertyName)
  {
    ArgumentUtility.CheckNotNullOrEmpty ("propertyName", propertyName);
    CheckIfObjectIsDiscarded ();

    return ClientTransaction.GetOriginalRelatedObjects (new RelationEndPointID (ID, propertyName));
  }

  /// <summary>
  /// Sets a relation to another object.
  /// </summary>
  /// <param name="propertyName">The name of the property referring to the relation, that should relate to <paramref name="newRelatedObject"/>. Must not be <see langword="null"/>.</param>
  /// <param name="newRelatedObject">The new <see cref="DomainObject"/> that should be related; <see langword="null"/> indicates that no object should be referenced.</param>
  /// <exception cref="System.ArgumentNullException"><paramref name="propertyName"/> is <see langword="null"/>.</exception>
  /// <exception cref="Rubicon.Utilities.ArgumentEmptyException"><paramref name="propertyName"/> is an empty string.</exception>
  /// <exception cref="DataManagement.ObjectDiscardedException">The object is already discarded. See <see cref="DataManagement.ObjectDiscardedException"/> for further information.</exception>
  protected internal void SetRelatedObject (string propertyName, DomainObject newRelatedObject)
  {
    ArgumentUtility.CheckNotNullOrEmpty ("propertyName", propertyName);
    CheckIfObjectIsDiscarded ();

    ClientTransaction.SetRelatedObject (new RelationEndPointID (ID, propertyName), newRelatedObject);
  }

  /// <summary>
  /// Sets the current property's relation to another object.
  /// </summary>
  /// <param name="newRelatedObject">The new <see cref="DomainObject"/> that should be related; <see langword="null"/> indicates that no object should be referenced.</param>
  /// <remarks>The current property must previously have been initialized with <see cref="PreparePropertyAccess" /> and should be de-initialized
  /// with <see cref="PropertyAccessFinished"/> when its value has been retrieved. Domain objects created with the <see cref="DomainObjectFactory"/>
  /// automatically initialize and de-initialize their virtual properties without needing any further work.</remarks>
  /// <exception cref="InvalidOperationException">The current property hasn't been initialized or there is no current property. Perhaps the domain 
  /// object was created by the <c>new</c> operator instead of the factory or the property is not virtual.</exception>
  /// <exception cref="System.ArgumentException"><paramref name="propertyName"/> does not refer to an one-to-one or many-to-one relation.</exception>
  /// <exception cref="DataManagement.ObjectDiscardedException">The object is already discarded. See <see cref="DataManagement.ObjectDiscardedException"/> for further information.</exception>
  protected internal void SetRelatedObject (DomainObject newRelatedObject)
  {
    string currentPropertyName = GetAndCheckCurrentPropertyName ();
    SetRelatedObject (currentPropertyName, newRelatedObject);
  }

  #endregion

  /// <summary>
  /// Method is invoked after the loading process of the object is completed.
  /// </summary>
  /// <remarks>
  /// Override this method to initialize <see cref="DomainObject"/>s that are loaded from the datasource.
  /// </remarks>
  protected virtual void OnLoaded ()
  {
  }

  /// <summary>
  /// Raises the <see cref="Committing"/> event.
  /// </summary>
  /// <param name="args">A <see cref="System.EventArgs"/> object that contains the event data.</param>
  protected virtual void OnCommitting (EventArgs args)
  {
    if (Committing != null)
      Committing (this, args);
  }

  /// <summary>
  /// Raises the <see cref="Committed"/> event.
  /// </summary>
  /// <param name="args">A <see cref="System.EventArgs"/> object that contains the event data.</param>
  protected virtual void OnCommitted (EventArgs args)
  {
    if (Committed != null)
      Committed (this, args);
  }

  /// <summary>
  /// Raises the <see cref="RollingBack"/> event.
  /// </summary>
  /// <param name="args">A <see cref="System.EventArgs"/> object that contains the event data.</param>
  protected virtual void OnRollingBack (EventArgs args)
  {
    if (RollingBack != null)
      RollingBack (this, args);
  }

  /// <summary>
  /// Raises the <see cref="RolledBack"/> event.
  /// </summary>
  /// <param name="args">A <see cref="System.EventArgs"/> object that contains the event data.</param>
  protected virtual void OnRolledBack (EventArgs args)
  {
    if (RolledBack != null)
      RolledBack (this, args);
  }

  /// <summary>
  /// Raises the <see cref="RelationChanging"/> event.
  /// </summary>
  /// <param name="args">A <see cref="RelationChangingEventArgs"/> object that contains the event data.</param>
  protected virtual void OnRelationChanging (RelationChangingEventArgs args)
  {
    if (RelationChanging != null)
      RelationChanging (this, args);
  }

  /// <summary>
  /// Raises the <see cref="RelationChanged"/> event.
  /// </summary>
  /// <param name="args">A <see cref="RelationChangedEventArgs"/> object that contains the event data.</param>
  protected virtual void OnRelationChanged (RelationChangedEventArgs args)
  {
    if (RelationChanged != null)
      RelationChanged (this, args);
  }

  /// <summary>
  /// Raises the <see cref="PropertyChanging"/> event.
  /// </summary>
  /// <param name="args">A <see cref="PropertyChangeEventArgs"/> object that contains the event data.</param>
  protected virtual void OnPropertyChanging (PropertyChangeEventArgs args)
  {
    if (PropertyChanging != null)
      PropertyChanging (this, args);
  }

  /// <summary>
  /// Raises the <see cref="PropertyChanged"/> event.
  /// </summary>
  /// <param name="args">A <see cref="PropertyChangeEventArgs"/> object that contains the event data.</param>
  protected virtual void OnPropertyChanged (PropertyChangeEventArgs args)
  {
    if (PropertyChanged != null)
      PropertyChanged (this, args);
  }

  /// <summary>
  /// Raises the <see cref="Deleting"/> event.
  /// </summary>
  /// <param name="args">A <see cref="System.EventArgs"/> object that contains the event data.</param>
  protected virtual void OnDeleting (EventArgs args)
  {
    if (Deleting != null)
      Deleting (this, args);
  }

  /// <summary>
  /// Raises the <see cref="Deleted"/> event.
  /// </summary>
  /// <param name="args">A <see cref="EventArgs"/> object that contains the event data.</param>
  protected virtual void OnDeleted (EventArgs args)
  {
    if (Deleted != null)
      Deleted (this, args);
  }

  internal void BeginRelationChange (
    string propertyName,
    DomainObject oldRelatedObject,
    DomainObject newRelatedObject)
  {
    ArgumentUtility.CheckNotNullOrEmpty ("propertyName", propertyName);

    RelationChangingEventArgs args = new RelationChangingEventArgs (propertyName, oldRelatedObject, newRelatedObject);
    OnRelationChanging (args);
  }

  internal void EndObjectLoading ()
  {
    OnLoaded ();
  }

  internal void EndRelationChange (string propertyName)
  {
    ArgumentUtility.CheckNotNullOrEmpty ("propertyName", propertyName);

    OnRelationChanged (new RelationChangedEventArgs (propertyName));
  }

  internal void BeginDelete ()
  {
    OnDeleting (EventArgs.Empty);
  }

  internal void EndDelete ()
  {
    OnDeleted (EventArgs.Empty);
  }

  internal void BeginCommit ()
  {
    OnCommitting (EventArgs.Empty);
  }

  internal void EndCommit ()
  {
    OnCommitted (EventArgs.Empty);
  }

  internal void BeginRollback ()
  {
    OnRollingBack (EventArgs.Empty);
  }

  internal void EndRollback ()
  {
    OnRolledBack (EventArgs.Empty);
  }

  internal void PropertyValueChanging (object sender, PropertyChangeEventArgs args)
  {
    OnPropertyChanging (args);
  }

  internal void PropertyValueChanged (object sender, PropertyChangeEventArgs args)
  {
    OnPropertyChanged (args);
  }

  protected void CheckIfObjectIsDiscarded ()
  {
    if (IsDiscarded)
      throw new ObjectDiscardedException (_dataContainer.GetID ());
  }
}
}