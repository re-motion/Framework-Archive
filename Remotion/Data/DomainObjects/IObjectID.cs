// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (c) rubicon IT GmbH, www.rubicon.eu
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
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Persistence.Configuration;

namespace Remotion.Data.DomainObjects
{
  /// <summary>
  /// Provides a covariant interface for the typed <see cref="ObjectID{T}"/>.
  /// </summary>
  /// <typeparam name="T">The class of the object.</typeparam>
  public interface IObjectID<out T> : IComparable
      where T : DomainObject
  {
    /// <summary>
    /// Gets the <see cref="Persistence.Configuration.StorageProviderDefinition"/> of the <see cref="Persistence.StorageProvider"/> which stores the object.
    /// </summary>
    StorageProviderDefinition StorageProviderDefinition { get; }

    /// <summary>
    /// Gets the ID value used to identify the object in the storage provider.
    /// </summary>
    /// <remarks>
    /// <b>Value</b> can be of type <see cref="System.Guid"/>, <see cref="System.Int32"/> or <see cref="System.String"/>.
    /// </remarks>
    object Value { get; }

    /// <summary>
    /// The class ID of the object class.
    /// </summary>
    string ClassID { get; }

    /// <summary>
    /// Gets the <see cref="Mapping.ClassDefinition"/> associated with this <see cref="ObjectID"/>.
    /// </summary>
    ClassDefinition ClassDefinition { get; }

    string ToString ();
    int GetHashCode ();
    bool Equals (object obj);
  }
}