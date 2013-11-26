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
using System.Collections.Specialized;

namespace Remotion.Globalization
{
  /// <summary>
  ///   An interface for defining a string resource manager.
  /// </summary>
  public interface IResourceManager : INullObject
  {
    /// <summary>
    ///   Returns all string resources inside the resource manager.
    /// </summary>
    /// <returns>
    ///   A collection of string pairs, the key being the resource's ID, the value being the string.
    /// </returns>
    NameValueCollection GetAllStrings ();

    /// <summary>
    ///   Searches for all string resources inside the resource manager whose name is prefixed 
    ///   with a matching tag.
    /// </summary>
    /// <param name="prefix"> The prefix all returned string resources must have. </param>
    /// <returns>
    ///   A collection of string pairs, the key being the resource's ID, the value being the string.
    /// </returns>
    NameValueCollection GetAllStrings (string prefix);

    /// <summary>
    ///   Gets the value of the specified String resource.
    /// </summary>
    /// <param name="id">The ID of the resource to get. </param>
    /// <returns>
    ///   The value of the resource. If no match is possible, the identifier is returned.
    /// </returns>
    //TODO AO: Refactor to TryGetString (id, out value) :: bool, value = null of try returns false
    //TODO AO: Move existing API to extension method, returns identifier if try returns false, add NotNull
    //TODO AO: Add extension overload GetStringOrDefault, returns null of try returns false, add CanBeNull
    string GetString (string id);

    /// <summary>Tests whether the <see cref="IResourceManager"/> contains the specified resource.</summary>
    /// <param name="id">The ID of the resource to look for.</param>
    /// <returns><see langword="true"/> if the <see cref="IResourceManager"/> contains the specified resource.</returns>
    //TODO AO: Move existing API to extension method and make obsolete, new method ContainsString
    //TODO AO: Check for usages in remotion that can be changed to TryGet
    bool ContainsResource (string id);

    string Name { get; }
  }
}
