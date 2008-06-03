/* Copyright (C) 2005 - 2008 rubicon informationstechnologie gmbh
 *
 * This program is free software: you can redistribute it and/or modify it under 
 * the terms of the re:motion license agreement in license.txt. If you did not 
 * receive it, please visit http://www.re-motion.org/licensing.
 * 
 * Unless otherwise provided, this software is distributed on an "AS IS" basis, 
 * WITHOUT WARRANTY OF ANY KIND, either express or implied. 
 */

using System;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects
{
/// <summary>
/// Represents the method that will handle the <see cref="DomainObjectCollection.Adding"/>, <see cref="DomainObjectCollection.Added"/>, 
/// <see cref="DomainObjectCollection.Removed"/> and <see cref="DomainObjectCollection.Removing"/> events of a <see cref="DomainObjectCollection"/>.
/// </summary>
public delegate void DomainObjectCollectionChangeEventHandler (object sender, DomainObjectCollectionChangeEventArgs args);

/// <summary>
/// Provides data for the <see cref="DomainObjectCollection.Adding"/>, <see cref="DomainObjectCollection.Added"/>, 
/// <see cref="DomainObjectCollection.Removing"/> and <see cref="DomainObjectCollection.Removed"/> event of a <see cref="DomainObjectCollection"/>.
/// </summary>
[Serializable]
public class DomainObjectCollectionChangeEventArgs : EventArgs
{
  private DomainObject _domainObject;

  /// <summary>
  /// Initializes a new instance of the <b>DomainObjectCollectionChangingEventArgs</b> class.
  /// </summary>
  /// <param name="domainObject">The <see cref="Remotion.Data.DomainObjects.DomainObject"/> that is being added or removed to the collection. Must not be <see langword="null"/>.</param>
  /// <exception cref="System.ArgumentNullException"><paramref name="domainObject"/> is <see langword="null"/>.</exception>
  public DomainObjectCollectionChangeEventArgs (DomainObject domainObject)
  {
    ArgumentUtility.CheckNotNull ("domainObject", domainObject);
    _domainObject = domainObject;
  }

  /// <summary>
  /// Gets the <see cref="Remotion.Data.DomainObjects.DomainObject"/> that is being added or removed.
  /// </summary>
  public DomainObject DomainObject
  {
    get { return _domainObject; }
  }
}
}
