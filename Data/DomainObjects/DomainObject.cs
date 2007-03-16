using System;
using Rubicon.Data.DomainObjects.DataManagement;
using Rubicon.Utilities;
using System.Reflection;
using Rubicon.Logging;
using Rubicon.Data.DomainObjects.Configuration;

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

  private static ILog s_log = LogManager.GetLogger (typeof (DomainObject));

  /// <summary>
  /// Creates a new instance of a domain object.
  /// </summary>
  /// <typeparam name="T">The type to be supported by the object.</typeparam>
  /// <returns>A new domain object instance.</returns>
  /// <remarks>
  /// <para>This method does not directly instantiate the given type <typeparamref name="T"/>, but instead dynamically creates a subclass that
  /// intercepts certain method calls in order to perform management tasks.</para>
  /// <para>This method ensures that the created domain object supports the new property syntax if the object's class is configured correspondingly
  /// (see <see cref="FactoryInstantiatedAttribute"/> and <see cref="FactoryInstantiationScope"/>).</para>
  /// <para>The given <paramref name="type"/> must implement a default constructor. This is not enforced by a <c>new()</c> constraint
  /// in order to support abstract classes with automatic properties.</para>
  /// </remarks>
  /// <exception cref="ArgumentException">The type <typeparamref name="T"/> is sealed or contains abstract methods (apart from automatic
  /// properties).</exception>
  /// <exception cref="MissingMethodException">The given type <typeparamref name="T"/> does not implement a public or protected default constructor.
  /// </exception>
  /// <exception cref="Exception">Any exception thrown by the constructor is propagated to the caller. (Note that this is different to
  /// <see cref="DomainObjectFactory.Create{T}"/>, where a <see cref="TargetInvocationException"/>  is thrown. For this method, the
  /// <see cref="TargetInvocationException"/> is logged and its inner exception is rethrown.)</exception>
  public static T Create<T> () where T : DomainObject
  {
    return (T) CreateInternal (typeof (T), new object[0]);
  }

  /// <summary>
  /// Creates a new instance of a domain object.
  /// </summary>
  /// <typeparam name="T">The type to be supported by the object.</typeparam>
  /// <param name="tx">The client transaction to be passed to the domain object's constructor.</param>
  /// <returns>A new domain object instance.</returns>
  /// <remarks>
  /// <para>This method does not directly instantiate the given type <typeparamref name="T"/>, but instead dynamically creates a subclass that
  /// intercepts certain method calls in order to perform management tasks.</para>
  /// <para>This method ensures that the created domain object supports the new property syntax if the object's class is configured correspondingly
  /// (see <see cref="FactoryInstantiatedAttribute"/> and <see cref="FactoryInstantiationScope"/>).</para>
  /// <para>The given type <typeparamref name="T"/> must implement a constructor taking a single <see cref="ClientTransaction"/> argument.</para>
  /// </remarks>
  /// <exception cref="ArgumentNullException">The <paramref name="clientTransaction"/> parameter is null.</exception>
  /// <exception cref="ArgumentException">The type <typeparamref name="T"/> is sealed or contains abstract methods (apart from automatic
  /// properties).</exception>
  /// <exception cref="MissingMethodException">The given <paramref name="type"/> does not implement a corresponding public or protected constructor.
  /// </exception>
  /// <exception cref="Exception">Any exception thrown by the constructor is propagated to the caller. (Note that this is different to
  /// <see cref="DomainObjectFactory.Create{T}(ClientTransaction)"/>, where a <see cref="TargetInvocationException"/>  is thrown. For this method, the
  /// <see cref="TargetInvocationException"/> is logged and its inner exception is rethrown.)</exception>
  public static T Create<T> (ClientTransaction tx) where T : DomainObject
  {
    ArgumentUtility.CheckNotNull ("tx", tx);
    return (T) CreateInternal (typeof (T), new object[] { tx });
  }

  private static DomainObject CreateInternal (Type type, object[] args)
  {
    try
    {
      if (ShouldUseFactoryForInstantiation (type))
      {
        return CreateInternalWithFactory (type, args);
      }
      else
      {
        return CreateInternalWithoutFactory (type, args);
      }
    }
    catch (TargetInvocationException ex)
    {
      s_log.Error ("TargetInvocationException in constructor call, rethrowing inner exception.", ex);
      throw ex.InnerException;
    }
  }

  private static DomainObject CreateInternalWithoutFactory (Type type, object[] args)
  {
    try
    {
      return (DomainObject) ReflectionUtility.CreateObject (type, args);
    }
    catch (ArgumentException ex)
    {
      throw new MissingMethodException ("The given type " + type.FullName + " does not implement a corresponding constructor.", ex);
    }
  }

  private static DomainObject CreateInternalWithFactory (Type type, object[] args)
  {
    return (DomainObject) DomainObjectsConfiguration.Current.MappingLoader.DomainObjectFactory.Create (type, args);
  }

  internal static DomainObject CreateWithDataContainer (DataContainer dataContainer)
  {
    ArgumentUtility.CheckNotNull ("dataContainer", dataContainer);

    return CreateInternal (dataContainer.DomainObjectType, new object[] { dataContainer });
  }

  // TODO: Change to use mapping instead of attribute later.
  public static bool ShouldUseFactoryForInstantiation (Type domainObjectType)
  {
    return FactoryInstantiationScope.WithinScope || domainObjectType.IsDefined (typeof (FactoryInstantiatedAttribute), true);
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
  public static T GetObject<T> (ObjectID id) where T : DomainObject
  {
    return (T) GetObject (id);
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
  protected static DomainObject GetObject (ObjectID id)
  {
    return GetObject (id, false);
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
    return (T) GetObject (id, includeDeleted);
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
  protected static DomainObject GetObject (ObjectID id, bool includeDeleted)
  {
    return GetObject (id, ClientTransaction.Current, includeDeleted);
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
    return (T) GetObject (id, clientTransaction);
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
  protected static DomainObject GetObject (ObjectID id, ClientTransaction clientTransaction)
  {
    return GetObject (id, clientTransaction, false);
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
    return (T) GetObject (id, clientTransaction, includeDeleted);
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
  protected static DomainObject GetObject (ObjectID id, ClientTransaction clientTransaction, bool includeDeleted)
  {
    ArgumentUtility.CheckNotNull ("id", id);
    ArgumentUtility.CheckNotNull ("clientTransaction", clientTransaction);

    return clientTransaction.GetObject (id, includeDeleted);
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
  protected DomainObject () : this (ClientTransaction.Current)
  {
  }

  /// <summary>
  /// Initializes a new <see cref="DomainObject"/>.
  /// </summary>
  /// <param name="clientTransaction">The <see cref="Rubicon.Data.DomainObjects.ClientTransaction"/> the <see cref="DomainObject"/> should be part of. Must not be <see langword="null"/>.</param>
  /// <exception cref="System.ArgumentNullException"><paramref name="clientTransaction"/> is <see langword="null"/>.</exception>
  protected DomainObject (ClientTransaction clientTransaction)
  {
    ArgumentUtility.CheckNotNull ("clientTransaction", clientTransaction);

    clientTransaction.NewObjectCreating (GetPublicDomainObjectType ());

    _dataContainer = clientTransaction.CreateNewDataContainer (GetPublicDomainObjectType ());
    _dataContainer.SetDomainObject (this);
  }

  /// <summary>
  /// Returns the public type representation of this domain object, i.e. the type object visible to mappings, database, etc.
  /// </summary>
  /// <returns>The public type representation of this domain object.</returns>
  /// <remarks>A domain object should override this method if it wants to impersonate one of its base types. The framework will handle this object
  /// as if it was of the type returned by this method and ignore its actual type.</remarks>
  protected virtual Type GetPublicDomainObjectType ()
  {
    return this.GetType ();
  }

  /// <summary>
  /// Infrastructure constructor necessary to load a <see cref="DomainObject"/> from a datasource.
  /// </summary>
  /// <remarks>
  /// All derived classes have to implement an (empty) constructor with this signature.
  /// Do not implement any initialization logic in this constructor, but use <see cref="DomainObject.OnLoaded"/> instead.
  /// </remarks>
  /// <param name="dataContainer">The newly loaded <b>DataContainer</b></param>
  protected DomainObject (DataContainer dataContainer)
  {
    ArgumentUtility.CheckNotNull ("dataContainer", dataContainer);

    _dataContainer = dataContainer;
  }

  // methods and properties

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
  /// Gets the value of the current <see cref="PropertyValue"/>.
  /// </summary>
  /// <typeparam name="T">The expected type of the property value.</typeparam>
  /// <returns>The value of the current property.</returns>
  /// <remarks>The current property must previously have been initialized with <see cref="PreparePropertyAccess" /> and should be de-initialized
  /// with <see cref="PropertyAccessFinished"/> when its value has been retrieved. Domain objects created with the <see cref="DomainObjectFactory"/>
  /// automatically initialize and de-initialize their virtual properties without needing any further work.</remarks>
  /// <exception cref="InvalidOperationException">The current property hasn't been initialized or there is no current property. Perhaps the domain 
  /// object was created by the <c>new</c> operator instead of the factory or the property is not virtual.</exception>
  /// <exception cref="InvalidTypeException">The current property's value does not match the requested type.</exception>
  /// <exception cref="DataManagement.ObjectDiscardedException">The object is already discarded. See <see cref="DataManagement.ObjectDiscardedException"/> for further information.</exception>
  protected internal virtual T GetPropertyValue<T> ()
  {
    string currentPropertyName = CurrentPropertyManager.CurrentPropertyName;
    if (currentPropertyName == null)
    {
      throw new InvalidOperationException ("The current property hasn't been initialized or there is no current property.");
    }
    else
    {
      object value = DataContainer[currentPropertyName];
      if (!(value is T))
      {
        throw new InvalidTypeException (currentPropertyName, typeof (T), value.GetType ());
      }
      else
      {
        return (T) value;
      }
    }
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
  /// <exception cref="DataManagement.ObjectDiscardedException">The object is already discarded. See <see cref="DataManagement.ObjectDiscardedException"/> for further information.</exception>
  protected internal virtual void SetPropertyValue (object value)
  {
    string currentPropertyName = CurrentPropertyManager.CurrentPropertyName;
    if (currentPropertyName == null)
    {
      throw new InvalidOperationException ("The current property hasn't been initialized or there is no current property.");
    }
    else
    {
      DataContainer[currentPropertyName] = value;
    }
  }

  /// <summary>
  /// Gets the related object of a given <paramref name="propertyName"/>.
  /// </summary>
  /// <param name="propertyName">The name of the property referring to the relation. <paramref name="propertyName"/> must refer to a one-to-one or many-to-one relation. Must not be <see langword="null"/>.</param>
  /// <returns>The <see cref="DomainObject"/> that is the current related object.</returns>
  /// <exception cref="System.ArgumentNullException"><paramref name="propertyName"/> is <see langword="null"/>.</exception>
  /// <exception cref="Rubicon.Utilities.ArgumentEmptyException"><paramref name="propertyName"/> is an empty string.</exception>
  /// <exception cref="DataManagement.ObjectDiscardedException">The object is already discarded. See <see cref="DataManagement.ObjectDiscardedException"/> for further information.</exception>
  /// <exception cref="System.ArgumentException"><paramref name="propertyName"/> does not refer to an one-to-one or many-to-one relation.</exception>
  protected virtual DomainObject GetRelatedObject (string propertyName)
  {
    ArgumentUtility.CheckNotNullOrEmpty ("propertyName", propertyName);
    CheckIfObjectIsDiscarded ();

    return ClientTransaction.GetRelatedObject (new RelationEndPointID (ID, propertyName));
  }

  /// <summary>
  /// Gets the original related object of a given <paramref name="propertyName"/> at the point of instantiation, loading, commit or rollback.
  /// </summary>
  /// <param name="propertyName">The name of the property referring to the relation. <paramref name="propertyName"/> must refer to a one-to-many relation. Must not be <see langword="null"/>.</param>
  /// <returns>The <see cref="DomainObject"/> that is the current related object.</returns>
  /// <exception cref="System.ArgumentNullException"><paramref name="propertyName"/> is <see langword="null"/>.</exception>
  /// <exception cref="Rubicon.Utilities.ArgumentEmptyException"><paramref name="propertyName"/> is an empty string.</exception>
  /// <exception cref="DataManagement.ObjectDiscardedException">The object is already discarded. See <see cref="DataManagement.ObjectDiscardedException"/> for further information.</exception>
  /// <exception cref="System.InvalidCastException"><paramref name="propertyName"/> does not refer to an <see cref="DataManagement.ObjectEndPoint"/>.</exception>
  /// <exception cref="System.ArgumentException"><paramref name="propertyName"/> does not refer to an one-to-one or many-to-one relation.</exception>
  protected virtual DomainObject GetOriginalRelatedObject (string propertyName)
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
  protected virtual DomainObjectCollection GetRelatedObjects (string propertyName)
  {
    ArgumentUtility.CheckNotNullOrEmpty ("propertyName", propertyName);
    CheckIfObjectIsDiscarded ();

    return ClientTransaction.GetRelatedObjects (new RelationEndPointID (ID, propertyName));
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
  protected virtual DomainObjectCollection GetOriginalRelatedObjects (string propertyName)
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
  protected void SetRelatedObject (string propertyName, DomainObject newRelatedObject)
  {
    ArgumentUtility.CheckNotNullOrEmpty ("propertyName", propertyName);
    CheckIfObjectIsDiscarded ();

    ClientTransaction.SetRelatedObject (new RelationEndPointID (ID, propertyName), newRelatedObject);
  }

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