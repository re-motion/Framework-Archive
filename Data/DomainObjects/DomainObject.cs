using System;
using System.Reflection;
using Rubicon.Collections;
using Rubicon.Data.DomainObjects.DataManagement;
using Rubicon.Data.DomainObjects.Mapping;
using Rubicon.Data.DomainObjects.Persistence;
using Rubicon.Reflection;
using Rubicon.Utilities;
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

  private static readonly MethodInfo s_newObjectWithDefaultConstructorMethodInfo =
      typeof (DomainObject).GetMethod ("NewObjectWithDefaultConstructor", BindingFlags.NonPublic | BindingFlags.Static, null, new Type[0], null);

  #region Creation and GetObject factory methods

  /// <summary>
  /// Returns an invocation object creating a new instance of a concrete domain object for the current <see cref="DomainObjects.ClientTransaction"/>.
  /// </summary>
  /// <typeparam name="T">The concrete type to be implemented by the object.</typeparam>
  /// <returns>An <see cref="FuncInvoker{T}"/> object used to create a new domain object instance.</returns>
  /// <remarks>
  /// <para>
  /// This method's return value is an <see cref="FuncInvoker{T}"/> object, which can be used to specify the required constructor and 
  /// pass it the necessary arguments in order to create a new domain object. Depending on the mapping being used by the object (and
  /// on whether <see cref="FactoryInstantiationScope"/> is being used), one of two methods of object creation is used: legacy or via factory.
  /// </para>
  /// <para>
  /// Legacy objects are created by simply invoking the constructor matching the arguments passed to the <see cref="FuncInvoker{T}"/>
  /// object returned by this method.
  /// </para>
  /// <para>
  /// Objects created by the factory are not directly instantiated; instead a proxy is dynamically created for performing management tasks.
  /// </para>
  /// <para>This method should not be directly invoked by a user, but instead by static factory methods of classes derived from
  /// <see cref="DomainObject"/>.</para>
  /// <para>For more information, also see the constructor documentation (<see cref="DomainObject()"/>).</para>
  /// </remarks>
  /// <seealso cref="DomainObject()"/>
  /// <exception cref="ArgumentException">The type <typeparamref name="T"/> cannot be extended to a proxy, for example because it is sealed
  /// or abstract (apart from automatic properties).</exception>
  /// <exception cref="MissingMethodException">The given type <typeparamref name="T"/> does not implement the required protected
  /// constructor (see Remarks section).
  /// </exception>
  /// <exception cref="Exception">Any exception thrown by the constructor is propagated to the caller.</exception>
  protected static IFuncInvoker<T> NewObject<T> () where T : DomainObject
  {
    return GetCreator (typeof (T)).GetTypesafeConstructorInvoker<T>();
  }

  /// <summary>
  /// Creates a <see cref="DomainObject"/> for a given <see cref="Type"/> using it's default constuctor.
  /// </summary>
  /// <param name="type">The <see cref="Type"/> of the <see cref="DomainObject"/> to be created.</param>
  /// <seealso cref="DomainObject()"/>
  /// <exception cref="ArgumentException">The <paramref name="type"/> cannot be extended to a proxy, for example because it is sealed
  /// or abstract (apart from automatic properties).</exception>
  /// <exception cref="MissingMethodException">The given <paramref name="type"/> does not implement the required protected
  /// constructor (see Remarks section).
  /// </exception>
  /// <exception cref="Exception">Any exception thrown by the constructor is propagated to the caller.</exception>
  public static DomainObject NewObject (Type type)
  {
    ArgumentUtility.CheckNotNullAndTypeIsAssignableFrom ("type", type, typeof (DomainObject));
    //TODO: FS: replace this with the native syntax once the subclassproxying is refactored.
    return (DomainObject) s_newObjectWithDefaultConstructorMethodInfo.MakeGenericMethod (type).Invoke (null, new object[0]);
  }

  private static T NewObjectWithDefaultConstructor<T> () where T : DomainObject
  {
    return DomainObject.NewObject<T>().With();
  }

  /// <summary>
  /// Creates a <see cref="DomainObject"/> from a given data container.
  /// </summary>
  /// <param name="dataContainer">The data container for the new domain object.</param>
  /// <returns>A new <see cref="DomainObject"/> for the given data container.</returns>
  /// <remarks>
  /// <para>This method is used by the <see cref="DataContainer"/> class when it is asked to load an object. It requires an infrastructure
  /// constructor taking a single <see cref="DataContainer"/> argument on the domain object's class (see <see cref="DomainObject(DataContainer)"/>).
  /// </para>
  /// </remarks>
  /// <exception cref="ArgumentNullException">The <paramref name="dataContainer"/> parameter is <see langword="null"/>.</exception>
  /// <exception cref="MissingMethodException">The instantiated type does not implement the required public or protected constructor
  /// (see Remarks section).</exception>
  /// <exception cref="Exception">Any exception thrown by the constructor is propagated to the caller.</exception>
  internal static DomainObject CreateWithDataContainer (DataContainer dataContainer)
  {
    ArgumentUtility.CheckNotNull ("dataContainer", dataContainer);
    return GetCreator (dataContainer.DomainObjectType).CreateWithDataContainer (dataContainer);
  }

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
  protected static T GetObject<T> (ObjectID id) where T : DomainObject
  {
    return GetObject<T> (id, false);
  }

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
  public static DomainObject GetObject (ObjectID id)
  {
    return GetObject<DomainObject> (id);
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
    ArgumentUtility.CheckNotNull ("id", id);

    return (T) ClientTransactionScope.CurrentTransaction.GetObject (id, includeDeleted);
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
  public static DomainObject GetObject (ObjectID id, bool includeDeleted)
  {
    return GetObject<DomainObject> (id, includeDeleted);
  }

  /// <summary>
  /// Loads a DomainObject into another transaction.
  /// </summary>
  /// <typeparam name="T">The type of domain object to load.</typeparam>
  /// <param name="sourceObject">The source object to load in the destination transaction.</param>
  /// <param name="destinationTransaction">The destination transaction to load the source object in.</param>
  /// <returns>A <typeparamref name="T"/> reference which constitutes the same domain object as <paramref name="sourceObject"/>, but loaded
  /// into <paramref name="destinationTransaction"/>.</returns>
  /// <remarks>
  /// This method is the equivalent of opening a <see cref="ClientTransactionScope"/> for the <paramref name="destinationTransaction"/> and
  /// calling <see cref="DomainObject.GetObject{T}(ObjectID)"/> with <paramref name="sourceObject">sourceObject's</paramref> <see cref="ID"/>.
  /// </remarks>
  public static T LoadIntoTransaction<T> (T sourceObject, ClientTransaction destinationTransaction)
      where T : DomainObject
  {
    using (new ClientTransactionScope (destinationTransaction, AutoRollbackBehavior.None))
    {
      return DomainObject.GetObject<T> (sourceObject.ID);
    }
  }

  #endregion

  #region Obsolete GetObject legacy methods

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
  [Obsolete ("This method is obsolete, use the generic variant instead.", true)]
  protected static DomainObject GetObject (ObjectID id, ClientTransaction clientTransaction)
  {
    ArgumentUtility.CheckNotNull ("clientTransaction", clientTransaction);
    using (new ClientTransactionScope (clientTransaction))
    {
      return GetObject<DomainObject> (id);
    }
  }

  /// <summary>
  /// Gets a <see cref="DomainObject"/> that is already loaded or attempts to load it from the datasource.
  /// </summary>
  /// <param name="id">The <see cref="ObjectID"/> of the <see cref="DomainObject"/> that is loaded. Must not be <see langword="null"/>.</param>
  /// <param name="clientTransaction">The <see cref="Rubicon.Data.DomainObjects.ClientTransaction"/> that is used to load the <see cref="DomainObject"/>.</param>
  /// <param name="includeDeleted">Indicates if the method should return <see cref="DomainObject"/>s that are already deleted.</param>
  /// <returns>The <see cref="DomainObject"/> with the specified <paramref name="id"/>.</returns>
  /// <exception cref="System.ArgumentNullException"><paramref name="id"/> or <paramref name="clientTransaction"/>is <see langword="null"/>.</exception>
  /// <exception cref="Persistence.StorageProviderException">
  ///   The Mapping does not contain a class definition for the given <paramref name="id"/>.<br /> -or- <br />
  ///   An error occurred while reading a <see cref="PropertyValue"/>.<br /> -or- <br />
  ///   An error occurred while accessing the datasource.
  /// </exception>
  /// <exception cref="MissingMethodException">The concrete <see cref="DomainObject"/> doesn't implement the required constructor.</exception>
  [Obsolete ("This method is obsolete, use the generic variant instead.", true)]
  protected static DomainObject GetObject (ObjectID id, ClientTransaction clientTransaction, bool includeDeleted)
  {
    ArgumentUtility.CheckNotNull ("clientTransaction", clientTransaction);
    using (new ClientTransactionScope (clientTransaction))
    {
      return GetObject<DomainObject> (id, includeDeleted);
    }
  }

  #endregion

  // True if the domain object type requires to be instantiated via the DomainObjectFactory; false if it is safe to just invoke its constructor.
  private static bool ShouldUseFactoryForInstantiation (Type domainObjectType)
  {
    if (FactoryInstantiationScope.WithinScope)
      return true;
    
    ClassDefinition classDefinition = MappingConfiguration.Current.ClassDefinitions.GetMandatory (domainObjectType);
    return classDefinition is ReflectionBasedClassDefinition;
  }

  // Returns a strategy object for creating instances of the given domain object type.
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

  private ObjectID _id;

  // construction and disposing

  /// <summary>
  /// Initializes a new <see cref="DomainObject"/> with the current <see cref="DomainObjects.ClientTransaction"/>.
  /// </summary>
  /// <remarks>Any constructors implemented on concrete domain objects should delegate to this base constructor, apart from the infrastructure
  /// constructor (see <see cref="DomainObject(DomainObjects.DataContainer)"/>). As domain objects generally should not be constructed via the
  /// <c>new</c> operator, these constructors must therefor remain protected, and the concrete domain objects should have a static "NewObject" method,
  /// which delegates to <see cref="DomainObject.NewObject"/>, passing it the required constructor arguments.</remarks>
  protected DomainObject ()
  {
    ClientTransactionScope.CurrentTransaction.TransactionEventSink.NewObjectCreating (GetPublicDomainObjectType ());

    DataContainer firstDataContainer = ClientTransactionScope.CurrentTransaction.CreateNewDataContainer (GetPublicDomainObjectType ());
    firstDataContainer.SetDomainObject (this);

    InitializeFromDataContainer (firstDataContainer);
  }

  #region Legacy constructors
  /// <summary>
  /// Initializes a new <see cref="DomainObject"/>.
  /// </summary>
  /// <param name="clientTransaction">The <see cref="Rubicon.Data.DomainObjects.ClientTransaction"/> the <see cref="DomainObject"/> should be part of. Must not be <see langword="null"/>.</param>
  /// <exception cref="System.ArgumentNullException"><paramref name="clientTransaction"/> is <see langword="null"/>.</exception>
  [Obsolete ("This constructor is obsolete, use the DomainObject() one in conjunction with ClientTransactionScope instead.", true)]
  protected DomainObject (ClientTransaction clientTransaction)
  {
    ArgumentUtility.CheckNotNull ("clientTransaction", clientTransaction);

    clientTransaction.TransactionEventSink.NewObjectCreating (GetPublicDomainObjectType ());

    DataContainer firstDataContainer = clientTransaction.CreateNewDataContainer (GetPublicDomainObjectType ());
    firstDataContainer.SetDomainObject (this);

    InitializeFromDataContainer (firstDataContainer);
  }

  /// <summary>
  /// Infrastructure constructor for loading a <see cref="DomainObject"/> from a data source when the legacy mapping is used.
  /// </summary>
  /// <remarks>
  /// <para>
  /// For the legacy mapping, all derived classes have to implement an (empty) constructor with this signature.
  /// Do not implement any initialization logic in this constructor, but use <see cref="DomainObject.OnLoaded"/> instead.
  /// </para>
  /// <para>
  /// This constructor should not be used when using the reflection-based mapping. The reflection-based mapping instantiates objects without
  /// using any constructor.
  /// </para>
  /// </remarks>
  /// <param name="dataContainer">The <see cref="DomainObjects.DataContainer"/> to be associated with the loaded domain object.</param>
  /// <exception cref="ArgumentNullException">The <paramref name="dataContainer"/> parameter is null.</exception>
  protected DomainObject (DataContainer dataContainer)
  {
    ArgumentUtility.CheckNotNull ("dataContainer", dataContainer);

    InitializeFromDataContainer (dataContainer);
  }
  #endregion

  // methods and properties

  /// <summary>
  /// Sets the data container during the loading process of a domain object.
  /// </summary>
  /// <param name="dataContainer">The data container to be associated with the loaded domain object.</param>
  /// <exception cref="ArgumentNullException">The <paramref name="dataContainer"/> parameter is null.</exception>
  internal void InitializeFromDataContainer (DataContainer dataContainer)
  {
    ArgumentUtility.CheckNotNull ("dataContainer", dataContainer);

    _id = dataContainer.ID;
    dataContainer.ClientTransaction.EnlistNewlyCreatedDomainObject (this);
  }

  /// <summary>
  /// GetType might return a <see cref="Type"/> object for a generated class, which is usually not what is expected.
  /// <see cref="DomainObject.GetPublicDomainObjectType"/> can be used to get the Type object of the original underlying domain object type. If
  /// the <see cref="Type"/> object for the generated class is explicitly required, this object can be cast to 'object' before calling GetType.
  /// </summary>
  [Obsolete ("GetType might return a Type object for a generated class., which is usually not what is expected. "
    + "DomainObject.GetPublicDomainObjectType can be used to get the Type object of the original underlying domain object type. If the Type object"
   + "for the generated class is explicitly required, this object can be cast to 'object' before calling GetType.", true)]
  public new Type GetType ()
  {
    throw new InvalidOperationException ("DomainObject.GetType should not be used.");
  }

  /// <summary>
  /// Returns the public type representation of this domain object, i.e. the type object visible to mappings, database, etc.
  /// </summary>
  /// <returns>The public type representation of this domain object.</returns>
  /// <remarks>A domain object should override this method if it wants to impersonate one of its base types. The framework will handle this object
  /// as if it was of the type returned by this method and ignore its actual type.</remarks>
  public virtual Type GetPublicDomainObjectType ()
  {
    return base.GetType ();
  }

  /// <summary>
  /// Gets the <see cref="ObjectID"/> of the <see cref="DomainObject"/>.
  /// </summary>
  /// <exception cref="DataManagement.ObjectDiscardedException">The object is already discarded. See <see cref="DataManagement.ObjectDiscardedException"/> for further information.</exception>
  public ObjectID ID
  {
    get { return _id; }
  }

  /// <summary>
  /// Gets the current state of the <see cref="DomainObject"/> in the <see cref="ClientTransactionScope.CurrentTransaction"/>.
  /// </summary>
  public StateType State
  {
    get
    {
      if (IsDiscarded)
        return StateType.Discarded;
      else
      {
        DataContainer dataContainer = GetDataContainer();
        if (dataContainer.State == StateType.Unchanged)
        {
          if (ClientTransactionScope.CurrentTransaction.HasRelationChanged (this))
            return StateType.Changed;
          else
            return StateType.Unchanged;
        }

        return dataContainer.State;
      }
    }
  }

  /// <summary>
  /// Marks the <see cref="DomainObject"/> as changed. If the object's previous <see cref="State"/> was <see cref="StateType.Unchanged"/>, it
  /// will be <see cref="StateType.Changed"/> after this method has been called.
  /// </summary>
  public void MarkAsChanged ()
  {
    CheckIfObjectIsDiscarded ();

    DataContainer dataContainer = GetDataContainer ();
    dataContainer.MarkAsChanged ();
  }


  /// <summary>
  /// Gets a value indicating the discarded status of the object.
  /// </summary>
  /// <remarks>
  /// For more information why and when an object is discarded see <see cref="Rubicon.Data.DomainObjects.DataManagement.ObjectDiscardedException"/>.
  /// </remarks>
  public bool IsDiscarded
  {
    get { return ClientTransactionScope.CurrentTransaction.DataManager.IsDiscarded (ID); }
  }

  protected internal void CheckIfObjectIsDiscarded ()
  {
    if (IsDiscarded)
      throw new ObjectDiscardedException (ID);
  }

  /// <summary>
  /// </summary>
  /// <value></value>
	[Obsolete ("Do not access the DataContainer of a DomainObject to retrieve field values, use its Properties member instead.")]
	protected DataContainerIndirection DataContainer
	{
		get
    {
      CheckIfObjectIsDiscarded ();
      CheckIfRightTransaction ();
      return new DataContainerIndirection (this);
    }
	}

		/// <summary>
  /// Gets the <see cref="DomainObjects.DataContainer"/> of the <see cref="DomainObject"/> in the <see cref="ClientTransactionScope.CurrentTransaction"/>.
  /// </summary>
  /// <exception cref="DataManagement.ObjectDiscardedException">The object is already discarded. See <see cref="DataManagement.ObjectDiscardedException"/> for further information.</exception>
  internal DataContainer GetDataContainer()
  {
    CheckIfRightTransaction ();

    return GetDataContainerForTransaction (ClientTransactionScope.CurrentTransaction);
  }

  internal DataContainer GetDataContainerForTransaction (ClientTransaction transaction)
  {
    if (transaction.DataManager.IsDiscarded (ID))
      throw new ObjectDiscardedException (ID);

    DataContainer dataContainer = transaction.DataManager.DataContainerMap[ID];
    if (dataContainer == null)
      dataContainer = transaction.LoadDataContainerForExistingObject (this);
    Assertion.IsNotNull (dataContainer);

    return dataContainer;
  }

  /// <summary>
  /// Deletes the <see cref="DomainObject"/>.
  /// </summary>
  /// <exception cref="DataManagement.ObjectDiscardedException">The object is already discarded. See <see cref="DataManagement.ObjectDiscardedException"/> for further information.</exception>
  /// <remarks>To perform custom actions when a <see cref="DomainObject"/> is deleted <see cref="OnDeleting"/> and <see cref="OnDeleted"/> should be overridden.</remarks>
  protected void Delete ()
  {
    CheckIfObjectIsDiscarded ();
    CheckIfRightTransaction ();

    ClientTransactionScope.CurrentTransaction.Delete (this);
  }

  #region Transaction handling
  /// <summary>
  /// Determines whether this instance can be used in the specified transaction.
  /// </summary>
  /// <param name="transaction">The transaction to check this object against.</param>
  /// <returns>
  /// True if this instance can be used in the specified transaction; otherwise, false.
  /// </returns>
  /// <remarks>If this method returns false, <see cref="ClientTransaction.EnlistDomainObject"/> can be used to enlist this instance in another
  /// transaction.</remarks>
  public bool CanBeUsedInTransaction (ClientTransaction transaction)
  {
    ArgumentUtility.CheckNotNull ("transaction", transaction);
    if (transaction.IsEnlisted (this))
      return true;
    else if (ClientTransactionScope.ActiveScope != null && ClientTransactionScope.ActiveScope.AutoEnlistDomainObjects)
    {
      transaction.EnlistDomainObject (this);
      return true;
    }
    else
      return false;
  }

  private void CheckIfRightTransaction ()
  {
    if (!CanBeUsedInTransaction (ClientTransactionScope.CurrentTransaction))
    {
      string message = string.Format ("Domain object '{0}' cannot be used in the current transaction as it was loaded or created in another "
          + "transaction. Use a ClientTransactionScope to set the right transaction, or call EnlistInTransaction to enlist the object "
          + "in the current transaction.", ID);
      throw new ClientTransactionsDifferException (message);
    }
  }

  #endregion

  #region Property access

  /// <summary>
  /// Prepares access to the <see cref="PropertyValue"/> of the given name.
  /// </summary>
  /// <param name="propertyName">The name of the <see cref="PropertyValue"/> to be accessed.</param>
  /// <remarks>This method prepares the given property for access via <see cref="CurrentProperty"/>.
  /// It is automatically invoked for virtual properties in domain objects created with the <see cref="DomainObjectFactory"/> and thus doesn't
  /// have to be called manually for these objects. If you choose to invoke <see cref="PreparePropertyAccess"/> and
  /// <see cref="PropertyAccessFinished"/> yourself, be sure to finish the property access with exactly one call to 
  /// <see cref="PropertyAccessFinished"/> from a finally-block.</remarks>
  /// <exception cref="System.ArgumentNullException"><paramref name="propertyName"/> is <see langword="null"/>.</exception>
  /// <exception cref="Rubicon.Utilities.ArgumentEmptyException"><paramref name="propertyName"/> is an empty string.</exception>
  /// <exception cref="ArgumentException">The <paramref name="propertyName"/> parameter does not denote a valid property.</exception>
  /// <exception cref="DataManagement.ObjectDiscardedException">The object is already discarded. See <see cref="DataManagement.ObjectDiscardedException"/> for further information.</exception>
  protected internal virtual void PreparePropertyAccess (string propertyName)
  {
    ArgumentUtility.CheckNotNullOrEmpty ("propertyName", propertyName);
    CheckIfObjectIsDiscarded();
    if (!PropertyAccessor.IsValidProperty (GetDataContainer().ClassDefinition, propertyName))
    {
      string message = string.Format (
          "The property identifier '{0}' is not a valid property of domain object type {1}.",
          propertyName,
          GetDataContainer().ClassDefinition.ClassType.FullName);
      throw new ArgumentException (message, "propertyName");
    }

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
      throw new InvalidOperationException (
          "There is no current property or it hasn't been properly initialized. Is the surrounding property virtual?");
    else
      return propertyName;
  }

  /// <summary>
  /// Provides simple, encapsulated access to the current property.
  /// </summary>
  /// <value>A <see cref="PropertyAccessor"/> object encapsulating the current property.</value>
  /// <remarks>
  /// The structure returned by this method allows simple access to the property's value and mapping definition objects regardless of
  /// whether it is a simple value property, a related object property, or a related object collection property.
  /// </remarks>
  /// <exception cref="InvalidOperationException">The current property hasn't been initialized or there is no current property. Perhaps the domain 
  /// object was created with the <c>new</c> operator instead of using the <see cref="NewObject{T}"/> method, or the property is not virtual.</exception>
  protected internal PropertyAccessor CurrentProperty
  {
    get
    {
      string propertyName = GetAndCheckCurrentPropertyName();
      return Properties[propertyName];
    }
  }

  /// <summary>
  /// Provides simple, encapsulated access to the properties of this <see cref="DomainObject"/>.
  /// </summary>
  /// <returns>A <see cref="PropertyIndexer"/> object which can be used to select a specific property of this <see cref="DomainObject"/>.</returns>
  protected internal PropertyIndexer Properties
  {
    get
    {
      CheckIfObjectIsDiscarded ();
      CheckIfRightTransaction ();
      return new PropertyIndexer (this);
    }
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
  [Obsolete ("This method is obsolete, use 'Properties' instead.")]
  protected internal virtual DomainObject GetRelatedObject (string propertyName)
  {
    ArgumentUtility.CheckNotNullOrEmpty ("propertyName", propertyName);
    return (DomainObject) Properties[propertyName].GetValueWithoutTypeCheck();
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
  [Obsolete ("This method is obsolete, use 'Properties' instead.")]
  protected internal virtual DomainObject GetOriginalRelatedObject (string propertyName)
  {
    ArgumentUtility.CheckNotNullOrEmpty ("propertyName", propertyName);
    return (DomainObject) Properties[propertyName].GetOriginalValueWithoutTypeCheck();
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
  [Obsolete ("This method is obsolete, use 'Properties' instead.")]
  protected internal virtual DomainObjectCollection GetRelatedObjects (string propertyName)
  {
    ArgumentUtility.CheckNotNullOrEmpty ("propertyName", propertyName);
    return (DomainObjectCollection) Properties[propertyName].GetValueWithoutTypeCheck();
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
  [Obsolete ("This method is obsolete, use 'Properties' instead.")]
  protected internal virtual DomainObjectCollection GetOriginalRelatedObjects (string propertyName)
  {
    ArgumentUtility.CheckNotNullOrEmpty ("propertyName", propertyName);
    return (DomainObjectCollection) Properties[propertyName].GetOriginalValueWithoutTypeCheck ();
  }

  /// <summary>
  /// Sets a relation to another object.
  /// </summary>
  /// <param name="propertyName">The name of the property referring to the relation, that should relate to <paramref name="newRelatedObject"/>. Must not be <see langword="null"/>.</param>
  /// <param name="newRelatedObject">The new <see cref="DomainObject"/> that should be related; <see langword="null"/> indicates that no object should be referenced.</param>
  /// <exception cref="System.ArgumentNullException"><paramref name="propertyName"/> is <see langword="null"/>.</exception>
  /// <exception cref="Rubicon.Utilities.ArgumentEmptyException"><paramref name="propertyName"/> is an empty string.</exception>
  /// <exception cref="DataManagement.ObjectDiscardedException">The object is already discarded. See <see cref="DataManagement.ObjectDiscardedException"/> for further information.</exception>
  [Obsolete ("This method is obsolete, use 'Properties' instead.")]
  protected internal void SetRelatedObject (string propertyName, DomainObject newRelatedObject)
  {
    ArgumentUtility.CheckNotNullOrEmpty ("propertyName", propertyName);
    Properties[propertyName].SetValueWithoutTypeCheck ((object) newRelatedObject);
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
    OnLoaded();
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
}
}
