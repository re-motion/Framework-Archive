﻿// This file is part of the re-motion Core Framework (www.re-motion.org)
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
using JetBrains.Annotations;
using Remotion.Globalization.Implementation;
using Remotion.ServiceLocation;

namespace Remotion.Globalization
{
  /// <summary>
  /// Defines an interface for retrieving the human-readable localized representation of the enumeration object.
  /// </summary>
  /// <seealso cref="EnumerationGlobalizationService"/>
  /// <threadsafety static="true" instance="true" />
  [ConcreteImplementation (typeof (EnumerationGlobalizationService), Lifetime = LifetimeKind.Singleton)]
  public interface IEnumerationGlobalizationService
  {
    /// <summary>
    /// Returns the human-readable enumeration name of the spefified reflection object.
    /// </summary>
    /// <param name="value">
    /// The <see cref="Enum"/> that defines the name for the resource lookup. Must not be <see langword="null" />.
    /// </param>
    /// <param name="result">The human-readable localized representation of the type or a version of the type name if no resource could be found.</param>
    /// <returns><value>True</value> if a resource could be found.</returns>
    bool TryGetEnumerationValueDisplayName ([NotNull]Enum value, out string result);
  }
}