using System;
using System.ComponentModel;

using Rubicon.Data.DomainObjects.DataManagement;
using Rubicon.Utilities;

namespace Rubicon.Data.DomainObjects
{
//Documentation: All done

/// <summary>
/// Represents the method that will handle the <see cref="DomainObjectCollection.Changing"/> event of a <see cref="DomainObjectCollection"/>.
/// </summary>
public delegate void DomainObjectCollectionChangingEventHandler (object sender, DomainObjectCollectionChangingEventArgs args);
/// <summary>
/// Represents the method that will handle the <see cref="DomainObjectCollection.Changed"/> event of a <see cref="DomainObjectCollection"/>.
/// </summary>
public delegate void DomainObjectCollectionChangedEventHandler (object sender, DomainObjectCollectionChangedEventArgs args);

/// <summary>
/// Provides data for the <see cref="DomainObjectCollection.Changing"/> event of a <see cref="DomainObjectCollection"/>.
/// </summary>
public class DomainObjectCollectionChangingEventArgs : EventArgs
{
  private DomainObject _domainObject;

  /// <summary>
  /// Initializes a new instance of the <b>DomainObjectCollectionChangingEventArgs</b> class.
  /// </summary>
  /// <param name="domainObject">The <see cref="Rubicon.Data.DomainObjects.DomainObject"/> that is being added or removed to the collection.</param>
  /// <exception cref="System.ArgumentNullException"><i>domainObject</i> is a null reference.</exception>
  public DomainObjectCollectionChangingEventArgs (DomainObject domainObject)
  {
    ArgumentUtility.CheckNotNull ("domainObject", domainObject);
    _domainObject = domainObject;
  }

  /// <summary>
  /// Gets the <see cref="Rubicon.Data.DomainObjects.DomainObject"/> that is being added or removed.
  /// </summary>
  public DomainObject DomainObject
  {
    get { return _domainObject; }
  }
}

/// <summary>
/// Provides data for the <see cref="DomainObjectCollection.Changing"/> event of a <see cref="DomainObjectCollection"/>.
/// </summary>
public class DomainObjectCollectionChangedEventArgs : EventArgs
{
  private DomainObject _domainObject;

  /// <summary>
  /// Initializes a new instance of the <b>DomainObjectCollectionChangedEventArgs</b> class.
  /// </summary>
  /// <param name="domainObject">The <see cref="Rubicon.Data.DomainObjects.DomainObject"/> that has been added or removed to the collection.</param>
  /// <exception cref="System.ArgumentNullException"><i>domainObject</i> is a null reference.</exception>
  public DomainObjectCollectionChangedEventArgs (DomainObject domainObject)
  {
    ArgumentUtility.CheckNotNull ("domainObject", domainObject);
    _domainObject = domainObject;
  }

  /// <summary>
  /// Gets the <see cref="Rubicon.Data.DomainObjects.DomainObject"/> that has been added or removed.
  /// </summary>
  public DomainObject DomainObject
  {
    get { return _domainObject; }
  }
}
}
