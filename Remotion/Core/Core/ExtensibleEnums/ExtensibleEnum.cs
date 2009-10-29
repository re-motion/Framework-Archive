// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
// 
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// version 3.0 as published by the Free Software Foundation.
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
using Remotion.Utilities;

namespace Remotion.ExtensibleEnums
{
  /// <summary>
  /// Base class for extensible enums. Create extensible enums by deriving a class representing the enumeration
  /// from <see cref="ExtensibleEnum"/> and define extension methods on <see cref="ExtensibleEnumValues{T}"/>
  /// to define the values of the enumeration. Each value is uniquely identified by the <see cref="ID"/> string
  /// passed to the <see cref="ExtensibleEnum"/> constructor. Value comparisons act solely based on this identifier.
  /// </summary>
  public abstract class ExtensibleEnum
  {
    /// <summary>
    /// Initializes a new enumeration value.
    /// </summary>
    /// <param name="id">The identifier of the value being created. This identifier is used for equality comparisons
    /// and hash code calculations.</param>
    protected ExtensibleEnum (string id)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("id", id);
      ID = id;
    }

    /// <summary>
    /// Gets the identifier representing this extensible enum value.
    /// </summary>
    /// <value>The ID of this value.</value>
    public string ID { get; private set; }
  }
}